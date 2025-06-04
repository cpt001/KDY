using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryFramework;

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
        Rogue,
        ShatteredWorld,
        PlanetaryNebula,
        Derelict,
        BattleSite,
        Asteroid,
        CometField,
    }

    [Header("Body Details")]
    public float size;
    public float ring1Depth;
    public float ring2Depth;
    public float ring3Depth;
    public float atmoDepth;
    public float cloudDepth;
    public float seaDepth;

    [Header("Body Attributes")]
    public float bodyTemperature;
    public float orbitalSpeed;
    public float spinSpeed;
    public bool techtonicallyActive;
    public bool isInGoldilocksZone;
    public List<OrbitingBody> moons = new List<OrbitingBody>();

    [Header("Classification Index")]
    public float Habitability;          //1 - safe, 2 - mask/covered skin, 3 - full body suit, 4 - vehicle required, 5 - unsafe
    public float EndemicHabitability;   //1 - intelligent life, 2 - sentient, 3 - basic life/trees, 4 - monocellular, 5 - dead
    public float ColonizationStatus;    //1 - ecumenopolis, 2 - city, 3 - scattered towns, 4 - outposts, 5 - expiditionary, 6 - unexplored

    [Header("Resources Present")]
    public Dictionary<Item, int> resourcesPresent = new Dictionary<Item, int>();

    public void GenerateBody()
    {
        //Determine body type
    }
}
