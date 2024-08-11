using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCheck : MonoBehaviour
{
    public bool placementValid;
    private List<Collider> trackedColliders = new List<Collider>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ConveyorBelt") || other.gameObject.CompareTag("Machine"))
        {
            trackedColliders.Add(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (trackedColliders.Contains(other))
        {
            trackedColliders.Remove(other);
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
