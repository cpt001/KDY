using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryFramework;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This script 
/// -snaps the player's building placement to the grid, and creates the ghost object the player will place. 
/// -handles existing building interactions for recipe setting, and deletion
/// 
/// Grid Conveyor 
/// - Need to implement height via pgup/dn
/// - Need to implement deletion detection
/// </summary>

public class GridCheck : MonoBehaviour
{
    Camera mainCam => Camera.main;
    [SerializeField] private LayerMask gridLayerMask;
    [SerializeField] private LayerMask machineLayerMask;
    public GameObject buildingGhost = null;
    //public Renderer buildingGhostRenderer;
    public ColliderCheck buildingColliderCheck;
    public Vector3 gridPoint;
    public MachineryPooling targetPullPool;
    [SerializeField] private KeyCode rotBldgRight = KeyCode.Comma;
    [SerializeField] private KeyCode rotBldgLeft = KeyCode.Period;
    private bool bulldozerMode = false;
    [SerializeField] private GameObject targetedMachine;

    [SerializeField] private GameObject RecipeSelectionPanel;
    [SerializeField] private GameObject RecipeOrganizer;
    [SerializeField] private List<RecipeInterface> recipeButtons = new List<RecipeInterface>();
    //[SerializeField] private List<Recipe> recipeList = new List<Recipe>();  //Local recipe list needed for GUI

    private void Awake()
    {
        foreach (Transform targetButton in RecipeOrganizer.transform)
        {
            recipeButtons.Add(targetButton.GetComponent<RecipeInterface>());
        }
    }

    private void Update()
    {
        MouseToWorldGrid();
        HandleDeconstruction();
        if (!buildingGhost)
        {
            return;
        }
        else
        {
            //Debug.Log("Building being placed: " + buildingGhost);
            if (!buildingGhost.activeInHierarchy)
            {
                buildingGhost.SetActive(true);
            }
            //MouseToWorldGrid();
            //PlacementValidity();
            HandleRotation();
            HandleConstruction();
        }
    }

    void MouseToWorldGrid()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, gridLayerMask))
        {
            //Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, rayHit.distance);
            Vector3 worldPoint = rayHit.point;
            gridPoint = SnapToGrid(worldPoint, 1f);

            if (buildingGhost != null)
            {
                buildingGhost.transform.position = gridPoint;
            }
        }
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, machineLayerMask) && !bulldozerMode)
        {
            Debug.Log("Detecting machinery");
            targetedMachine = rayHit.transform.gameObject;
            if (Input.GetMouseButtonDown(0) && targetedMachine.GetComponent<Processor>())
            {
                HandleRecipeSetup(targetedMachine.GetComponent<Processor>());
            }
        }
    }
    static Vector3 SnapToGrid(Vector3 pos, float gridUnitSize)
    {
        Vector3 snapPos = Snapping.Snap(pos, Vector3.one * gridUnitSize, SnapAxis.All);
        snapPos.y = 0;
        return snapPos;
    }
    void HandleRotation()
    {
        if (Input.GetKeyDown(rotBldgRight) || (Input.GetKey(KeyCode.LeftShift) && Input.mouseScrollDelta.y < 0))
        {
            buildingGhost.transform.Rotate(new Vector3(0, -45, 0));
        }
        if (Input.GetKeyDown(rotBldgLeft) || (Input.GetKey(KeyCode.LeftShift) && Input.mouseScrollDelta.y > 0))
        {
            buildingGhost.transform.Rotate(new Vector3(0, 45, 0));
        }
    }

    void HandleConstruction()
    {
        //Construct Building
        if (Input.GetMouseButtonDown(0) && targetPullPool != null && buildingColliderCheck.placementValid)
        {
            GameObject placedStructure = targetPullPool.GetPooledStructures();
            if (placedStructure != null)
            {
                placedStructure.transform.position = buildingGhost.transform.position;
                placedStructure.transform.rotation = buildingGhost.transform.rotation;
                placedStructure.SetActive(true);
            }
        }
        //Cancel Construction
        if (Input.GetMouseButtonDown(1))
        {
            buildingGhost.SetActive(false);
            buildingGhost = null;
            targetPullPool = null;
        }
    }

    public void BulldozerButtonToggle()
    {
        bulldozerMode = bulldozerMode ? false : true;
    }
    void HandleDeconstruction()
    {
        /*if (Input.GetKey(toggleBulldozerMode))
        {
            bulldozerMode = bulldozerMode ? false : true;
        }*/
        if (bulldozerMode == true)
        {
            Debug.Log("Bulldozer active");
            GameObject targetMachine = null;
            //raycast for targets
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, machineLayerMask)) 
            {
                Debug.Log("Targeting... " + rayHit.transform.gameObject);
                targetMachine = rayHit.transform.gameObject;
                targetMachine.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
            else if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, ~machineLayerMask)) 
            {
                if (targetMachine)
                {
                    //Resetting the color doesn't quite work. 
                    targetMachine.GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
                    targetMachine = null;
                }
            }

            if (Input.GetMouseButtonDown(0) && targetMachine)
            {
                targetMachine.SetActive(false);
            }
        }
    }

    void HandleRecipeSetup(Processor targetProcessor)
    {
        //Clear previous buttons
        foreach (RecipeInterface rTracker in recipeButtons)
        {
            rTracker.thisButtonRecipe = null;
        }
        //Set new data from target machine to each button
        for (int i = 0; i < recipeButtons.Count; i++)
        {
            if (i < targetProcessor.validRecipes.Length)
            {
                //Debug.Log("Assigning recipes to button " + recipeButtons[i].gameObject + " || Current i = " + i);
                recipeButtons[i].processorTarget = targetProcessor;
                recipeButtons[i].thisButtonRecipe = targetProcessor.validRecipes[i];
                recipeButtons[i].buttonText.text = targetProcessor.validRecipes[i].ToString();
                recipeButtons[i].gameObject.SetActive(true);
            }
            else
            {
                recipeButtons[i].gameObject.SetActive(false);
            }
        }
        RecipeSelectionPanel.SetActive(true);
    }
}