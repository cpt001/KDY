using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FleetInterface : MonoBehaviour
{
    private FleetCreationManager fleetManager => GameObject.Find("EventSystem").GetComponent<FleetCreationManager>();
    public FleetData associatedFleet;
    public TextMeshProUGUI buttonText;

    public void SetFleetToModify()
    {
        fleetManager.ManageFleet(associatedFleet);
    }
}
