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

            //root.Add(new Label("Input Local Storage (readonly)"));
            //foreach (var pair in p.SerializeInputs())
            //{
            //    root.Add(new Label($"{pair.itemResourcePath} {pair.amount}"));
            //}
            //root.Add(new Label("Output Local Storage (readonly)"));
            //foreach (var pair in p.SerializeOutputs())
            //{
            //    root.Add(new Label($"{pair.itemResourcePath} {pair.amount}"));
            //}

            //root.Add(new Label($"Can start Production {p.CanStartProductionTest}"));
            //root.Add(new Label($"is working {p.IsWorking}"));
            ////root.Add(new Label($"Efficiency {p.Eff}"));

            EditorUtility.SetDirty(this);

            // Return the finished inspector UI
            return root;
        } 
    }
}