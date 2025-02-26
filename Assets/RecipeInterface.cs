using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FactoryFramework;

/// <summary>
/// Pre-Referenced by button, simply handles new recipe assignment to machine
/// </summary>

public class RecipeInterface : MonoBehaviour
{
    public TextMeshProUGUI buttonText => GetComponentInChildren<TextMeshProUGUI>();
    public Processor processorTarget;
    public Recipe thisButtonRecipe;
    
    public void SetRecipeToMachine()
    {
        processorTarget.AssignRecipe(thisButtonRecipe);
    }
}
