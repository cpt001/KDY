using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    [System.Serializable]
    public class LocalStorage
    {
        public ItemStack itemStack;
        public bool overrideMaxStack = false;
        [Min(1)]
        public int overrideMaxStackNum = 1;

        public bool IsFull
        {
            get
            {
                if (itemStack.item == null) return false;
                if (overrideMaxStack) return itemStack.amount >= overrideMaxStackNum;
                return itemStack.amount >= itemStack.item.itemData.maxStack;
            }
        }
        public Item ItemType => (itemStack.item == null) ? null : itemStack.item;

        public void Add(Item item, int amount = 1)
        {
            if (item == null) return;
            if (itemStack.item == null)
            {
                itemStack.item = item;
                itemStack.amount = amount;
            }
            else if (itemStack.item == item)
            {
                itemStack.amount += amount;
            }
            else
            {
                throw new System.Exception("Not enough room to take a new type of item");
            }
            itemStack.amount = (overrideMaxStack) ? Mathf.Min(itemStack.amount, overrideMaxStackNum) : Mathf.Min(itemStack.amount, item.itemData.maxStack);
        }
        public Item Remove(int amount=1)
        {
            if (amount > itemStack.amount) Debug.LogError($"Cannot give {amount} items. LocalStorage contains {itemStack.amount} items");
            itemStack.amount-=amount;
            if (itemStack.amount <= 0)
            {
                var item = itemStack.item;
                itemStack.item = null;
                return item;
            }
            return itemStack.item;
        }
    }
}