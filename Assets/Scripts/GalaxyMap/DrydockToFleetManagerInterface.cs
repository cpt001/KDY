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
                            fleetManager.FactoryFighterCount++;
                            break;
                        }
                    case ("Bomber"):
                        {
                            fleetManager.FactoryBomberCount++;
                            break;
                        }
                    case ("Gunship"):
                        {
                            fleetManager.FactoryGunshipCount++;
                            break;
                        }
                    case ("Transport"):
                        {
                            fleetManager.FactoryTransportCount++;
                            break;
                        }
                    case ("Gunboat"):
                        {
                            fleetManager.FactoryGunboatCount++;
                            break;
                        }
                    case ("Corvette"):
                        {
                            fleetManager.FactoryCorvetteCount++;
                            break;
                        }
                    case ("Destroyer"):
                        {
                            fleetManager.FactoryDestroyerCount++;
                            break;
                        }
                    case ("Frigate"):
                        {
                            fleetManager.FactoryFrigateCount++;
                            break;
                        }
                    case ("Cruiser"):
                        {
                            fleetManager.FactoryCruiserCount++;
                            break;
                        }
                    case ("Battleship"):
                        {
                            fleetManager.FactoryBattleshipCount++;
                            break;
                        }                   
                    case ("Carrier"):
                        {
                            fleetManager.FactoryCarrierCount++;
                            break;
                        }
                    case ("Dreadnought"):
                        {
                            fleetManager.FactoryDreadnoughtCount++;
                            break;
                        }
                }
            }
            //Clear the output
            thisDrydock.ClearOutputStorage();
        }
    }
}
