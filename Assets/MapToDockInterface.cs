using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryFramework;

/// <summary>
/// OOP
/// -Freighter arrives in dock
/// -Freighter transfers information to dock, locks bool to prevent cloned inventories
/// -Information transfers to storage component
/// </summary>

public class MapToDockInterface : MonoBehaviour
{
    private Processor dockProcessorComponent;
    private Storage dockStorageComponent;
    private StorageInspector storageInspector;
    private LocalStorage localStorage;
    private Freighter freighterInDock;
    private bool cargoReceived;

    [Header("Debug Items")]
    [SerializeField] private Item Copper;
    [SerializeField] private Item Gold;
    [SerializeField] private Item Iron;
    [SerializeField] private Item PlantMatter;
    [SerializeField] private Item Quartz;
    [SerializeField] private Item Zinc;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (freighterInDock && !cargoReceived)
        {
            ReceiveFreighterInventory();
        }
    }

    void ReceiveFreighterInventory()
    {
        foreach (KeyValuePair<Item, int> targetItem in freighterInDock.itemsOnFreighter)
        {
            localStorage.Add(targetItem.Key, targetItem.Value);
        }
        cargoReceived = true;
    }
    public void AddDebugCopper()
    {
        localStorage.Add(Copper, 99999);
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
