using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour
{
    public int sectorPriority;  //Sets the chance of this sector being combined into another
    public List<StarSystem> starSystems = new List<StarSystem>();
    public bool markForCombination = false;
    public List<Sector> localSectors = new List<Sector>();
    public bool playerCapitalSector = false;
    public bool enemyCapitalSector = false;

    //Generate star and system data
}
