using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// Data hierarchy [Galaxy, Sector, System, Orbiting Body
/// -Galaxy - The main goal of the game. Reconquer the entire galaxy. Holds all information.
/// -Sector - Gives the player overarching goals within their campaign to achieve
/// -System - Individual systems within a sector, containing resources and hostiles
/// -Orbiting body - Detailed information within a system to give it more life in the game
/// </summary>
public class GalController : MonoBehaviour
{
    [Header("Generation Parameters")]
    [SerializeField] private Slider galacticArmCount;   //How many arms galaxy will have    || Limited 1-11
    private float turnFraction; //Spacing between spawned nodes/sectors in fibannoci spiral
    [SerializeField] private Slider sectorCount;    //How wide the galaxy will be || Limited 30-300
    private enum GalacticCenterDeadZone { Small, Medium, Large }; //Determines how many nodes are skipped during generation
    private GalacticCenterDeadZone deadzone;
    private float deadZoneCollisionRadius;
    private bool galaxyAlreadyPresent;
    [Header("Sector Parameters")]
    [SerializeField] private GameObject sectorPrefab;
    private List<Sector> sectorsInGalaxy = new List<Sector>();
    [SerializeField] private LayerMask sectorLayerMask;
    private enum SectorSize { Small, Medium, Large };
    private SectorSize sectorSize;
    [Header("Star Parameter")]
    [SerializeField] private GameObject starPrefab;

    public void GenerateNewGalaxy()
    {
        if (!galaxyAlreadyPresent)
        {
            GenerateArms();
            galaxyAlreadyPresent = true;
        }
        else
        {
            DestroyPreviousGalaxy();
            GenerateArms();
        }
    }
    void DestroyPreviousGalaxy()
    {
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void GenerateArms()
    {
        //Nodes are generated while going a certain distance around a circle. .99 turn fraction will move 99% of the way around a circle before generating the next
        #region Sector Generation
        switch (galacticArmCount.value)
        {
            //Need to modify with furthest points
            case (1): { turnFraction = 0.99f; break; }
            case (2): { turnFraction = 0.509f; break; }
            case (3): { turnFraction = 0.3300575f; break; }
            case (4): { turnFraction = 0.2450275f; break; }
            case (5): { turnFraction = 0.195001f; break; }
            case (6): { turnFraction = 0.1700575f; break; }
            case (7): { turnFraction = 0.14001f; break; }
            case (8): { turnFraction = 0.1230575f; break; }
            case (9): { turnFraction = 0.113001f; break; }
            case (10): { turnFraction = 0.0990001f; break; }
            case (11): { turnFraction = 0.09000546f; break; }
        }

        //Generates each sector
        for (int i = 0; i < sectorCount.value; i++)
        {
            float dst = i + .001f / galacticArmCount.value;
            float angle = 2f * Mathf.PI * turnFraction * i;

            float x = dst * Mathf.Cos(angle);
            float z = dst * Mathf.Sin(angle);

            GameObject sector = Instantiate(sectorPrefab, new Vector3(x, 0, z), Quaternion.identity, gameObject.transform);
            sectorsInGalaxy.Add(sector.GetComponent<Sector>());
            //if (i == sectorCount.value - 1) defunct?
            #region Determine player and enemy spawn sectors
            if (galacticArmCount.value != 1)
            {
                if (i == (sectorCount.value - 1))
                {
                    sector.GetComponent<Sector>().enemyCapitalSector = true;
                    sector.name = sector.name + " {E}";
                    //Debug.Log("Enemy capital: " + sector);
                }
                if (i == (sectorCount.value - galacticArmCount.value))  //This doesnt seem to work very well with higher arm counts
                {
                    sector.GetComponent<Sector>().playerCapitalSector = true;
                    sector.name = sector.name + " {P}";
                    //Debug.Log("Player capital: " + sector);
                }
            }
            else
            {
                if (i == (sectorCount.value - sectorCount.value))
                {
                    sector.GetComponent<Sector>().enemyCapitalSector = true;
                    sector.name = sector.name + " {E}";
                    //Debug.Log("Enemy capital: " + sector);
                }
                if (i == (sectorCount.value - galacticArmCount.value))
                {
                    sector.GetComponent<Sector>().playerCapitalSector = true;
                    sector.name = sector.name + " {P}";
                    //Debug.Log("Player capital: " + sector);
                }
            }
            #endregion
        }
        #endregion
        //Determines size of deadzone at galactic center, and removes sectors
        #region Deadzone settings - Removes sector clusterfuck in center of galaxy
        switch (deadzone)
        {
            case GalacticCenterDeadZone.Small: { deadZoneCollisionRadius = 10.0f; break; }
            case GalacticCenterDeadZone.Medium: { deadZoneCollisionRadius = 20.0f; break; }
            case GalacticCenterDeadZone.Large: { deadZoneCollisionRadius = 30.0f; break; }
        }


        Collider[] hitColliders = Physics.OverlapSphere(transform.position, deadZoneCollisionRadius, sectorLayerMask, QueryTriggerInteraction.Collide);        
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log("Destroying hit collider " + hitCollider.gameObject);
            Destroy(hitCollider.gameObject);
        }
        #endregion
        
        if (transform.childCount == sectorCount.value)
        {
            StartCoroutine(GenerateSectors());
        }
    }

