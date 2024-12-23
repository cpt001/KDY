using FactoryFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Storage))]
public class StorageInspector : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        // Create a new VisualElement to be the root of our inspector UI
        VisualElement root = new VisualElement();

        // Attach a default inspector to the foldout
        InspectorElement.FillDefaultInspector(root, serializedObject, this);

        root.Add(DiscordLink.CreateDiscordButton());

        // Return the finished inspector UI
        return root;
    }

}
