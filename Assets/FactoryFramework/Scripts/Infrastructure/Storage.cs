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
            storage = new ItemStack[capacity];
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
    }
}