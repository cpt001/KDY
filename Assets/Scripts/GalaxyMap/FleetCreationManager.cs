using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Script
/// -Tracks every ship built within the factory, passed in by dd to fleetmanager scripts
/// -Allows management of fleets at the factory
/// </summary>
/// 
public class FleetCreationManager : MonoBehaviour
{
    public List<FleetData> FleetsPresent = new List<FleetData>();
    public List<ShipData> DamagedShips = new List<ShipData>();
    #region How many of each ship exist in the factory
    //Accessed by drydock interface, needs to stay public
    [Header("Factory Counts")]
    public int FactoryFighterCount;
    [SerializeField] private TextMeshProUGUI fighterFactoryCountText;
    public int FactoryBomberCount;
    [SerializeField] private TextMeshProUGUI bomberFactoryCountText;

    public int FactoryGunshipCount;
    [SerializeField] private TextMeshProUGUI gunshipFactoryCountText;
    public int FactoryTransportCount;
    [SerializeField] private TextMeshProUGUI transportFactoryCountText;
    public int FactoryGunboatCount;
    [SerializeField] private TextMeshProUGUI gunboatFactoryCountText;

    public int FactoryCorvetteCount;
    [SerializeField] private TextMeshProUGUI corvetteFactoryCountText;
    public int FactoryDestroyerCount;
    [SerializeField] private TextMeshProUGUI destroyerFactoryCountText;
    public int FactoryFrigateCount;
    [SerializeField] private TextMeshProUGUI frigateFactoryCountText;

    public int FactoryCruiserCount;
    [SerializeField] private TextMeshProUGUI cruiserFactoryCountText;
    public int FactoryBattleshipCount;
    [SerializeField] private TextMeshProUGUI battleshipFactoryCountText;
    public int FactoryCarrierCount;
    [SerializeField] private TextMeshProUGUI carrierFactoryCountText;

    public int FactoryDreadnoughtCount;
    [SerializeField] private TextMeshProUGUI dreadnoughtFactoryCountText;
    #endregion
    #region How many of each ship exist in the currently managed fleet
    [Header("Fleet Counts")]
    [SerializeField] private TextMeshProUGUI fighterFleetCountText;
    private int FleetFighterCount;
    private int FleetBomberCount;
    [SerializeField] private TextMeshProUGUI bomberFleetCountText;

    private int FleetGunshipCount;
    [SerializeField] private TextMeshProUGUI gunshipFleetCountText;
    private int FleetTransportCount;
    [SerializeField] private TextMeshProUGUI transportFleetCountText;
    private int FleetGunboatCount;
    [SerializeField] private TextMeshProUGUI gunboatFleetCountText;

    private int FleetCorvetteCount;
    [SerializeField] private TextMeshProUGUI corvetteFleetCountText;
    private int FleetDestroyerCount;
    [SerializeField] private TextMeshProUGUI destroyerFleetCountText;
    private int FleetFrigateCount;
    [SerializeField] private TextMeshProUGUI frigateFleetCountText;

    private int FleetCruiserCount;
    [SerializeField] private TextMeshProUGUI cruiserFleetCountText;
    private int FleetBattleshipCount;
    [SerializeField] private TextMeshProUGUI battleshipFleetCountText;
    private int FleetCarrierCount;
    [SerializeField] private TextMeshProUGUI carrierFleetCountText;

    private int FleetDreadnoughtCount;
    [SerializeField] private TextMeshProUGUI dreadnoughtFleetCountText;
    #endregion
    #region How many damaged ships exist in dock
    [Header("Damaged Counts")]
    [SerializeField] private TextMeshProUGUI fighterDamagedCountText;
    private int DamagedFighterCount;
    private int DamagedBomberCount;
    [SerializeField] private TextMeshProUGUI bomberDamagedCountText;

    private int DamagedGunshipCount;
    [SerializeField] private TextMeshProUGUI gunshipDamagedCountText;
    private int DamagedTransportCount;
    [SerializeField] private TextMeshProUGUI transportDamagedCountText;
    private int DamagedGunboatCount;
    [SerializeField] private TextMeshProUGUI gunboatDamagedCountText;

    private int DamagedCorvetteCount;
    [SerializeField] private TextMeshProUGUI corvetteDamagedCountText;
    private int DamagedDestroyerCount;
    [SerializeField] private TextMeshProUGUI destroyerDamagedCountText;
    private int DamagedFrigateCount;
    [SerializeField] private TextMeshProUGUI frigateDamagedCountText;

