using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script snaps the player's building placement to the grid, and creates the ghost object the player will place. 
/// 
/// Grid Conveyor 
/// - Need to implement height via pgup/dn
/// - Need to implement continued placement
/// </summary>

public class GridCheck : MonoBehaviour
{
    Camera mainCam => Camera.main;
    [SerializeField] private LayerMask layerMask;
    public GameObject buildingGhost = null;
    //public Renderer buildingGhostRenderer;
    private ColliderCheck buildingColliderCheck => buildingGhost.GetComponent<ColliderCheck>();
    public Vector3 gridPoint;
    
    private void Update()
    {
        if (!buildingGhost)
        {
            return;
        }
        else
        {
            MouseToWorldGrid();
            //PlacementValidity();
            HandleRotation();
        }
    }

    void MouseToWorldGrid()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, layerMask))
        {
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

    /*void PlacementValidity()
    {
        if (!buildingColliderCheck.placementValid)
        {
            buildingGhostRenderer.material.SetColor("_Color", Color.red);
        }
        else
        {
            buildingGhostRenderer.material.SetColor("_Color", Color.blue);

            if (Input.GetMouseButtonDown(0))
            {
                throw new System.NotImplementedException("Building placed, NYI!");
            }
        }
    }*/
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
}
