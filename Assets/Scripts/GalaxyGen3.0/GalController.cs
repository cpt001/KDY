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
            case (1): { turnFraction = 0.99f; break; }
            case (2): { turnFraction = 0.509f; break; }
            case (3): { turnFraction = 0.3300575f; break; }
            case (4): { turnFraction = 0.2450275f; break; }
            case (5): { turnFraction = 0.195001f; break; }
            case (6): { turnFraction = 0.1700575f; break; }
            case (7): { turnFraction = 0.14001f; break; }
            case (8): { turnFraction = 0.1230575f; break; }
            case (9): { turnFraction = 0.113001f; break; }
            case (10): { turnFraction = .0990001f; break; }
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
            ///Can i skip generation on the first % of the sectors?
            ///Mark 3rd and 4th from last sectors as being capital sectors. This should put them on opposing sides of the galaxy, but not on the very edge 
        }
        #endregion
        //Determines size of deadzone at galactic center, and removes sectors
        #region Deadzone removal
        switch (deadzone)
        {
            case GalacticCenterDeadZone.Small: { deadZoneCollisionRadius = 10.0f; break; }
            case GalacticCenterDeadZone.Medium: { deadZoneCollisionRadius = 20.0f; break; }
            case GalacticCenterDeadZone.Large: { deadZoneCollisionRadius = 30.0f; break; }
        }

        RaycastHit[] deadzoneObjects = Physics.SphereCastAll(gameObject.transform.position, deadZoneCollisionRadius, Vector3.left, Mathf.Infinity, sectorLayerMask, QueryTriggerInteraction.Collide);
        foreach (RaycastHit sector in deadzoneObjects)
        {
            Destroy(sector.transform.gameObject);
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
                //Mark for combination to another sector.
                sectorComponent.markForCombination = true;
                //-Set target sector (nearest sector over priority)
                sectorComponent.localSectors = sectorsInGalaxy;
                //This is currently sorting just by priority... it also needs to take transform locality into account
                sectorComponent.localSectors.OrderBy(t => t.transform).ThenBy(p => sectorComponent.sectorPriority);
                //sectorComponent.localSectors.Sort((a, b) => a.sectorPriority.CompareTo(b.sectorPriority));

                //-Move generated stars from this sector to target sector
                /*foreach (Sector targetSector in sectorComponent.localSectors)
                {
                    if (targetSector.sectorPriority > sectorComponent.sectorPriority)
                    {
                        foreach (StarSystem star in sectorComponent.starSystems)
                        {
                            targetSector.starSystems.Add(star);
                        }
                    }
                }
                //-Destroy this sector
                Destroy(sector.gameObject);
                */
            }
        }
        //Add sectors to list on this controller

        yield return null;
    }

    //The bug was my not parenting correctly, then not remembering that the individual stars lack a visual component for the moment
    //Regeneration doesnt work
    IEnumerator GenerateStarAndSystemData(GameObject sector)
    {
        yield return new WaitForSeconds(0.1f);
        ParticleSystem sectorParticle = sector.GetComponent<ParticleSystem>();
        sectorParticle.Play();
        Debug.Log(sectorParticle + " should populate with gameobjects");
        ParticleSystem.Particle[] generatedSector = new ParticleSystem.Particle[sectorParticle.particleCount];
        sectorParticle.GetParticles(generatedSector);
        foreach (ParticleSystem.Particle star in generatedSector)
        {
            //Each star is being found, but objects arent being made?
            GameObject starObject = Instantiate(starPrefab, star.position, transform.rotation, sector.transform);
            Debug.Log("Star object: " + starObject.gameObject);
            StarSystem starSystem = starObject.GetComponent<StarSystem>();
            sector.GetComponent<Sector>().starSystems.Add(starSystem);
        }

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
    ///-Work on sector combination
    ///-Create orbiting body model from scrapped planets in original model
    ///-Create fleet and freighter interactions
    ///-Create interface
}
