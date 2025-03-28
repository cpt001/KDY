using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Script
/// -Tracks every ship built within the factory
/// -Allows management of fleets at the factory
/// </summary>
public class DrydockToMapInterface : MonoBehaviour
{
    public int FighterCount;
    public int BomberCount;

    public int GunshipCount;
    public int TransportCount;
    public int GunboatCount;

    public int CorvetteCount;
    public int DestroyerCount;
    public int FrigateCount;

    public int CruiserCount;
    public int BattleshipCount;
    public int CarrierCount;

    public int DreadnoughtCount;

    public List<ShipData> damagedShipsInDock;
    public List<FleetData> FleetsPresent = new List<FleetData>();

    [SerializeField] private TMPro.TMP_InputField fleetNameInput;

    private void Start()
    {
        InitializeRandomShipAmounts();
    }

    void InitializeRandomShipAmounts()
    {
        FighterCount = Random.Range(0, 20);
        BomberCount = Random.Range(0, 20);
        GunshipCount = Random.Range(0, 20);
        TransportCount = Random.Range(0, 20);
        GunboatCount = Random.Range(0, 20);
        CorvetteCount = Random.Range(0, 20);
        DestroyerCount = Random.Range(0, 20);
        FrigateCount = Random.Range(0, 20);
        CruiserCount = Random.Range(0, 20);
        BattleshipCount = Random.Range(0, 20);
        CarrierCount = Random.Range(0, 20);
        DreadnoughtCount = Random.Range(0, 20);
    }

    //Opens a simple interface that allows the player to name their new fleet, then opens the manage fleet screen
    public void CreateNewFleet()
    {
        FleetData newFleetData = gameObject.AddComponent<FleetData>();
        if (fleetNameInput.text != null)
        {
            newFleetData.fleetID = fleetNameInput.text;
            ManageFleet(newFleetData);
        }
        else
        {
            //Autoname
            newFleetData.fleetID = "Fleet " + FleetsPresent.Count + 1.ToString() ;
            ManageFleet(newFleetData);
        }
    }

    public void DisbandFleet(FleetData targetFleet)
    {
        //Return all ships to drydock
        //Sort damaged ships into slots
        //Destroy compoonent
        Destroy(targetFleet);
    }

    public void ManageFleet(FleetData targetFleet)
    {
        //Get all ships
        //Display damaged in separate section?
        // + and - buttons for ships in fleet.
    }

    public void CloseFleetManager()
    {
        
    }

    public void AddFighter(FleetData targetFleet) { if (FighterCount != 0) {  } }
}
