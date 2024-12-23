using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    public class Producer : LogisticComponent
    {
        #region fields
        [SerializeField] public Item resource;

        public LocalStorage internalStorage;

        [Tooltip("Only allow certain types of items. Leave Blank for Any")]
        public Item[] validItems;

        public float resourcesPerSecond = 1f;
        private float secondsPerResource { get { return 1f / resourcesPerSecond; } }
        public float  SecondsSinceLastResource { get; private set; }
        #endregion

        public void SetResource(Item item)
        {
            if (item == resource) return;
            resource = item;
        }

        #region Lifecycle
        private void OnEnable()
        {
            SecondsSinceLastResource = Time.time;
            IsWorking = false;
        }
        private void OnDisable()
        {
            IsWorking = false;
        }
        #endregion

        #region Workload
        public override void ProcessLoop()
        {
            if (resource == null)
            {
                IsWorking = false;
                return;
            }
            IsWorking = true;
            if (internalStorage.IsFull)
            {
                IsWorking = false;
                return;
            }
            var elapsedTime= Time.time - SecondsSinceLastResource;
            var resourceAmount = Mathf.FloorToInt(this.PowerEfficiency * elapsedTime / secondsPerResource);
            if (resourceAmount > 0)
            {
                SecondsSinceLastResource = Time.time;

                if (internalStorage.ItemType == null || internalStorage.ItemType == resource)
                {
                    internalStorage.Add(resource, resourceAmount);
                }
            }
        }
        
        #endregion

        #region Overrides
        public override bool RecieveItem(Item item)
        {
            return false;
        }
        public override bool CanRecieveItem(Item item)
        {
            return false;
        }
        public override bool TransferItem(LogisticComponent output)
        {
            if (internalStorage.ItemType == null) return false;
            if (output.CanRecieveItem(internalStorage.ItemType))
            {
                Item item = internalStorage.Remove();
                return true;
            }
            
            return false;
        }
        public override Item OutputItem { 
            get
            {
                if (internalStorage.ItemType != null)
                {
                    return internalStorage.ItemType;
                }
                return null;
            }
        }
        #endregion
    }
}