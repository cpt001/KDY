using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using FactoryFramework;

namespace FactoryFramework.Editor
{
    [CustomEditor(typeof(Processor))]
    public class ProcessorEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            Processor p = (Processor)target;

            // Create a new VisualElement to be the root of our inspector UI
            VisualElement root = new VisualElement();

            // Attach a default inspector to the foldout
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            root.Add(DiscordLink.CreateDiscordButton());

            EditorUtility.SetDirty(this);

            // Return the finished inspector UI
            return root;
        } 
    }
}