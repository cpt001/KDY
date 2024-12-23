using FactoryFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightClickDelete : MonoBehaviour
{
    private BuildingPlacement buildingPlacement;
    private ConveyorPlacement conveyorPlacement;

    private void Awake()
    {
        buildingPlacement = FindObjectOfType<BuildingPlacement>();
        conveyorPlacement = FindObjectOfType<ConveyorPlacement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (buildingPlacement.IsPlacing || conveyorPlacement.IsCreatingPath)
            return;
        // right click to delete
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            foreach (RaycastHit hit in Physics.RaycastAll(ray, 100f))
            {
                // after we delete something just return so we dont delete multiple
                if (hit.collider.transform.root.TryGetComponent(out LogisticComponent lc))
                {
                    lc.DisconnectAll();
                    Destroy(lc.gameObject);
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