    IEnumerator GenerateSectors()
    {
        //Find and combine some sectors
        foreach (Transform sector in gameObject.transform)
        {
            //Activate particle generator with parameters from UI
            StartCoroutine(GenerateStarAndSystemData(sector.gameObject));

            switch (sectorSize)
            {
                case SectorSize.Small:
                    {
                        sector.GetComponent<Sector>().sectorPriority = Random.Range(1, 4);
                        break;
                    }
                case SectorSize.Medium:
                    {
                        sector.GetComponent<Sector>().sectorPriority = Random.Range(1, 6);
                        break;
                    }
                case SectorSize.Large:
                    {
                        sector.GetComponent<Sector>().sectorPriority = Random.Range(1, 8);
                        break;
                    }
            }

            Sector sectorComponent = sector.GetComponent<Sector>();

            if (sectorComponent.sectorPriority < 2)
            {
                yield return new WaitForSeconds(0.1f);
                //Mark for combination to another sector.
                sectorComponent.markForCombination = true;
                //-Set target sector (nearest sector over priority)
                sectorComponent.localSectors = sectorsInGalaxy;
                //This is currently sorting just by priority... it also needs to take transform locality into account
                sectorComponent.localSectors.OrderBy(t => t.transform).ThenBy(p => sectorComponent.sectorPriority);
                //sectorComponent.localSectors.Sort((a, b) => a.sectorPriority.CompareTo(b.sectorPriority));

                //-Move generated stars from this sector to target sector
                foreach (Sector targetSector in sectorComponent.localSectors)
                {
                    if (targetSector.sectorPriority > sectorComponent.sectorPriority)
                    {
                        foreach (StarSystem star in sectorComponent.starSystems)
                        {
                            targetSector.starSystems.Add(star);
                            star.gameObject.transform.parent = targetSector.transform;
                        }
                    }
                }
                //-Destroy this sector
                //Destroy(sector.gameObject);
                Debug.Log(sector.gameObject + " marked for destruction");
                
            }
        }
        //Add sectors to list on this controller

        yield return null;
    }

    //Regeneration doesnt work
    IEnumerator GenerateStarAndSystemData(GameObject sector)
    {
        yield return new WaitForSeconds(0.1f);
        ParticleSystem sectorParticle = sector.GetComponent<ParticleSystem>();
        sectorParticle.Play();
        ParticleSystem.Particle[] generatedSector = new ParticleSystem.Particle[sectorParticle.particleCount];
        sectorParticle.GetParticles(generatedSector);
        foreach (ParticleSystem.Particle star in generatedSector)
        {
            //Needs to be local position
            GameObject starObject = Instantiate(starPrefab, star.position, sector.transform.rotation, sector.transform);
            starObject.transform.localPosition = star.position;
            //Debug.Log("Star object: " + starObject.gameObject);
            StarSystem starSystem = starObject.GetComponent<StarSystem>();
            sector.GetComponent<Sector>().starSystems.Add(starSystem);
        }
        sectorParticle.Pause();
        //Set star type
        //Enemy generation on node
        //Set number of orbiting bodies
        yield return null;
    }

    IEnumerator GenerateOrbitingBodies()
    {
        //Determine star type
        //Determine type of orbiting body
        //Determine resource and amount from orbiting body
        //Determine ease of access to resource (harvesting time)
        yield return null;
    }

    ///Next to do:
    ///-Get star data from older project
    ///--Colors, temps, likelihoods, etc
    ///--Can likely implement shadergraph, found a good tutorial
    ///
    ///-Work on sector combination
    ///-Work on star to star linkages
    ///-Populate galaxy with enemies
    ///-Create fleet and freighter interactions
    ///-Create interface
}