    private int DamagedCruiserCount;
    [SerializeField] private TextMeshProUGUI cruiserDamagedCountText;
    private int DamagedBattleshipCount;
    [SerializeField] private TextMeshProUGUI battleshipDamagedCountText;
    private int DamagedCarrierCount;
    [SerializeField] private TextMeshProUGUI carrierDamagedCountText;

    private int DamagedDreadnoughtCount;
    [SerializeField] private TextMeshProUGUI dreadnoughtDamagedCountText;
    #endregion
    public List<ShipData> damagedShipsInDock;


    [SerializeField] private TMPro.TMP_InputField fleetNameInput;
    [SerializeField] private GameObject fleetButtonContainer;
    [SerializeField] private GameObject fleetButtonPrefab;

    private void Start()
    {
        InitializeRandomShipAmounts();
        UpdateCounterText();
    }

    void InitializeRandomShipAmounts()
    {
        FactoryFighterCount = Random.Range(0, 20);
        FactoryBomberCount = Random.Range(0, 20);
        FactoryGunshipCount = Random.Range(0, 20);
        FactoryTransportCount = Random.Range(0, 20);
        FactoryGunboatCount = Random.Range(0, 20);
        FactoryCorvetteCount = Random.Range(0, 20);
        FactoryDestroyerCount = Random.Range(0, 20);
        FactoryFrigateCount = Random.Range(0, 20);
        FactoryCruiserCount = Random.Range(0, 20);
        FactoryBattleshipCount = Random.Range(0, 20);
        FactoryCarrierCount = Random.Range(0, 20);
        FactoryDreadnoughtCount = Random.Range(0, 20);
    }

    //Opens a simple interface that allows the player to name their new fleet, then opens the manage fleet screen
    public void CreateNewFleet()
    {
        FleetData newFleetData = gameObject.AddComponent<FleetData>();
        if (fleetNameInput.text != "")  //Apparently the field isnt null, but also doesnt contain anything x.x
        {
            Debug.Log("name input: " + fleetNameInput.text);
            Debug.Log("Named fleet created");
            newFleetData.fleetID = fleetNameInput.text;
            //ManageFleet(newFleetData);
            FleetsPresent.Add(newFleetData);
            UpdateFleetsInDock();
        }
        else
        {
            //Autoname
            Debug.Log("Generic fleet created");
            //int currentFleetCount = FleetsPresent.Count + 1;
            newFleetData.fleetID = "Fleet " + FleetsPresent.Count;
            //ManageFleet(newFleetData);
            FleetsPresent.Add(newFleetData);
            UpdateFleetsInDock();
        }
    }

    public void DisbandFleet(FleetData targetFleet)
    {
        //Return all ships to drydock
        //Sort damaged ships into slots
        //Destroy compoonent
        Destroy(targetFleet);
    }

    public void UpdateFleetsInDock()
    {
        //Remove all previous buttons
        foreach (Transform t in fleetButtonContainer.transform)
        {
            Destroy(t.gameObject);
        }
        //Creates buttons with data on an increasing decrement to populate the fleet manager
        int currentDecrement = 0;
        for (int i = 0; i < FleetsPresent.Count; i++)
        {
            var fleetButton = Instantiate(fleetButtonPrefab, new Vector3(fleetButtonContainer.transform.position.x, fleetButtonContainer.transform.position.y - currentDecrement, fleetButtonContainer.transform.position.z), fleetButtonPrefab.transform.rotation, fleetButtonContainer.transform);
            fleetButton.GetComponent<FleetInterface>().buttonText.text = FleetsPresent[i].fleetID;
            fleetButton.GetComponent<FleetInterface>().associatedFleet = FleetsPresent[i];
            currentDecrement += 12; //Not screen respective
        }
    }

