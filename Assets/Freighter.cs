using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryFramework;

//Destination, inventory, target dock, return timer
public class Freighter : MonoBehaviour
{
    private enum FreighterClass { Small, Medium, Large, ExtraLarge }
    [SerializeField] private FreighterClass freighterClass;
    private float jumpDist;
    private float jumpRechargeTimer;
    private float capacityFillTimer;
    private int maxNumOfItemsCarried;
    private float health;

    private Transform dockDestination;
    private Transform holdingPatternDestination;    //Utilize when all docks occupied
    private List<LocalStorage> storedItems; //This is one route to take
    public Dictionary<Item, int> itemsOnFreighter = new Dictionary<Item, int>();
    //Star Data
    private int returnTimer;

    private void OnEnable()
    {
        SetFreighterData();
    }
    void SetFreighterData()
    {
        switch (freighterClass)
        {
            case FreighterClass.Small:
            {
                break;
            }
            case FreighterClass.Medium:
            {
                break;
            }
            case FreighterClass.Large:
            {
                break;
            }
            case FreighterClass.ExtraLarge:
            {
                break;
            }
        }
    }

    void SetFreighterInventory()
    {
        //get target star. if none, return
        //calculate number of jumps to target star
        //set return timer based on star distance, add jump recharge timer for each star on route, times by 2?
        //at destination, generate items based on the star's chances, fill to freighter capacity after set timer

    }


}
