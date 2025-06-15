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
        Rogue,
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
    public float cloudDepth;    //Sits slightly above planet
    public float seaDepth;      //Static size of planet, just a simple colored sphere

    [Header("Body Attributes")]
    public float bodyTemperature;
    public float orbitalSpeed;
    public float spinSpeed;
    public bool techtonicallyActive;
    public List<OrbitingBody> moons = new List<OrbitingBody>();

    [Header("Classification Index")]
    public float Habitability;          //1 - safe (Earth), 2 - mask/covered skin (Mars), 3 - pressurized suit (Moon), 4 - vehicle required (Mustafar), 5 - unsafe (Shattered World)
    public float EndemicHabitability;   //1 - intelligent life, 2 - sentient, 3 - basic life/trees, 4 - monocellular, 5 - dead
    public float ColonizationStatus;    //1 - ecumenopolis, 2 - city, 3 - scattered towns, 4 - outposts, 5 - expiditionary, 6 - unexplored

    [Header("Resources Present")]
    public Dictionary<Item, int> resourcesPresent = new Dictionary<Item, int>();

    public void GenerateBody()
    {
        //Determine body type
        size = Random.Range(1, 12);
        ring1Depth = size + 2;
        ring2Depth = ring1Depth + 1;
        ring3Depth = ring2Depth + 1;

        atmoDepth = Random.Range(size + 0.5f, size + 2f);
        cloudDepth = (atmoDepth + size) / 2;
        seaDepth = size;


        bodyType = (BodyType)Random.Range(0, System.Enum.GetValues(typeof(BodyType)).Length);

    }
}
