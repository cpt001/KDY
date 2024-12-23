using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    [System.Serializable]
    public class SerializedLogisticComponent
    {
        public string type;
        public Vector3 position;
        public Quaternion rotation;
        public string guid;
        public string resourcesPath;
        public string[] InputReferences;
        public string[] OutputReferences;

        //also all of the data from the subclasses to make it easy
        public string resourceReference;
        public string recipeReference;
        public SerializedItemStack[] internalInput;
        public SerializedItemStack[] internalOutput;
        public SerializedItemOnBelt[] iobs;
        public float timeSinceLastJob;
        public float speed;
        public Vector3[] pathPoints;
        //public string frameMeshReference;
        //public string beltMeshReference;
    }
}
