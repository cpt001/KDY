using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryFramework;
/// <summary>
/// This is a transition layer, placed on drydocks. 
/// When a construction is complete, it sends the completed ship to the fleet creation manager pool
/// </summary>
public class DrydockToFleetManagerInterface : MonoBehaviour
{
    private Processor thisDrydock => GetComponent<Processor>();
    private FleetCreationManager fleetManager => GameObject.Find("EventSystem").GetComponent<FleetCreationManager>();

    private void LateUpdate()
    {
        //Length not 0 when starting?
        if (thisDrydock.outputItems.Length != 0)
        {
            //Check what items are in the output slot
            foreach (LocalStorage ls in thisDrydock.outputItems)
            {
                switch (ls.ItemType.name)
                {
                    case ("Fighter"):
                        {
                            fleetManager.FighterCount++;
                            break;
                        }
                    case ("Bomber"):
                        {
                            fleetManager.BomberCount++;
                            break;
                        }
                    case ("Gunship"):
                        {
                            fleetManager.GunshipCount++;
                            break;
                        }
                    case ("Transport"):
                        {
                            fleetManager.TransportCount++;
                            break;
                        }
                    case ("Gunboat"):
                        {
                            fleetManager.GunboatCount++;
                            break;
                        }
                    case ("Corvette"):
                        {
                            fleetManager.CorvetteCount++;
                            break;
                        }
                    case ("Destroyer"):
                        {
                            fleetManager.DestroyerCount++;
                            break;
                        }
                    case ("Frigate"):
                        {
                            fleetManager.FrigateCount++;
                            break;
                        }
                    case ("Cruiser"):
                        {
                            fleetManager.CruiserCount++;
                            break;
                        }
                    case ("Battleship"):
                        {
                            fleetManager.BattleshipCount++;
                            break;
                        }                   
                    case ("Carrier"):
                        {
                            fleetManager.CarrierCount++;
                            break;
                        }
                    case ("Dreadnought"):
                        {
                            fleetManager.DreadnoughtCount++;
                            break;
                        }
                }
            }
            //Clear the output
            thisDrydock.ClearOutputStorage();
        }
    }
}
