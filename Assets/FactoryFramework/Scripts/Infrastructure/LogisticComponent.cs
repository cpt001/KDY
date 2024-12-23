using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using System.Linq;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FactoryFramework
{
    /// <summary>
    /// Base class used for all things that create/process/destroy/transfer materials
    /// </summary>
    /// 
    [RequireComponent(typeof(SerializationReference))]
    public abstract class LogisticComponent : MonoBehaviour
    {
        #region Serialization
        protected GlobalLogisticsSettings settings { get { return ConveyorLogisticsUtils.settings; } }
        protected SerializationReference _sRef;
        public System.Guid GUID { get {
                if (_sRef == null) _sRef ??= GetComponent<SerializationReference>();
                return _sRef.GUID; } } // set { _sRef.GUID = value; }
        public string ResourcesPath { get {
                if (_sRef == null) _sRef ??= GetComponent<SerializationReference>();
                return _sRef.resourcesPath; } }
        #endregion

        protected PowerGridComponent _powerGridComponent;
        public float PowerEfficiency 
        { get
            {
                if (this._powerGridComponent?.basePowerDraw > 0)
                {
                    return (_powerGridComponent?.grid?.Efficiency) ?? 0f;

                } else
                    return 1f;
            }
            private set { }
        }
        public bool IsWorking { get; protected set; }

        public LogisticComponent[] Inputs;
        public LogisticComponent[] Outputs;

        // show if not connected!
        public GameObject[] InputHookVisuals;
        public GameObject[] OutputHookVisuals;

        public Transform[] InputHooks;
        public Transform[] OutputHooks;

        #region Transfer Items
        //public LocalStorage[] internalStorage;
        public virtual Item OutputItem { get; }
        public virtual bool TransferItem(LogisticComponent output) { return false; }
        public virtual bool RecieveItem(Item item) { return false; }
        public virtual bool CanRecieveItem(Item item) { return false; }
        #endregion

        private void Awake()
        {
            _powerGridComponent ??= GetComponent<PowerGridComponent>();
            _sRef ??= GetComponent<SerializationReference>();
        }

        private void OnValidate()
        {
            _powerGridComponent ??= GetComponent<PowerGridComponent>();
            _sRef ??= GetComponent<SerializationReference>();
        }

        private void Update()
        {
            if (Inputs==null || Outputs == null) return;
            if (InputHooks != null && InputHookVisuals!=null)
            {
                for (int i = 0; i < InputHooks.Length; i++)
                {
                    InputHookVisuals[i]?.SetActive(Inputs[i] == null);
                }
            }
            if (OutputHooks != null && OutputHookVisuals!=null)
            {
                for (int i = 0; i < OutputHooks.Length; i++)
                {
                    OutputHookVisuals[i]?.SetActive(Outputs[i] == null);
                }
            }

            ProcessLoop();

            TransferItems();
        }

        public virtual void ProcessLoop() { }

        public virtual void TransferItems() {
            foreach (var output in Outputs)
            {
                if (OutputItem == null) return;
                if (output == null) continue;
                // special handling for merger
                if (output is Merger merger){continue;}
                if (output.CanRecieveItem(OutputItem))
                {
                    Item item = OutputItem;
                    TransferItem(output);
                    output.RecieveItem(item);
                }
            }
        }

        #region Connections
        public bool ConnectInput(LogisticComponent input, int index=-1)
        {
            index = (index==-1) ? Array.IndexOf(Inputs, null) : index;
            if (index == -1) return false;

            Inputs[index] = input;
            return true;
        }
        public bool DisconnectInput(LogisticComponent input)
        {
            var index = Array.IndexOf(Inputs, input);
            if (index == -1) return false;

            Inputs[index] = null;
            return true;
        }
        public bool ConnectOutput(LogisticComponent output, int index = -1)
        {
            index = (index == -1) ? Array.IndexOf(Outputs, null) : index;
            if (index == -1) return false;

            Outputs[index] = output;
            return true;
        }
        public bool DisconnectOutput(LogisticComponent output)
        {
            var index = Array.IndexOf(Outputs, output);
            if (index == -1) return false;

            Outputs[index] = null;
            return true;
        }
        public void DisconnectAll()
        {
            foreach (var output in Outputs)
            {
                if (output != null)
                {
                    output.DisconnectInput(this);
                    this.DisconnectOutput(output);
                }
            }
            foreach (var input in Inputs)
            {
                if (input != null)
                {
                    input.DisconnectOutput(this);
                    this.DisconnectInput(input);
                }
            }
        }
        public int GetNearestOutput(Vector3 worldPos)
        {
            Assert.IsTrue(Outputs.Count() == OutputHooks.Count(), "Error: Outputs and OutputHooks need to be the same length");
            Vector3 localPos = transform.InverseTransformPoint(worldPos);
            float minDist = float.MaxValue;
            int nearestIndex = -1;
            for (int i = 0; i < OutputHooks.Count(); i++)
            {
                var hook = OutputHooks.ElementAt(i);
                var connected = Outputs.ElementAt(i);
                if (connected != null) continue;
                float dist = Vector3.Distance(hook.localPosition, localPos);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestIndex = i;
                }
            }
            return nearestIndex;
        }
        public int GetNearestInput(Vector3 worldPos)
        {
            Assert.IsTrue(Inputs.Count() == InputHooks.Count(), "Error: Inputs and InputHooks need to be the same length");
            Vector3 localPos = transform.InverseTransformPoint(worldPos);
            float minDist = float.MaxValue;
            int nearestIndex = -1;
            for (int i = 0; i < InputHooks.Count(); i++)
            {
                var hook = InputHooks.ElementAt(i);
                var connected = Inputs.ElementAt(i);
                if (connected != null) continue;
                float dist = Vector3.Distance(hook.localPosition, localPos);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestIndex = i;
                }
            }
            return nearestIndex;
        }
        #endregion

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            string working = IsWorking ? "Working" : "Idle";
            working += $" ({PowerEfficiency:P0})";
            var style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 12;
            Handles.Label(transform.position, working,style);
        }
#endif
    }
}