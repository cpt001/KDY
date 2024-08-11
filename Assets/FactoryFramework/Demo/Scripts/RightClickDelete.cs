using FactoryFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightClickDelete : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {       
        // right click to delete
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            foreach (RaycastHit hit in Physics.RaycastAll(ray, 100f))
            {
                // after we delete something just return so we dont delete multiple
                if (hit.collider.transform.root.TryGetComponent<Conveyor>(out Conveyor conveyor))
                {
                    conveyor.Disconnect();
                    Destroy(conveyor.gameObject);
                    return;
                }

                if (hit.collider.gameObject.TryGetComponent<Building>(out Building building))
                {
                    foreach (Socket socket in building.gameObject.GetComponentsInChildren<Socket>())
                    {
                        //remove this from sockets
                        socket.Disconnect();
                    }
                    Destroy(building.gameObject);
                    return;
                }

                if (hit.collider.TryGetComponent(out PowerGridComponent pgc))
                {
                    Destroy(pgc.gameObject);
                    return;
                }
            }
        }
        return;
    }
}
