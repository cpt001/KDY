using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryFramework;
using System.Linq;
/// <summary>
/// Classify body types into the habitability index, then randomize based on the score.
/// 
/// -Removed temperatures. Its just an entirely unnecessary layer that was overcomplicating my thought process for this system
/// -To do: 
/// -Designate each body to be generated
/// -Set spin speed
/// -Create visual element
/// </summary>
public class OrbitingBody : MonoBehaviour
{
    public enum BodyType
    {
        Terrestrial,
        Volcanic,
        Gas,
        Oceanic,
        Icy,
        Desert,
        Barren,
        ShatteredWorld,
        PlanetaryNebula,
        Derelict,
        BattleSite,
        Asteroid,
        CometField,
    }
    public BodyType bodyType;

    [Header("Body Details")]
    public int size;            //Physical size of planet. Has a seamless noise map applied to it to create illusion of terrain
    public int ring1Depth;      //
    public float ring2Depth;    //
    public float ring3Depth;    //
    public float atmoDepth;     //Sits above planet as outer layer
    public Color atmoColor;
    [SerializeField] private GameObject atmoObject => GetComponent<GameObject>().transform.Find("Atmosphere").gameObject;
    public float cloudDepth;    //Sits slightly above planet
    public Color cloudColor;
    [SerializeField] private GameObject cloudObject => GetComponent<GameObject>().transform.Find("Clouds").gameObject;
    public float seaDepth;      //Static size of planet, just a simple colored sphere
    public Color seaColor;
    [SerializeField] private GameObject seaObject => GetComponent<GameObject>().transform.Find("Ocean").gameObject;
    public Color landColor;
    [SerializeField] private GameObject landObject => GetComponent<GameObject>().transform.Find("Land").gameObject;

    [Header("Body Attributes")]
    public float orbitalSpeed;
    public float spinSpeed;
    public bool techtonicallyActive;
    public List<OrbitingBody> moons = new List<OrbitingBody>();
    public enum AtmosphericHabitability { Safe, Masked, Pressure_Suit, Vehicular, Unsafe }; public AtmosphericHabitability atmosphereHostility;          //Swapped from float, this has better data conveyance
    public enum EndemicHabitability { Intelligent_Life, Sentient, Basic_Life, Monocellular, Extinct }; public EndemicHabitability endemicHabitation;        //Native creatures
    public enum ColonizationStatus { Ecumenopolis, City, Scattered_Towns, Outposts, Expeditionary, Unexplored }; public ColonizationStatus colonization;    //
    public enum SurvivorStatus { Bunkered_Survivors, Scattered_Survivors, Escapists, Lone_Survivors, Annihilation }; public SurvivorStatus survivors;    //No chance any planet is untouched - colonization status determines full range of potential survivors

    [Header("Resources Present")]
    public Dictionary<Item, int> resourcesPresent = new Dictionary<Item, int>();

    public void GenerateBody()
    {
        //Determine body type
        size = Random.Range(1, 12);
        ring1Depth = size + 2;
        ring2Depth = ring1Depth + 1;
        ring3Depth = ring2Depth + 1;


        bodyType = (BodyType)Random.Range(0, System.Enum.GetValues(typeof(BodyType)).Length);

        switch(bodyType)
        {            
            case BodyType.Terrestrial:      //244 - 340 kelvin
                {
                    //GenPlanet
                    break;
                }
            case BodyType.Volcanic:         //1070 - 1770 kelvin
                {
                    //GenPlanet
                    break;
                }
            case BodyType.Gas:              //Pick element, set temp
                {
                    //GenPlanet
                    break;
                }
            case BodyType.Oceanic:          //Pick element, set temp
                {
                    //GenPlanet
                    break;
                }
            case BodyType.Icy:              //25-30 kelvin
                {
                    //GenPlanet
                    int atmoChance = Random.Range(0, 2);
                    break;
                }
            case BodyType.Desert:           //Any temp
                {
                    //GenPlanet
                    break;
                }
            case BodyType.Barren:           //Any temp
                {
                    //GenPlanet
                    int atmoChance = Random.Range(0, 2);
                    break;
                }
            case BodyType.ShatteredWorld:
                {
                    //SpecialGen
                    int atmoChance = Random.Range(0, 2);

                    break;
                }
            case BodyType.PlanetaryNebula:
                {
                    //SpecialGen
                    break;
                }
            case BodyType.Derelict:
                {
                    //SpecialGen
                    break;
                }
            case BodyType.BattleSite:
                {
                    //SpecialGen
                    break;
                }
            case BodyType.Asteroid:
                {
                    //SpecialGen
                    break;
                }
            case BodyType.CometField:
                {
                    //SpecialGen
                    break;
                }
        }

    }

