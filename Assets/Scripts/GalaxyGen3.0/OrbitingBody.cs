using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryFramework;
using System.Linq;
/// <summary>
/// Classify body types into the habitability index, then randomize based on the score.
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
    public float cloudDepth;    //Sits slightly above planet
    public Color cloudColor;
    public float seaDepth;      //Static size of planet, just a simple colored sphere
    public Color seaColor;

    [Header("Body Attributes")]
    public int bodyTemperature;
    public float orbitalSpeed;
    public float spinSpeed;
    public bool techtonicallyActive;
    public List<OrbitingBody> moons = new List<OrbitingBody>();
    public enum AtmosphericHabitability { Safe, Masked, Pressure_Suit, Vehicular, Unsafe }; public AtmosphericHabitability atmosphereHostility;          //Swapped from float, this has better data conveyance
    public enum EndemicHabitability { Intelligent_Life, Sentient, Basic_Life, Monocellular, Extinct }; public EndemicHabitability endemicHabitation;
    public enum ColonizationStatus { Ecumenopolis, City, Scattered_Towns, Outposts, Expeditionary, Unexplored }; public ColonizationStatus colonization;    
    public enum SurvivorStatus { Bunkered_Survivors, Scattered_Survivors, Escapists, LoneSurvivors, TotalDestruction }; public SurvivorStatus survivors;    //No chance any planet is untouched - colonization status determines full range of potential survivors

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
            case BodyType.Terrestrial:
                {
                    //GenPlanet
                    break;
                }
            case BodyType.Volcanic:
                {
                    //GenPlanet
                    break;
                }
            case BodyType.Gas:
                {
                    //GenPlanet
                    break;
                }
            case BodyType.Oceanic:
                {
                    //GenPlanet
                    break;
                }
            case BodyType.Icy:
                {
                    //GenPlanet
                    break;
                }
            case BodyType.Desert:
                {
                    //GenPlanet
                    break;
                }
            case BodyType.Barren:
                {
                    //GenPlanet
                    break;
                }
            case BodyType.ShatteredWorld:
                {
                    //SpecialGen
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

    private IEnumerator GeneratePlanetaryBody(BodyType bodyType, bool hasAtmo, bool hasSea)
    {
        if (hasAtmo)
        {
            atmoDepth = Random.Range(size + 0.5f, size + 2f);
            cloudDepth = (atmoDepth + size) / 2;
            atmosphereHostility = (AtmosphericHabitability)Random.Range(0, System.Enum.GetValues(typeof(AtmosphericHabitability)).Length);
        }
        if (hasSea)
        {
            seaDepth = size;
        }

        //Determine temperature range based on the habitability || Determine habitability based on the temperature? -- Might be better to determine the body type based on temps instead
        switch (atmosphereHostility)
        {            
            case AtmosphericHabitability.Safe:
                {
                    switch (bodyType)
                    {                        
                        case BodyType.Terrestrial:
                            {
                                bodyTemperature = Random.Range(-40, 150);
                                break;
                            }
                        case BodyType.Volcanic:
                            {
                                break;
                            }
                        case BodyType.Gas:
                            {
                                break;
                            }
                        case BodyType.Oceanic:
                            {
                                break;
                            }
                        case BodyType.Icy:
                            {
                                break;
                            }
                        case BodyType.Desert:
                            {
                                break;
                            }
                        case BodyType.Barren:
                            {
                                break;
                            }
                    }
                    break;
                }
            case AtmosphericHabitability.Masked:
                {
                    switch (bodyType)
                    {
                        case BodyType.Terrestrial:
                            {
                                bodyTemperature = Random.Range(-40, 150);
                                break;
                            }
                        case BodyType.Volcanic:
                            {
                                break;
                            }
                        case BodyType.Gas:
                            {
                                break;
                            }
                        case BodyType.Oceanic:
                            {
                                break;
                            }
                        case BodyType.Icy:
                            {
                                break;
                            }
                        case BodyType.Desert:
                            {
                                break;
                            }
                        case BodyType.Barren:
                            {
                                break;
                            }
                    }
                    break;
                }
            case AtmosphericHabitability.Pressure_Suit:
                {
                    switch (bodyType)
                    {
                        case BodyType.Terrestrial:
                            {
                                bodyTemperature = Random.Range(-100, 200);
                                break;
                            }
                        case BodyType.Volcanic:
                            {
                                break;
                            }
                        case BodyType.Gas:
                            {
                                break;
                            }
                        case BodyType.Oceanic:
                            {
                                break;
                            }
                        case BodyType.Icy:
                            {
                                break;
                            }
                        case BodyType.Desert:
                            {
                                break;
                            }
                        case BodyType.Barren:
                            {
                                break;
                            }
                    }
                    break;
                }
            case AtmosphericHabitability.Vehicular:
                {
                    switch (bodyType)
                    {
                        case BodyType.Terrestrial:
                            {
                                bodyTemperature = Random.Range(-250, 250);
                                break;
                            }
                        case BodyType.Volcanic:
                            {
                                break;
                            }
                        case BodyType.Gas:
                            {
                                break;
                            }
                        case BodyType.Oceanic:
                            {
                                break;
                            }
                        case BodyType.Icy:
                            {
                                break;
                            }
                        case BodyType.Desert:
                            {
                                break;
                            }
                        case BodyType.Barren:
                            {
                                break;
                            }
                    }
                    break;
                }
            case AtmosphericHabitability.Unsafe:
                {
                    switch (bodyType)
                    {
                        case BodyType.Terrestrial:
                            {
                                bodyTemperature = Random.Range(-400, 400);
                                break;
                            }
                        case BodyType.Volcanic:
                            {
                                break;
                            }
                        case BodyType.Gas:
                            {
                                break;
                            }
                        case BodyType.Oceanic:
                            {
                                break;
                            }
                        case BodyType.Icy:
                            {
                                break;
                            }
                        case BodyType.Desert:
                            {
                                break;
                            }
                        case BodyType.Barren:
                            {
                                break;
                            }
                    }
                    break;
                }
        }

        yield return null;
    }
}
