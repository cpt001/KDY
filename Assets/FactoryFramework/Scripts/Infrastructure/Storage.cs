using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    public class Storage : LogisticComponent
    {
        [Min(1)]
        public int capacity;
        public ItemStack[] storage;

        private void Awake()
        {
            if (storage == null)
            {
                storage = new ItemStack[capacity];
            }
        }

        public override bool CanRecieveItem(Item item)
        {
            if (item == null) return false;
            foreach (ItemStack stack in storage)
            {
                if (stack.item == item && stack.amount < item.itemData.maxStack) return true;
                if (stack.item == null || stack.amount == 0) return true;
            }
            return false;
        }
        public override bool RecieveItem(Item item)
        {
            for (int s = 0; s < storage.Length; s++)
            {
                ItemStack stack = storage[s];
                if (stack.item == item && stack.amount < item.itemData.maxStack)
                {
                    stack.amount += 1;
                    storage[s] = stack;
                    return true;
                }
                if (stack.item == null || stack.amount == 0)
                {
                    stack.item = item;
                    stack.amount = 1;
                    storage[s] = stack;
                    return true;
                }
            }
            return false;
        }
        //This will need to be modified later to perform random output from any itemstack from the dock
        public override Item OutputItem {
            get{
                foreach (ItemStack stack in storage)
                {
                    if (stack.item != null && stack.amount > 0) return stack.item;
                }
                return null;
            } 
        }
        public override bool TransferItem(LogisticComponent output)
        {
            for (int s = 0; s < storage.Length; s++)
            {
                ItemStack stack = storage[s];
                if (stack.item == null) continue;
                if ( stack.amount > 0)
                {
                    stack.Remove(1);
                    storage[s] = stack;
                    return true;
                }
            }
            return false;
        }

        #region Exposed Helpers
        public Dictionary<Item, int> AllItemCounts()
        {
            Dictionary<Item, int> items = new Dictionary<Item, int>();
            foreach (ItemStack stack in storage)
            {
                if (stack.item != null && stack.amount > 0)
                {
                    if (items.ContainsKey(stack.item))
                    {
                        items[stack.item] += stack.amount;
                    }
                    else
                    {
                        items.Add(stack.item, stack.amount);
                    }
                }
            }
            return items;
        }
        public int ItemTotal(Item item)
        {
            int total = 0;
            foreach (ItemStack stack in storage)
            {
                if (stack.item == item) total += stack.amount;
            }
            return total;
        }
        public bool ContainsItem(Item item, int amount=1)
        {
            return ItemTotal(item) >= amount;
        }
        public int RemoveItem(Item item, int amount = 1)
        {
            int originalAmount = amount;
            for (int s = 0; s < storage.Length; s++)
            {
                ItemStack stack = storage[s];
                if (stack.item == item)
                {
                    if (stack.amount < amount)
                    {
                        amount -= stack.amount;
                        stack.Remove(stack.amount);
                        
                    } else                     {
                        stack.Remove(amount);
                        storage[s] = stack;
                        return originalAmount;
                    }
                    storage[s] = stack;
                }
            }
            Debug.LogWarning($"Not enough of item {item} in storage. Removed partial {originalAmount - amount} items of requested {originalAmount}");
            return originalAmount-amount;
        }
        public int AddItem(Item item, int amount = 1)
        {
            int originalAmount = amount;
            //Debug.Log("Passed item: " + item + " w/amt: " + originalAmount + " to: " + gameObject);     //It has the info
            for (int s = 0; s < storage.Length; s++)
            {
                ItemStack stack = storage[s];
                //Debug.Log("Target stack: " + stack);    //It knows where to put it
                if (stack.item == item) //Its looking for an already existing item... thats overwritten when the storage initializes
                {
                    //Debug.Log("Found existing item");
                    if (stack.amount + amount > item.itemData.maxStack)
                    {
                        amount -= item.itemData.maxStack - stack.amount;
                        stack.amount = item.itemData.maxStack;
                    } else
                    {
                        stack.amount += amount;
                        storage[s] = stack;
                        return originalAmount;
                    }
                    storage[s] = stack;
                }
                else
                {
                    //Need to build this if condition -- Built :D
                    //Debug.Log("Missing existing item, creating new...");
                    if (storage[s].item == null)
                    {
                        storage[s].item = item;
                        storage[s].amount = amount;
                    }
                    else if (storage[s].item == item)
                    {
                        storage[s].amount = amount;
                    }
                }
            }
            Debug.LogWarning($"Not enough space for item {item} in storage. Added partial {originalAmount - amount} items of requested {originalAmount}");
            return originalAmount-amount;
        }
        #endregion
    }
}