    public void ManageFleet(FleetData targetFleet)
    {
        //0's out previous fleet input
        FleetFighterCount = 0;
        FleetBomberCount = 0;
        FleetGunshipCount = 0;
        FleetTransportCount = 0;
        FleetGunboatCount = 0;
        FleetCorvetteCount = 0;
        FleetDestroyerCount = 0;
        FleetFrigateCount = 0;
        FleetCruiserCount = 0;
        FleetBattleshipCount = 0;
        FleetCarrierCount = 0;
        FleetDreadnoughtCount = 0;
        //Populate list of ships from existing fleet, reduces to simple numbers for modification
        if (targetFleet.shipsInFleet != null)
        {
            foreach (ShipData ship in targetFleet.shipsInFleet)
            {
                switch (ship.thisShip)
                {
                    case ShipData.ShipType.Fighter:
                        {
                            if (ship.health != ship.maxHealth)
                            {
                                FleetFighterCount++;
                            }
                            else
                            {
                                DamagedFighterCount++;
                            }
                            targetFleet.shipsInFleet.Remove(ship);
                            break;
                        }
                    case ShipData.ShipType.Bomber:
                        {
                            if (ship.health != ship.maxHealth)
                            {
                                FleetBomberCount++;
                            }
                            else
                            {
                                DamagedBomberCount++;
                            }
                            targetFleet.shipsInFleet.Remove(ship);
                            break;
                        }
                    case ShipData.ShipType.Gunship:
                        {
                            if (ship.health != ship.maxHealth)
                            {
                                FleetGunshipCount++;
                            }
                            else
                            {
                                DamagedGunshipCount++;
                            }
                            targetFleet.shipsInFleet.Remove(ship);
                            break;
                        }
                    case ShipData.ShipType.Transport:
                        {
                            if (ship.health != ship.maxHealth)
                            {
                                FleetTransportCount++;
                            }
                            else
                            {
                                DamagedTransportCount++;
                            }
                            targetFleet.shipsInFleet.Remove(ship);
                            break;
                        }
                    case ShipData.ShipType.Gunboat:
                        {
                            if (ship.health != ship.maxHealth)
                            {
                                FleetGunboatCount++;
                            }
                            else
                            {
                                DamagedGunboatCount++;
                            }
                            targetFleet.shipsInFleet.Remove(ship);
                            break;
                        }
                    case ShipData.ShipType.Corvette:
                        {
                            if (ship.health != ship.maxHealth)
                            {
                                FleetCorvetteCount++;
                            }
                            else
                            {
                                DamagedCorvetteCount++;
                            }
                            targetFleet.shipsInFleet.Remove(ship);
                            break;
                        }
                    case ShipData.ShipType.Destroyer:
                        {
                            if (ship.health != ship.maxHealth)
                            {
                                FleetDestroyerCount++;
                            }
                            else
                            {
                                DamagedDestroyerCount++;
                            }
                            targetFleet.shipsInFleet.Remove(ship);
                            break;
                        }
                    case ShipData.ShipType.Frigate:
                        {
                            if (ship.health != ship.maxHealth)
                            {
                                FleetFrigateCount++;
                            }
                            else
                            {
                                DamagedFrigateCount++;
                            }
                            targetFleet.shipsInFleet.Remove(ship);
                            break;
                        }
                    case ShipData.ShipType.Cruiser:
                        {
                            if (ship.health != ship.maxHealth)
                            {
                                FleetCruiserCount++;
                            }
                            else
                            {
                                DamagedCruiserCount++;
                            }
                            targetFleet.shipsInFleet.Remove(ship);
                            break;
                        }
                    case ShipData.ShipType.Battleship:
                        {
                            if (ship.health != ship.maxHealth)
                            {
                                FleetBattleshipCount++;
                            }
                            else
                            {
                                DamagedBattleshipCount++;
                            }
                            targetFleet.shipsInFleet.Remove(ship);
                            break;
                        }
                    case ShipData.ShipType.Carrier:
                        {
                            if (ship.health != ship.maxHealth)
                            {
                                FleetCarrierCount++;
                            }
                            else
                            {
                                DamagedCarrierCount++;
                            }
                            targetFleet.shipsInFleet.Remove(ship);
                            break;
                        }
                    case ShipData.ShipType.Dreadnought:
                        {
                            if (ship.health != ship.maxHealth)
                            {
                                FleetDreadnoughtCount++;
                            }
                            else
                            {
                                DamagedDreadnoughtCount++;
                            }
                            targetFleet.shipsInFleet.Remove(ship);
                            break;
                        }
                }
            }
        }
        UpdateCounterText();
    }

