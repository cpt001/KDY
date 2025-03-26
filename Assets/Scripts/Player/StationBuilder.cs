using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationBuilder : MonoBehaviour
{
    private GridCheck grid => Camera.main.GetComponent<GridCheck>();
    public bool isBuildingStation;


    private void Update()
    {
        if (isBuildingStation && Input.GetMouseButtonDown(0))
        {
            SetStartPos(grid.gridPoint);
        }
    }


    void SetStartPos(Vector3 currentPos)
    {
        //if (currentPos)
        //Instantiate line of walls along vector to mouse end point in update -- bad solution, too performance intensive for large scale  //  combine mesh https://www.youtube.com/watch?v=2QY5jphJt5U
        
        //Instantiate single object, then scale along x? to end point

        //Create list of wall start and end points as player clicks
        //Allow continued implementation. If end point = start point, mark it as a room, and auto fill floor?
        //Need to add exceptions for starting on an existing wall
    }

    void SetWall()
    {

    }

    void FillFloor()
    {
        //Button for a paint bucket-like tool to fill in the floor
        //Maybe a snap, drag, snap system? 
        //If a wall is converted to a room, find each corner of the room... then what? how do i create a custom polygon to fill the space?
        //Maybe a culled plane?
    }
}
