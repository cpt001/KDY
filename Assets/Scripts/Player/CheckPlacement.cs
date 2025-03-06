using UnityEngine;

//This script handles grid snap, and collider checks against existing objects
public class CheckPlacement : MonoBehaviour
{
    [SerializeField] private int gridSpacing;
    [SerializeField] private LayerMask layerMask;
    public GameObject objectBeingPlaced;

    public GameObject currentBuildingPrefab;
    public enum BuildingType
    {
        Producer,
        Assembler,
        Consumer,
        Conveyor,
    }
    public BuildingType buildingType;

    // Update is called once per frame
    void Update()
    {
        //GetMousePosition();
        MouseToGridPosition();
    }


    void MouseToGridPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, layerMask))
        {
            //Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, rayHit.distance);
            Vector3 worldPoint = rayHit.point;
            Vector3 gridPoint = SnapToGrid(worldPoint, 1f);

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.white, 0);
            Debug.DrawLine(Camera.main.transform.position, rayHit.point, Color.blue, 0);
            Debug.DrawLine(Camera.main.transform.position, worldPoint, Color.cyan, 0);
            Debug.DrawLine(Camera.main.transform.position, gridPoint, Color.magenta, 0);

            Debug.Log($"{worldPoint} | {gridPoint} | {rayHit.point}");

            objectBeingPlaced.transform.position = gridPoint;
        }
    }



    void GetMousePosition()
    {
        //Get mouse pos
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, layerMask))
        {
            Debug.DrawLine(Camera.main.transform.position, rayHit.point,  Color.red, 30.0f);
            //set object position to mouse pos
            Vector3 tempV3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, rayHit.distance);
            tempV3 = Camera.main.ScreenToWorldPoint(tempV3);
            tempV3 = new Vector3(tempV3.x, rayHit.point.y, tempV3.z);

            //round object position to nearest xz coord
            if (rayHit.transform.CompareTag("Space"))
            {
                objectBeingPlaced.GetComponent<Renderer>().material.color = Color.blue;
                objectBeingPlaced.transform.position = SnapToGrid(tempV3, gridSpacing);
                ClickBehavior(rayHit.transform);
            }
            else if (rayHit.transform.CompareTag("Tile"))
            {
                objectBeingPlaced.GetComponent<Renderer>().material.color = Color.green;
                ClickBehavior(rayHit.transform);
            }
            else if (rayHit.transform.CompareTag("Producer"))
            {
                objectBeingPlaced.GetComponent<Renderer>().material.color = Color.yellow;
                ClickBehavior(rayHit.transform);
            }
            else if (rayHit.transform.CompareTag("Assembler"))
            {
                objectBeingPlaced.GetComponent<Renderer>().material.color = Color.yellow;
                ClickBehavior(rayHit.transform);
            }
            else if (rayHit.transform.CompareTag("Consumer"))
            {
                objectBeingPlaced.GetComponent<Renderer>().material.color = Color.yellow;
                ClickBehavior(rayHit.transform);
            }
            else if (rayHit.transform.CompareTag("Conveyor"))
            {
                objectBeingPlaced.GetComponent<Renderer>().material.color = Color.yellow;
                Debug.Log("Conveyor spotted");
                ClickBehavior(rayHit.transform);
            }

        }
    }

    public void ClickBehavior(Transform target)
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Create
            if (target.CompareTag("Space"))
            {
                //Check funding
                //Create tile
                
            }
            else if (target.CompareTag("Tile"))
            {

            }
            else if (target.CompareTag("Producer"))
            {

            }
            else if (target.CompareTag("Assembler"))
            {

            }
            else if (target.CompareTag("Consumer"))
            {

            }
            else if (target.CompareTag("Conveyor"))
            {

            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            //Destroy
            if (target.CompareTag("Space"))
            {
                //Do nothing
            }
            else if (target.CompareTag("Tile"))
            {

            }
            else if (target.CompareTag("Producer"))
            {

            }
            else if (target.CompareTag("Assembler"))
            {

            }
            else if (target.CompareTag("Consumer"))
            {

            }
            else if (target.CompareTag("Conveyor"))
            {

            }
        }
    }


    public void SwapBuildableItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            buildingType = BuildingType.Conveyor;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            buildingType = BuildingType.Producer;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            buildingType = BuildingType.Assembler;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            buildingType = BuildingType.Consumer;
        }
    }

    /*public static Vector3 SnaptoGrid(Vector3 pos, int v)
    {
        float x = pos.x;
        float y = pos.y;
        float z = pos.z;


        x = Mathf.FloorToInt(x / v) * v;
        y = 0;
        z = Mathf.FloorToInt(z / v) * v;

        return new Vector3(x, y, z);
    }*/

    public static Vector3 SnapToGrid(Vector3 pos, float gridUnitSize)
    {
        /* return new Vector3(
           Mathf.FloorToInt(pos.x / gridUnitSize) * gridUnitSize,
           0,
           Mathf.FloorToInt(pos.z / gridUnitSize) * gridUnitSize);
        */

        Vector3 snapPos = Snapping.Snap(pos, Vector3.one * gridUnitSize, SnapAxis.All);
        snapPos.y = 0f;
        return snapPos;
    }
}
