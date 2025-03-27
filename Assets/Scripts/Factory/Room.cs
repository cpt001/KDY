using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Wall> walls = new List<Wall>();

    void CreateNewRoom(Wall nearWall, Wall farWall)
    {
        foreach (Wall wall in walls)
        {
            if (wall.StartConnection != null && wall.EndConnection != null)
            {
                //Room valid, can place floor
            }
            else
            {
                Debug.Log("Room invalid; " + wall + " Start: " + wall.StartConnection + " | End: " + wall.EndConnection);
            }
        }
    }
}
