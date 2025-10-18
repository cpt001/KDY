using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Sector : MonoBehaviour
{
    public int sectorPriority;  //Sets the chance of this sector being combined into another
    public List<StarSystem> starSystems = new List<StarSystem>();
    public bool markForCombination = false;
    public List<Sector> localSectors = new List<Sector>();
    public bool playerCapitalSector = false;
    public bool enemyCapitalSector = false;

    public List<Transform> systemsInSector = new List<Transform>();

    public void StartLinkingCoroutine()
    {
        StartCoroutine(LinkStars());
    }

    public IEnumerator LinkStars()
    {
        //Get furthest object from center of sector, allow connection to other sectors. 
        systemsInSector = systemsInSector.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToList();
        //Allow 1 or 2 connections within the sector
        for (int i = 0; i < systemsInSector.Count; i++)
        {
            if (i < systemsInSector.Count - 2)
            {
                StarSystem currentSystem = systemsInSector[i].GetComponent<StarSystem>();
                currentSystem.allowExternalConnections = false;
                currentSystem.maxConnections = 2;

                //Clone this list into the connected stars variable
                foreach (StarSystem star in starSystems)
                {
                    if (star != currentSystem)
                    {
                        currentSystem.possibleStarConnections.Add(star);
                    }
                }
                //Sort the connected stars list -- Works, idiot forgot the initial list to reference
                currentSystem.possibleStarConnections = currentSystem.possibleStarConnections.OrderBy(z => Vector3.Distance(z.transform.position, currentSystem.transform.position)).ToList();
                //Remove extra stars
                currentSystem.possibleStarConnections.RemoveRange(2, 12);
            }
            else
            {
                systemsInSector[i].GetComponent<StarSystem>().allowExternalConnections = true;
                systemsInSector[i].GetComponent<StarSystem>().maxConnections = 3;
            }
            //Still need to create actual linkage system that allows stars to detect other nearby stars
            //Maybe draw a raycast to the other stars in the sector, compare those distances.
            //Fringe stars - those allowing connections to other sectors - can use a collider based search

        }
        yield return null;
    }
}
