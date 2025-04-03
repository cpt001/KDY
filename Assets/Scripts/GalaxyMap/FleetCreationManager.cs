using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script
/// -Tracks every ship built within the factory, passed in by dd to fleetmanager scripts
/// -Allows management of fleets at the factory
/// </summary>
/// 
public class FleetCreationManager : MonoBehaviour
{
    public FleetData fleetData;
    public List<FleetData> FleetsPresent = new List<FleetData>();
    public List<ShipData> DamagedShips = new List<ShipData>();

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
            newFleetData.fleetID = "Fleet " + FleetsPresent.Count + 1.ToString();
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


    public void AddFighter(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void AddBomber(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void AddGunship(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void AddTransport(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void AddGunboat(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void AddCorvette(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void AddDestroyer(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void AddFrigate(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void AddCruiser(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void AddBattleship(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void AddCarrier(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void AddDreadnought(FleetData targetFleet) { if (FighterCount != 0) { } }

    public void RemoveFighter(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void RemoveBomber(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void RemoveGunship(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void RemoveTransport(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void RemoveGunboat(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void RemoveCorvette(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void RemoveDestroyer(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void RemoveFrigate(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void RemoveCruiser(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void RemoveBattleship(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void RemoveCarrier(FleetData targetFleet) { if (FighterCount != 0) { } }
    public void RemoveDreadnought(FleetData targetFleet) { if (FighterCount != 0) { } }
}
