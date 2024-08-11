using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    public class Producer : Building, IOutput
    {
        #region fields
        [Tooltip("Resource being Produce")]
        public LocalStorage resource;

        [Tooltip("Only allow certain types of items. Leave Blank for Any")]
        public Item[] validItems;

        [SerializeField] private float _resourcesPerSecond = 1f;
        private float secondsPerResource { get { return 1f / _resourcesPerSecond; } }

        private IEnumerator _currentRoutine;
        #endregion

        public void SetOutputResource(Item item)
        {
            if (item == resource.itemStack.item) return;
            if (resource.itemStack.amount > 0)
                Debug.LogWarning($"Producer {gameObject.name} is dropping {resource.itemStack.amount} {resource.itemStack.item.name}(s) into the void");
            resource.itemStack.amount = 0;
            resource.itemStack.item = item;
            CancelWork();
        }

        #region Lifecycle
        private void OnEnable()
        {
            // use mesh to calculate bounds
            Mesh m = GetComponent<MeshFilter>()?.mesh;
            Vector3 center = (m != null) ? m.bounds.center : transform.position;
            Vector3 size = (m != null) ? m.bounds.extents : Vector3.one;
            foreach (Collider c in Physics.OverlapBox(transform.TransformPoint(center), size*1.125f))
            {
                if (c.TryGetComponent(out Resource r)){
                    if (validItems.Count() == 0 || validItems.Contains(r.item))
                    {
                        this.SetOutputResource(r.item);
                    }
                    else
                    {
                        Debug.LogWarning($"Resource {r.item.name} is not a valid resource for this building to mine");
                    }
                    
                    break;
                }
            }
            IsWorking = false;
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
        private bool CanStartWork()
        {
            if (IsWorking) return false;
            if (resource.itemStack.item == null) return false;
            // is full?
            if (resource.itemStack.amount == resource.itemStack.item.itemData.maxStack) return false;
            return true;
        }
        public override void ProcessLoop()
        {
            if (CanStartWork())
            {
                _currentRoutine = ProduceResource();
                StartCoroutine(_currentRoutine);
            }
        }
        private IEnumerator ProduceResource()
        {
            IsWorking = true;
            float _t = 0f;
            while (_t < secondsPerResource)
            {
                _t += Time.deltaTime * PowerEfficiency;
                yield return null;
            }
            resource.itemStack.amount += 1;
            _currentRoutine = null;
            IsWorking = false;
        }
        #endregion

        #region IOutput
        public bool CanGiveOutput(Item filter = null)
        {
            if (filter != null) Debug.LogWarning("Producer Does not Implement Item Filter Output");
            return resource.itemStack.item != null && resource.itemStack.amount > 0;
        }
        public Item OutputType() { return resource.itemStack.item; }
        public Item GiveOutput(Item filter = null)
        {
            if (filter != null) Debug.LogWarning("Producer Does not Implement Item Filter Output");
            if (resource.itemStack.item == null || resource.itemStack.amount == 0) return null;
            resource.itemStack.amount -= 1;
            return resource.itemStack.item;
        }
        #endregion

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(Vector3.zero, 1f);
        }
    }
}