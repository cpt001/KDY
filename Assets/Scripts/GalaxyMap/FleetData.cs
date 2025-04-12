using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This is responsible for each fleet's data.
/// </summary>
public class FleetData : MonoBehaviour
{
    public string fleetID;
    public List<ShipData> shipsInFleet = new List<ShipData>();
    public FleetData hostileFleet;
    //Temp, set actual system later
    public Transform currentSystem;

    //This is triggered when a fleet enters a new system
    void AttackEnemyFleet()
    {
        foreach (ShipData ships in shipsInFleet)
        {
            ships.SetTurretTargets(hostileFleet);
        }
    }
}
