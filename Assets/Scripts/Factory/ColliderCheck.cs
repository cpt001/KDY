using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCheck : MonoBehaviour
{
    public bool placementValid;
    public List<Collider> trackedColliders = new List<Collider>();
    private Renderer buildingGhostRenderer => GetComponent<Renderer>();
    public bool collidingWithWall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ConveyorBelt") || other.gameObject.CompareTag("Machine"))
        {
            trackedColliders.Add(other);
        }
        if (other.CompareTag("Wall"))
        {
            if (!this.CompareTag("Wall"))
            {
                //Wall on Wall
            }
            else if (this.CompareTag("Wall") && name == "")
            {
                //Snap to wall
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (trackedColliders.Contains(other))
        {
            trackedColliders.Remove(other);
        }
        if (trackedColliders.Count != 0)
        {
            buildingGhostRenderer.material.SetColor("_Color", Color.red);
        }
        else
        {
            buildingGhostRenderer.material.SetColor("_Color", Color.blue);
        }
    }
    private void FixedUpdate()
    {
        if (trackedColliders.Count == 0)
        {
            placementValid = true;
        }
        else
        {
            placementValid = false;
        }
    }
}