    private IEnumerator GeneratePlanetaryBody(bool limitedColonization, bool hasAtmo, bool hasSea)
    {
        landObject.transform.localScale = new Vector3(size, size, size);
        landColor = Random.ColorHSV();
        if (hasAtmo)
        {
            atmoDepth = Random.Range(size + 0.5f, size + 2f);
            atmoObject.transform.localScale = new Vector3(atmoDepth, atmoDepth, atmoDepth);
            atmoColor = Random.ColorHSV();

            cloudDepth = (atmoDepth + size) / 2;
            cloudObject.transform.localScale = new Vector3(cloudDepth, cloudDepth, cloudDepth);
            cloudColor = Random.ColorHSV();

            atmosphereHostility = (AtmosphericHabitability)Random.Range(0, System.Enum.GetValues(typeof(AtmosphericHabitability)).Length);
            endemicHabitation = (EndemicHabitability)Random.Range(0, 5);
        }
        if (hasSea)
        {
            seaDepth = size;
        }

        switch (atmosphereHostility)
        {

            case AtmosphericHabitability.Safe:
                {
                    colonization = (ColonizationStatus)Random.Range(0, 6);
                    if (limitedColonization)
                    {
                        if (colonization != ColonizationStatus.Unexplored)
                        {
                            survivors = (SurvivorStatus)Random.Range(2, 5);
                        }
                    }
                    else
                    {
                        if (colonization != ColonizationStatus.Unexplored)
                        {
                            survivors = (SurvivorStatus)Random.Range(0, 5);
                        }
                    }

                    break;
                }
            case AtmosphericHabitability.Masked:
                {
                    colonization = (ColonizationStatus)Random.Range(1, 6);
                    if (limitedColonization)
                    {
                        if (colonization != ColonizationStatus.Unexplored)
                        {
                            survivors = (SurvivorStatus)Random.Range(3, 5);
                        }
                    }
                    else
                    {
                        if (colonization != ColonizationStatus.Unexplored)
                        {
                            survivors = (SurvivorStatus)Random.Range(0, 5);
                        }
                    }
                    break;
                }
            case AtmosphericHabitability.Pressure_Suit:
                {
                    colonization = (ColonizationStatus)Random.Range(2, 6);
                    if (colonization != ColonizationStatus.Unexplored)
                    {
                        survivors = (SurvivorStatus)Random.Range(1, 5);
                    }
                    break;
                }
            case AtmosphericHabitability.Vehicular:
                {
                    colonization = (ColonizationStatus)Random.Range(3, 6);
                    if (colonization != ColonizationStatus.Unexplored)
                    {
                        survivors = (SurvivorStatus)Random.Range(3, 5);
                    }
                    break;
                }
            case AtmosphericHabitability.Unsafe:
                {
                    colonization = (ColonizationStatus)Random.Range(4, 6);
                    if (colonization != ColonizationStatus.Unexplored)
                    {
                        survivors = (SurvivorStatus)Random.Range(4, 5);
                    }
                    break;
                }
        }

        yield return null;
    }
}
