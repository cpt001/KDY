using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryFramework;
/// <summary>
/// This script allows manual adding of items to the docks via interface.
/// This will also serve as a reminder later when the freighter is being built on how to interface with the dock
/// </summary>
public class DockDebugInterface : MonoBehaviour
{
    public LocalStorage localStorage;
    public Storage targetStorage;

    [Header("Debug Items")]
    [SerializeField] private Item Copper;
    [SerializeField] private Item Gold;
    [SerializeField] private Item Iron;
    [SerializeField] private Item PlantMatter;
    [SerializeField] private Item Quartz;
    [SerializeField] private Item Zinc;

    public void AddDebugCopper()
    {
        //targetStorage.storage = new ItemStack[1];   //This is progress. Allows setting of item slot -- Anti-progress. Itemstack is reinitialized with this

        //Only copper works for the moment... but it is working
        targetStorage.AddItem(Copper, 5000);    //This is limited by the max stack from the prefab

        //targetStorage.storage.SetValue(Copper, 99999);  //This still isnt right. 
        //localStorage.Add(Copper, 99999);
    }
    public void AddDebugGold()
    {
        targetStorage.AddItem(Gold, 5000);

    }
    public void AddDebugIron()
    {
        targetStorage.AddItem(Iron, 5000);

    }
    public void AddDebugPlant()
    {
        targetStorage.AddItem(PlantMatter, 5000);

    }
    public void AddDebugQuartz()
    {
        targetStorage.AddItem(Quartz, 5000);

    }
    public void AddDebugZinc()
    {
        targetStorage.AddItem(Zinc, 5000);

    }
}
