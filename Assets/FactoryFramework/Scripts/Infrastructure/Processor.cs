using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FactoryFramework
{
    public class Processor : LogisticComponent
    {
        [Tooltip("Active Recipe")]
        public Recipe recipe;
        public float RecipeStartTime { get; private set; }

        public Recipe[] validRecipes;
        public Recipe[] invalidRecipes;

        public LocalStorage[] inputItems;
        public LocalStorage[] outputItems;

        private Coroutine _currRoutine;

        #region Overrides
        public override Item OutputItem
        {
            get
            {
                var output = outputItems.Where(o => o.ItemType != null).FirstOrDefault();
                if (output != null)
                    return output.ItemType;
                return null;
            }
        }
        public override bool CanRecieveItem(Item item)
        {
            // check if this is a valid item to recieve            
            if (recipe != null)
            {
                var validItems = inputItems.Select(x => x.ItemType).Union(recipe?.InputItems);
                if (!validItems.Contains(item))
                {
                    Debug.LogWarning($"Invalid item type for this processor with this recipe {recipe.name}");
                    return false;
                }
            }
            // find a stack with this item and make sure it isnt full
            var matchingStack = inputItems.Where(i => i.ItemType == item).FirstOrDefault();
            if (matchingStack != null)
            {
                if (matchingStack.IsFull)
                    return false;
                return true;
            }
            // check for empty stacks
            return inputItems.Any(i => (i.ItemType == null));
        }
        public override bool RecieveItem(Item item)
        {
            var matchingStack = inputItems.Where(i => i.ItemType == item && !i.IsFull).FirstOrDefault();
            if (matchingStack != null)
            {
                matchingStack.Add(item);
                return true;
            }
            var emptyStack = inputItems.Where(i => i.ItemType == null).FirstOrDefault();
            if (emptyStack != null)
            {
                emptyStack.Add(item);
                return true;
            }
            return false;
        }
        public override bool TransferItem(LogisticComponent output)
        {
            var matchingOutput = outputItems.Where(o => o.ItemType != null && o.itemStack.amount>0).FirstOrDefault();
            if (matchingOutput != null)
            {
                matchingOutput.Remove();
            }
            return false;
        }
        #endregion

        #region Lifecycle
        private void OnEnable()
        {
            IsWorking = false;
            recipe = null;
        }
        private void OnDisable()
        {
            IsWorking = false;
            StopAllCoroutines();
            _currRoutine = null;
        }
        #endregion

        #region Workload
        private bool MissingIngredients()
        {
            var inputItemTypes = inputItems.Select(i => i.ItemType);
            // if the recipe input items are not all present in the inputItems
            if (recipe.InputItems.Intersect(inputItemTypes).Count() != recipe.InputItems.Count())
            {
                return true;
            }
            return false;
        }
        private int CalculateMaxProduction()
        {
            int maxProduced = int.MaxValue;
            foreach (var ingredient in recipe.inputs)
            {
                var matchingInput = inputItems.Where(i => i.ItemType == ingredient.item).FirstOrDefault();
                if (matchingInput != null)
                {
                    maxProduced = Mathf.Min(maxProduced, matchingInput.itemStack.amount / ingredient.amount);
                } else return 0;
            }
            // check max output capacity
            foreach (var output in recipe.OutputItems)
            {
                var matchingOutput = outputItems.Where(o => o.ItemType == output).FirstOrDefault();
                if (matchingOutput != null)
                {
                    if (matchingOutput.IsFull) return 0;
                    if (matchingOutput.overrideMaxStack)
                        maxProduced = Mathf.Min(maxProduced, matchingOutput.overrideMaxStackNum - matchingOutput.itemStack.amount);
                    else
                        maxProduced = Mathf.Min(maxProduced, output.itemData.maxStack - matchingOutput.itemStack.amount);
                } else if (outputItems.Where(o => o.ItemType == null).Count() == 0) return 0;
            }

            return maxProduced;
        }
        private void ConsumeInputsForNProduction(int n)
        {
            foreach (var ingredient in recipe.inputs)
            {
                var matchingInput = inputItems.Where(i => i.ItemType == ingredient.item).FirstOrDefault();
                if (matchingInput != null)
                {
                    matchingInput.Remove(ingredient.amount * n);
                } else throw new System.Exception("Missing input item when consuming");
            }
        }
        private void AddNProduction(int n)
        {
            foreach (var output in recipe.OutputItems)
            {
                var matchingOutput = outputItems.Where(o => o.ItemType == output).FirstOrDefault();
                if (matchingOutput != null)
                {
                    matchingOutput.Add(output, n);
                }
                else
                {
                    var empty = outputItems.Where(o => o.ItemType == null).FirstOrDefault();
                    if (empty != null)
                    {
                        empty.Add(output, n);
                    } else throw new System.Exception("No empty output slots to add production");
                }
            }
        }
        private IEnumerator CreateOutput(float startTime)
        {
            IsWorking = true;
            // FIXME use start startTime so we can pick up where we left off
            float t = 0f; // recipe.secondsToProduce - startTime;
            while (t < recipe.secondsToProduce)
            {
                t += Time.deltaTime;
                RecipeStartTime = -t;
                yield return null;
            }
            var amountToProduce = Mathf.RoundToInt(this.PowerEfficiency * (t / recipe.secondsToProduce));
            if (amountToProduce > 0)
            {
                int maxProduction = Mathf.Min(amountToProduce, CalculateMaxProduction());

                ConsumeInputsForNProduction(maxProduction);
                AddNProduction(maxProduction);

                RecipeStartTime = Time.time;
            }
            _currRoutine = null;

        }
        public override void ProcessLoop()
        {
            if (_currRoutine != null) return;

            if (recipe == null)
            {
                IsWorking = false;
                NoRecipe();
                return;
            }

            if (MissingIngredients())
            {
                IsWorking = false;
                return;
            }

            if (outputItems.Length>0 && outputItems.All(o => o.IsFull))
            {
                IsWorking = false;
                return;
            }
            _currRoutine = StartCoroutine(CreateOutput(RecipeStartTime));
        }
        #endregion

        #region Input and Recipes
        public void ClearInternalStorage()
        {
            inputItems = new LocalStorage[inputItems.Length];
            outputItems = new LocalStorage[outputItems.Length];
        }
        public bool AssignRecipe(Recipe recipe, bool clearStorage =false)
        {
            this.recipe = recipe;
            if (clearStorage)
                ClearInternalStorage();
            return true;
        }

        public void TestCall()
        {
            Debug.Log("Button should have recipe assigned!");
        }

        public bool MatchFirstRecipe(out Recipe found)
        {
            found = null;
            // no matching recipes if we have no production input
            if (inputItems.All(x=>x.ItemType==null)) return false;
            // find recipe based on what inputs are currently available
            Recipe[] matchedRecipes = RecipeFinder.FilterRecipes(
                inputItems.Select(i=>i.ItemType).ToArray(), Outputs.Length, validRecipes, invalidRecipes);
            if (matchedRecipes.Count() == 0) return false;
            found = matchedRecipes[0];
            return true;
        }
        #endregion

        #region Working-State Helpers
        protected bool NoRecipe()
        {
            if (recipe == null)
            {
                // use first recipe that is valid and can use available inputs
                if (MatchFirstRecipe(out Recipe found))
                {
                    AssignRecipe(found);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
#endregion

        #region Input_Output_Helpers
        
        #endregion  

        #region SERIALIZATION_HELPERS
        private List<SerializedItemStack> SerializeField(LocalStorage[] localStorage)
        {
            return localStorage.Select(x => new SerializedItemStack { itemResourcePath = x.ItemType.resourcesPath, amount=x.itemStack.amount }).ToList();
        }
        public SerializedItemStack[] SerializeInputs() => SerializeField(inputItems).ToArray();
        public SerializedItemStack[] SerializeOutputs() => SerializeField(outputItems).ToArray();

        private LocalStorage[] DeserializeField(SerializedItemStack[] items)
        {
            return items.Select(x => new LocalStorage { 
                itemStack = new ItemStack
                {
                    item = Resources.Load<Item>(x.itemResourcePath),
                    amount = x.amount
                }}).ToArray();
        }
        public void DeserializeInputs(SerializedItemStack[] inputs) => inputItems = DeserializeField(inputs);
        public void DeserializeOutputs(SerializedItemStack[] outputs) => outputItems = DeserializeField(outputs);
        #endregion

    }
}