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
    [SerializeField] private GameObject DockDebugSelectionPanel;
    [SerializeField] private GameObject RecipeOrganizer;
    [SerializeField] private List<RecipeInterface> recipeButtons = new List<RecipeInterface>();
    [SerializeField] private List<DockDebugInterface> dockDebugButtons = new List<DockDebugInterface>();
    [SerializeField] private Texture2D bulldozerCursor;
    [SerializeField] private GameObject previousTargetMachine = null;
    private enum PlayerAction { Examine, Bulldoze, ChangeRecipe, BuildWall, BuildRoom, BuildMachine };
    [SerializeField] private GameObject wallPrefab;
    private GameObject heldWall;
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

        if (heldWall)
        {
            Debug.Log("wall held condition");
            UpdateWallPositions(heldWall.GetComponent<Wall>(), heldWall.GetComponent<ColliderCheck>());
        }
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
            HandleMachinePlacement();
        }
        //yuno work?

    }

    void MouseToWorldGrid()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;

        //For placing anything on the grid
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

        //For examination and machine setup
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, machineLayerMask) && !bulldozerMode)
        {
            //Debug.Log("Detecting machinery");
            targetedMachine = rayHit.transform.gameObject;
            if (!RecipeSelectionPanel.activeInHierarchy)
            {
                HandleMachineColors(targetedMachine, PlayerAction.Examine);

                if (Input.GetMouseButtonDown(0) && targetedMachine.GetComponent<Processor>() && targetedMachine.name != "SmallFreighterDock(Clone)")
                {
                    HandleMachineColors(targetedMachine, PlayerAction.ChangeRecipe);    //This isnt working correctly
                    HandleRecipeSetup(targetedMachine.GetComponent<Processor>());
                }
                else if (Input.GetMouseButtonDown(0) && targetedMachine.name == "SmallFreighterDock(Clone)")
                {
                    HandleMachineColors(targetedMachine, PlayerAction.ChangeRecipe);
                    HandleDockDebug(targetedMachine.GetComponent<Storage>());
                }
            }

        }
    }
    //All hail the grid
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
    void HandleMachinePlacement()
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
        if (bulldozerMode == true)
        {
            //Debug.Log("Bulldozer active");
            //Cursor.SetCursor(bulldozerCursor, Vector2.zero, CursorMode.Auto);
            GameObject bulldozerTarget = null;

            //raycast for targets
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, machineLayerMask)) 
            {
                //Debug.Log("Targeting... " + rayHit.transform.gameObject);
                bulldozerTarget = rayHit.transform.gameObject;
                HandleMachineColors(bulldozerTarget, PlayerAction.Bulldoze);
            }

            if (Input.GetMouseButtonDown(0) && bulldozerTarget)
            {
                //NYI, examine right click delete for answers
                if (bulldozerTarget.transform.TryGetComponent(out LogisticComponent lc))
                {
                    lc.DisconnectAll();
                }
                bulldozerTarget.SetActive(false);
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
                recipeButtons[i].buttonText.text = targetProcessor.validRecipes[i].name;
                recipeButtons[i].gameObject.SetActive(true);
            }
            else
            {
                recipeButtons[i].gameObject.SetActive(false);
            }
        }
        RecipeSelectionPanel.transform.Find("Machine Name").GetComponentInChildren<TextMeshProUGUI>().text = targetProcessor.name;
        RecipeSelectionPanel.SetActive(true);
    }

    void HandleDockDebug(Storage targetStorage)
    {
        //Debug.Log("Entered debug menu!");
        DockDebugSelectionPanel.SetActive(true);
        foreach (DockDebugInterface ddi in dockDebugButtons)
        {
            ddi.targetStorage = targetStorage;
        }
    }

    void HandleMachineColors(GameObject targetMachine, PlayerAction playerAction)
    {
        if (previousTargetMachine == null)
        {
            previousTargetMachine = targetMachine;
        }
        else if (previousTargetMachine != null && previousTargetMachine != targetMachine)
        {
            previousTargetMachine.GetComponent<SelectionStateMachine>().SetColor(SelectionStateMachine.SelectionState.NotSelected);
            previousTargetMachine = targetMachine;
        }

        if (targetMachine.name != "Conveyor(Clone)")
        {
            switch (playerAction)
            {
                case PlayerAction.Examine:
                    {
                        targetMachine.GetComponent<SelectionStateMachine>().SetColor(SelectionStateMachine.SelectionState.HoverSelect);
                        break;
                    }
                case PlayerAction.Bulldoze:
                    {
                        targetMachine.GetComponent<SelectionStateMachine>().SetColor(SelectionStateMachine.SelectionState.DozerSelect);
                        break;
                    }
                case PlayerAction.ChangeRecipe:
                    {
                        targetMachine.GetComponent<SelectionStateMachine>().SetColor(SelectionStateMachine.SelectionState.RecipeSelect);
                        break;
                    }
            }
            //targetMachine.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

        }
        else
        {
            targetMachine.GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.red);
        }
    }
    public void ConfirmRecipe()
    {
        if (previousTargetMachine)
        {
            previousTargetMachine.GetComponent<SelectionStateMachine>().SetColor(SelectionStateMachine.SelectionState.NotSelected);
            RecipeSelectionPanel.SetActive(false);
        }
    }
    public void ConfirmDockDebug()
    {
        previousTargetMachine.GetComponent<SelectionStateMachine>().SetColor(SelectionStateMachine.SelectionState.NotSelected);
        DockDebugSelectionPanel.SetActive(false);
    }

    //Button spawns new wall
    public void CreateNewWall()
    {
        if (!heldWall)
        {
            Debug.Log("Wall made");
            heldWall = Instantiate(wallPrefab);
        }
    }
    //Updates wall information
    void UpdateWallPositions(Wall currentWall, ColliderCheck wallCollider)
    {
        Debug.Log("Updating wall");
        //No information set
        if (currentWall.StartPoint == Vector3.zero && currentWall.EndPoint == Vector3.zero)
        {
            heldWall.transform.position = gridPoint;
        }
        //First click sets start point
        if (currentWall.StartPoint == Vector3.zero && wallCollider.placementValid && Input.GetMouseButtonDown(0))
        {
            currentWall.StartPoint = gridPoint;
            return;
        }
        //NYI: Set start point with wall collision

        //Handles dragging of new walls
        if (currentWall.StartPoint != Vector3.zero && currentWall.EndPoint == Vector3.zero)
        {
            Vector3 initialScale = heldWall.transform.localScale;

            float distance = Vector3.Distance(currentWall.StartPoint, gridPoint);
            heldWall.transform.localScale = new Vector3(distance, initialScale.y, initialScale.z);
           
            Vector3 midPoint = (currentWall.StartPoint + gridPoint) / 2;
            heldWall.transform.position = midPoint;

            Vector3 rotation = gridPoint - currentWall.StartPoint;
            heldWall.transform.right = rotation;
        }
        //Click sets end point
        if (currentWall.StartPoint != Vector3.zero && currentWall.EndPoint == Vector3.zero && Input.GetMouseButtonDown(0))
        {
            currentWall.EndPoint = gridPoint;
            heldWall = null;
        }
        //NYI: Set end point with wall collision
        //Right click destroys wall
    }
    public void ConvertToRoom()
    {

    }
}