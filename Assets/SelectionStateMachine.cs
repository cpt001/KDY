using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionStateMachine : MonoBehaviour
{
    private Renderer rend => GetComponent<Renderer>();
    public enum SelectionState
    {
        NotSelected,
        HoverSelect,
        DozerSelect,
        RecipeSelect,
    }

    [SerializeField] private MeshRenderer gameObjectRenderer => GetComponent<MeshRenderer>();
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material hoverMaterial;    //This probably isn't needed
    [SerializeField] private Material dozerMaterial;
    [SerializeField] private Material recipeMaterial;

    private void OnEnable()
    {
        //Debug.Log("Enabled");
        SetColor(SelectionState.NotSelected);
    }

    public void SetColor(SelectionState selState = SelectionState.NotSelected)
    {
        if (!gameObject.CompareTag("ConveyorBelt"))
        {
            switch (selState)
            {
                case SelectionState.NotSelected:
                    {
                        gameObjectRenderer.material = defaultMaterial;
                        break;
                    }
                case SelectionState.HoverSelect:
                    {
                        gameObjectRenderer.material = hoverMaterial;
                        break;
                    }
                case SelectionState.DozerSelect:
                    {
                        gameObjectRenderer.material = dozerMaterial;
                        break;
                    }
                case SelectionState.RecipeSelect:
                    {
                        gameObjectRenderer.material = recipeMaterial;
                        break;
                    }
            }
        }
        else
        {
            switch (selState)
            {
                case SelectionState.DozerSelect:
                {
                    foreach (Transform child in transform)
                    {
                        if (child.GetComponent<Renderer>())
                        {
                            child.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                        }
                    }
                    break;
                }
            }

        }

    }
}
