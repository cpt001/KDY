using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryFramework;

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
        targetStorage.storage = new ItemStack[1];   //This is progress. Allows setting of item slot
        //targetStorage.storage.SetValue(Copper, 99999);  //This still isnt right. 
        //localStorage.Add(Copper, 99999);
    }
    public void AddDebugGold()
    {
        localStorage.Add(Gold, 99999);
    }
    public void AddDebugIron()
    {
        localStorage.Add(Iron, 99999);
    }
    public void AddDebugPlant()
    {
        localStorage.Add(PlantMatter, 99999);
    }
    public void AddDebugQuartz()
    {
        localStorage.Add(Quartz, 99999);
    }
    public void AddDebugZinc()
    {
        localStorage.Add(Zinc, 99999);
    }
}
