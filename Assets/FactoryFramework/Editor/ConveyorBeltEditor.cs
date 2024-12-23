using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FactoryFramework;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(ConveyorBelt))]
public class ConveyorBeltEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        ConveyorBelt self = (ConveyorBelt)target;
        var root = new VisualElement();

        // Create default inspector
        var defaultInspector = new VisualElement();
        InspectorElement.FillDefaultInspector(defaultInspector, serializedObject, this);
        root.Add(defaultInspector);

        //hidden speed field that just listens for changes
        var speedField = new PropertyField(serializedObject.FindProperty("speed"));
        speedField.RegisterValueChangeCallback(evt =>
        {
            var speed = evt.changedProperty.floatValue;
            self.SetSpeed(speed);
            serializedObject.ApplyModifiedProperties();
        });
        speedField.style.display = DisplayStyle.None;
        root.Add(speedField);
        // Add a button at the bottom
        var button = DiscordLink.CreateDiscordButton();
        root.Add(button);
        return root;
    }

}
