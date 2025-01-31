using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script snaps the player's building placement to the grid, and creates the ghost object the player will place. 
/// 
/// Grid Conveyor 
/// - Need to implement height via pgup/dn
/// </summary>

public class GridCheck : MonoBehaviour
{
    Camera mainCam => Camera.main;
    [SerializeField] private LayerMask layerMask;
    public GameObject buildingGhost = null;
    //public Renderer buildingGhostRenderer;
    public ColliderCheck buildingColliderCheck;
    public Vector3 gridPoint;
    public MachineryPooling targetPullPool;
    
    private void Update()
    {
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
            MouseToWorldGrid();
            //PlacementValidity();
            HandleRotation();
            HandleConstruction();
        }
    }

    void MouseToWorldGrid()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, layerMask))
        {
            Debug.Log("Raycast hitting: " + rayHit.transform.name);
            //Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, rayHit.distance);
            Vector3 worldPoint = rayHit.point;
            gridPoint = SnapToGrid(worldPoint, 1f);

            buildingGhost.transform.position = gridPoint;
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
        if (Input.GetKeyDown(KeyCode.Q) || (Input.GetKey(KeyCode.LeftShift) && Input.mouseScrollDelta.y < 0))
        {
            buildingGhost.transform.Rotate(new Vector3(0, -45, 0));
        }
        if (Input.GetKeyDown(KeyCode.E) || (Input.GetKey(KeyCode.LeftShift) && Input.mouseScrollDelta.y > 0))
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
                buildingColliderCheck.trackedColliders.Add(placedStructure.GetComponent<Collider>());
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
}
