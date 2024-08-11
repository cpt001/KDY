using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FactoryFramework
{
    public class Processor : Building, IInput, IOutput
    {
        [Tooltip("Active Recipe")]
        public Recipe recipe;

        [Tooltip("How many different ingredients go into this machine")]
        public int numInputs;
        [Tooltip("How many different products come out of this machine")]
        public int numOutputs;

        public Recipe[] validRecipes;
        public Recipe[] invalidRecipes;

        private Dictionary<Item, int> _inputs = new Dictionary<Item, int>();
        private Dictionary<Item, int> _outputs = new Dictionary<Item, int>();

        private IEnumerator _currentRoutine;

        #region Lifecycle
        private void OnEnable()
        {
            IsWorking = false;
            recipe = null;
        }
        private void OnDisable()
        {
            CancelWork();
        }
        private void CancelWork()
        {
            if (_currentRoutine != null) StopCoroutine(_currentRoutine);
        }
        #endregion

        #region Workload
        public override void ProcessLoop()
        {
            
            if (CanStartProduction())
            {
                _currentRoutine = Process();
                StartCoroutine(_currentRoutine);
            }
        }

        IEnumerator Process()
        {
            IsWorking = true;
            ConsumeInputIngredients();
            float _t = 0f;
            while (_t < recipe.tickCost)
            {
                //FIXME custom tick?
                _t += Time.deltaTime * this.PowerEfficiency;
                yield return null;
            }
            CreateOutputProducts();
            // do we need to un-assign the current recipe? Check if we can make any more
            bool needsReset = !recipe.InputItems.All(i => _inputs.ContainsKey(i));
            if (needsReset)
            {
                AssignRecipe(null, false);
            }
            _currentRoutine = null;
            IsWorking = false;
        }
        #endregion

        #region Input and Recipes
        public void ClearInternalStorage()
        {
            _inputs = new Dictionary<Item, int>();
        }
        public bool AssignRecipe(Recipe recipe, bool clearStorage =false)
        {
            this.recipe = recipe;
            if (clearStorage)
                ClearInternalStorage();
            return true;
        }
        public bool MatchFirstRecipe(out Recipe found)
        {
            found = null;
            // no matching recipes if we have no production input
            if (_inputs.Keys.Count == 0) return false;
            // find recipe based on what inputs are currently available
            Recipe[] matchedRecipes = RecipeFinder.FilterRecipes(
                _inputs.Keys.ToArray(), numOutputs, validRecipes, invalidRecipes);
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

        protected bool AnyOutputsFull()
        {
            foreach (Item item in recipe.OutputItems)
            {
                _outputs.TryGetValue(item, out int amount);
                if (amount >= item.itemData.maxStack)
                {
                    return true;
                }
            }
            return false;
        }

        protected bool MissingIngredients()
        {
            foreach (ItemStack itemsRequired in recipe.inputs)
            {
                if (_inputs.TryGetValue(itemsRequired.item, out int amount))
                {
                    if (amount < itemsRequired.amount)
                        return true;
                } else
                    return true;
            }
            return false;
        }

        protected bool CanStartProduction()
        {
            // cannot start a new production cycle while one is running
            if (IsWorking) return false;
            // need a recipe to make!
            if (NoRecipe()) return false;
            //check for outputs being full
            if (AnyOutputsFull()) return false;
            // check that we have enough input ingredients. Reset recipe to find another
            if (MissingIngredients()) { recipe = null; return false; }
            return true;
        }
        public bool CanStartProductionTest { get { return CanStartProduction(); } }
#endregion

        #region Input_Output_Helpers
        private void ConsumeInputIngredients()
        {
            for (int i = 0; i < recipe.inputs.Length; i++)
            {
                ItemStack ingredient = recipe.inputs[i];
                _inputs[ingredient.item] -= ingredient.amount;
                // remove key if empty and reset recipe to be re-matched
                if (_inputs[ingredient.item] == 0)
                {
                    _inputs.Remove(ingredient.item);
                }
            }
        }
        private void CreateOutputProducts()
        {
            for (int i = 0; i < recipe.outputs.Length; i++)
            {
                Item item = recipe.OutputItems[i];
                if (_outputs.ContainsKey(item))
                {
                    _outputs[item] = Mathf.Min(_outputs[item] + recipe.outputs[i].amount, item.itemData.maxStack);
                } else
                {
                    _outputs.Add(item, 1);
                }
            }
        }

        public bool CanGiveOutput(Item filter = null)
        {
            if (filter != null)
            {
                _outputs.TryGetValue(filter, out int amount);
                if (amount > 0) return true;
            } else
            {
                foreach (KeyValuePair<Item, int> availableOutput in _outputs)
                {
                    if (availableOutput.Value > 0) return true;
                }

            }
            return false;
        }
        public Item OutputType() {
            foreach (KeyValuePair<Item, int> availableOutput in _outputs)
            {
                if (availableOutput.Value > 0) return availableOutput.Key;
            }
            return null;
        }
        public Item GiveOutput(Item filter = null)
        {
            Item result = null;
            if (filter != null)
            {
                _outputs.TryGetValue(filter, out int amount);
                if (amount > 0)
                {
                    _outputs[filter] -= 1;
                    result = filter;

                    // remove key
                    if (_outputs[filter] == 0)
                        _outputs.Remove(filter);
                }
            }
            else
            {
                foreach (KeyValuePair<Item, int> availableOutput in _outputs.ToList())
                {
                    if (availableOutput.Value > 0)
                    {
                        _outputs[availableOutput.Key] -= 1;
                        result = availableOutput.Key;

                        //remove key
                        if (_outputs[availableOutput.Key] == 0)
                            _outputs.Remove(availableOutput.Key);
                    }
                }

            }
            return result;
        }

        public void TakeInput(Item item)
        {
            if (_inputs.ContainsKey(item))
                _inputs[item] += 1;
            else
                _inputs.Add(item, 1);
        }
        public bool CanTakeInput(Item item)
        {
            if (item == null) return false;
            
            if (_inputs.ContainsKey(item))
            {
                return _inputs[item] < item.itemData.maxStack;
            } else
            {
                return _inputs.Keys.Count < numInputs;
            }
        }
        #endregion  

        #region SERIALIZATION_HELPERS
        private List<SerializedItemStack> SerializeField(Dictionary<Item, int> dict)
        {
            List<SerializedItemStack> items = new List<SerializedItemStack>();
            foreach(KeyValuePair<Item, int> obj in dict)
            {
                items.Add(new SerializedItemStack(){ itemResourcePath = obj.Key.resourcesPath, amount = obj.Value});
            }
            return items;
        }
        public SerializedItemStack[] SerializeInputs() => SerializeField(_inputs).ToArray();
        public SerializedItemStack[] SerializeOutputs() => SerializeField(_outputs).ToArray();

        private Dictionary<Item, int> DeserializeField(List<SerializedItemStack> items)
        {
            Dictionary<Item, int> dict = new Dictionary<Item, int>();
            foreach(var iStack in items)
            {
                dict.Add(Resources.Load<Item>(iStack.itemResourcePath), iStack.amount);
            }
            return dict;
        }
        public void DeserializeInputs(SerializedItemStack[] inputs) => _inputs = DeserializeField(inputs.ToList());
        public void DeserializeOutputs(SerializedItemStack[] inputs) => _outputs = DeserializeField(inputs.ToList());
        #endregion

    }
}