    //Sends collective fleet data from UI to fleetdata
    public void ConfirmFleetChanges(FleetData targetFleet)
    {
        for (int i = 0; i < FleetFighterCount; i++)
        {
            ShipData newShip = new ShipData();
            newShip.thisShip = ShipData.ShipType.Fighter;

            targetFleet.shipsInFleet.Add(newShip);
        }
        for (int i = 0; i < FleetBomberCount; i++)
        {
            ShipData newShip = new ShipData();
            newShip.thisShip = ShipData.ShipType.Bomber;

            targetFleet.shipsInFleet.Add(newShip);
        }
        for (int i = 0; i < FleetGunshipCount; i++)
        {
            ShipData newShip = new ShipData();
            newShip.thisShip = ShipData.ShipType.Gunship;

            targetFleet.shipsInFleet.Add(newShip);
        }
        for (int i = 0; i < FleetTransportCount; i++)
        {
            ShipData newShip = new ShipData();
            newShip.thisShip = ShipData.ShipType.Transport;

            targetFleet.shipsInFleet.Add(newShip);
        }
        for (int i = 0; i < FleetGunboatCount; i++)
        {
            ShipData newShip = new ShipData();
            newShip.thisShip = ShipData.ShipType.Gunboat;

            targetFleet.shipsInFleet.Add(newShip);
        }
        for (int i = 0; i < FleetCorvetteCount; i++)
        {
            ShipData newShip = new ShipData();
            newShip.thisShip = ShipData.ShipType.Corvette;

            targetFleet.shipsInFleet.Add(newShip);
        }
        for (int i = 0; i < FleetDestroyerCount; i++)
        {
            ShipData newShip = new ShipData();
            newShip.thisShip = ShipData.ShipType.Destroyer;

            targetFleet.shipsInFleet.Add(newShip);
        }
        for (int i = 0; i < FleetFrigateCount; i++)
        {
            ShipData newShip = new ShipData();
            newShip.thisShip = ShipData.ShipType.Frigate;

            targetFleet.shipsInFleet.Add(newShip);
        }
        for (int i = 0; i < FleetCruiserCount; i++)
        {
            ShipData newShip = new ShipData();
            newShip.thisShip = ShipData.ShipType.Cruiser;

            targetFleet.shipsInFleet.Add(newShip);
        }
        for (int i = 0; i < FleetBattleshipCount; i++)
        {
            ShipData newShip = new ShipData();
            newShip.thisShip = ShipData.ShipType.Battleship;

            targetFleet.shipsInFleet.Add(newShip);
        }
        for (int i = 0; i < FleetCarrierCount; i++)
        {
            ShipData newShip = new ShipData();
            newShip.thisShip = ShipData.ShipType.Carrier;

            targetFleet.shipsInFleet.Add(newShip);
        }
        for (int i = 0; i < FleetDreadnoughtCount; i++)
        {
            ShipData newShip = new ShipData();
            newShip.thisShip = ShipData.ShipType.Dreadnought;

            targetFleet.shipsInFleet.Add(newShip);
        }
    }

    public void CloseFleetManager()
    {

    }

    #region Button fleet management
    public void AddFighter() { if (FactoryFighterCount != 0) { FleetFighterCount++; FactoryFighterCount--; UpdateCounterText(); } }
    public void AddBomber() { if (FactoryBomberCount != 0) { FleetBomberCount++; FactoryBomberCount--; UpdateCounterText(); } }
    public void AddGunship() { if (FactoryGunshipCount != 0) { FleetGunshipCount++; FactoryGunshipCount--; UpdateCounterText(); } }
    public void AddTransport() { if (FactoryTransportCount != 0) { FleetTransportCount++; FactoryTransportCount--; UpdateCounterText(); } }
    public void AddGunboat() { if (FactoryGunboatCount != 0) { FleetGunboatCount++; FactoryGunboatCount--; UpdateCounterText(); } }
    public void AddCorvette() { if (FactoryCorvetteCount != 0) { FleetCorvetteCount++; FactoryCorvetteCount--; UpdateCounterText(); } }
    public void AddDestroyer() { if (FactoryDestroyerCount != 0) { FleetDestroyerCount++; FactoryDestroyerCount--; UpdateCounterText(); } }
    public void AddFrigate() { if (FactoryFrigateCount != 0) { FleetFrigateCount++; FactoryFrigateCount--; UpdateCounterText(); } }
    public void AddCruiser() { if (FactoryCruiserCount != 0) { FleetCruiserCount++; FactoryCruiserCount--; UpdateCounterText(); } }
    public void AddBattleship() { if (FactoryBattleshipCount != 0) { FleetBattleshipCount++; FactoryBattleshipCount--; UpdateCounterText(); } }
    public void AddCarrier() { if (FactoryCarrierCount != 0) { FleetCarrierCount++; FactoryCarrierCount--; UpdateCounterText(); } }
    public void AddDreadnought() { if (FactoryDreadnoughtCount != 0) { FleetDreadnoughtCount++; FactoryDreadnoughtCount--; UpdateCounterText(); } }

    public void RemoveFighter() { if (FleetFighterCount != 0) { FleetFighterCount--; FactoryFighterCount++; UpdateCounterText(); } }
    public void RemoveBomber() { if (FleetBomberCount != 0) { FleetBomberCount--; FactoryBomberCount++; UpdateCounterText(); } }
    public void RemoveGunship() { if (FleetGunshipCount != 0) { FleetGunshipCount--; FactoryGunshipCount++; UpdateCounterText(); } }
    public void RemoveTransport() { if (FleetTransportCount != 0) { FleetTransportCount--; FactoryTransportCount++; UpdateCounterText(); } }
    public void RemoveGunboat() { if (FleetGunboatCount != 0) { FleetGunboatCount--; FactoryGunboatCount++; UpdateCounterText(); } }
    public void RemoveCorvette() { if (FleetCorvetteCount != 0) { FleetCorvetteCount--; FactoryCorvetteCount++; UpdateCounterText(); } }
    public void RemoveDestroyer() { if (FleetDestroyerCount != 0) { FleetDestroyerCount--; FactoryDestroyerCount++; UpdateCounterText(); } }
    public void RemoveFrigate() { if (FleetFrigateCount != 0) { FleetFrigateCount--; FactoryFrigateCount++; UpdateCounterText(); } }
    public void RemoveCruiser() { if (FleetCruiserCount != 0) { FleetCruiserCount--; FactoryCruiserCount++; UpdateCounterText(); } }
    public void RemoveBattleship() { if (FleetBattleshipCount != 0) { FleetBattleshipCount--; FactoryBattleshipCount++; UpdateCounterText(); } }
    public void RemoveCarrier() { if (FleetCarrierCount != 0) { FleetCarrierCount--; FactoryCarrierCount++; UpdateCounterText(); } }
    public void RemoveDreadnought() { if (FleetDreadnoughtCount != 0) { FleetDreadnoughtCount--; FactoryDreadnoughtCount++; UpdateCounterText(); } }
    #endregion

    void UpdateCounterText()
    {
        //Factory
        fighterFactoryCountText.text = FactoryFighterCount.ToString();
        bomberFactoryCountText.text = FactoryBomberCount.ToString();
        gunshipFactoryCountText.text = FactoryGunshipCount.ToString();
        transportFactoryCountText.text = FactoryTransportCount.ToString();
        gunboatFactoryCountText.text = FactoryGunboatCount.ToString();
        corvetteFactoryCountText.text = FactoryCorvetteCount.ToString();
        destroyerFactoryCountText.text = FactoryDestroyerCount.ToString();
        frigateFactoryCountText.text = FactoryFrigateCount.ToString();
        cruiserFactoryCountText.text = FactoryCruiserCount.ToString();
        battleshipFactoryCountText.text = FactoryBattleshipCount.ToString();
        carrierFactoryCountText.text = FactoryCarrierCount.ToString();
        dreadnoughtFactoryCountText.text = FactoryDreadnoughtCount.ToString();
        //Fleet
        fighterFleetCountText.text = FleetFighterCount.ToString();
        bomberFleetCountText.text = FleetBomberCount.ToString();
        gunshipFleetCountText.text = FleetGunshipCount.ToString();
        transportFleetCountText.text = FleetTransportCount.ToString();
        gunboatFleetCountText.text = FleetGunboatCount.ToString();
        corvetteFleetCountText.text = FleetCorvetteCount.ToString();
        destroyerFleetCountText.text = FleetDestroyerCount.ToString();
        frigateFleetCountText.text = FleetFrigateCount.ToString();
        cruiserFleetCountText.text = FleetCruiserCount.ToString();
        battleshipFleetCountText.text = FleetBattleshipCount.ToString();
        carrierFleetCountText.text = FleetCarrierCount.ToString();
        dreadnoughtFleetCountText.text = FleetDreadnoughtCount.ToString();
        //Damaged
        fighterDamagedCountText.text = DamagedFighterCount.ToString();
        bomberDamagedCountText.text = DamagedBomberCount.ToString();
        gunshipDamagedCountText.text = DamagedGunshipCount.ToString();
        transportDamagedCountText.text = DamagedTransportCount.ToString();
        gunboatDamagedCountText.text = DamagedGunboatCount.ToString();
        corvetteDamagedCountText.text = DamagedCorvetteCount.ToString();
        destroyerDamagedCountText.text = DamagedDestroyerCount.ToString();
        frigateDamagedCountText.text = DamagedFrigateCount.ToString();
        cruiserDamagedCountText.text = DamagedCruiserCount.ToString();
        battleshipDamagedCountText.text = DamagedBattleshipCount.ToString();
        carrierDamagedCountText.text = DamagedCarrierCount.ToString();
        dreadnoughtDamagedCountText.text = DamagedDreadnoughtCount.ToString();
    }
    void AddShipToTargetFleet(FleetData targetFleet, ShipData.ShipType shipType)
    {

    }
}