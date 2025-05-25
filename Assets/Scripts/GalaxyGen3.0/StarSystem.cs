using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem : MonoBehaviour
{
    public List<FleetData> fleetsInSystem = new List<FleetData>();

    public enum StarType
    {
        Null,
        ClassO,     //UV Star           -- very rare -- massive stars -- very hot -- may damage ships in system, even with shields and through armor
        ClassB,     //Blue Luminous     -- silicon mentioned -- unusual properties of faster rotations around, maybe introduces issues with ship control?
        ClassA,     //White/BlueWhite   -- ionized metals -- ~1 in 160
        ClassG,     //Yellow            -- neutral metals -- ~1 in 13 -- our sun is classed a g star
        ClassK,     //Orange            -- best chance of life -- neutral metals
        ClassM,     //Red Dwarf         -- 76% of main sequence stars are m -- lower neutral metals, oxide visible
        ClassL,     //Brown Dwarf       -- dark red in color -- low gravity of star -- alkali metals prominent
        ClassC,     //Carbon star       -- nearly dead start -- high amounts of carbon from burned material present in atmosphere -- usually giants or super giants
        Neutron,    //Dead star         -- small and cold -- larger it is, more neutrino flux carried
        BlackHole,  //Dead star         -- an explosion so massive that a hole in space has replaced it -- event horizon is lethal
        Pulsar,     //Rotating star, super magnetized -- lethal to ships caught in magnetization
        Quasar,     //Supermassive black hole, with large accretion disk

        Nova,   //14
    }
    public StarType typeOfStar;

    public List<StarSystem> connectedStars = new List<StarSystem>();
    public List<OrbitingBody> satellites = new List<OrbitingBody>();

    public Color starColor;

    public void SetupStar()
    {
        switch (typeOfStar)
        {
            case StarType.Null:
                {
                    starColor = new Color(0f, 255f, 10f);   //Debug green
                    Debug.Log("Star @" + transform.name + " is null");
                    break;
                }
            case StarType.ClassO:   //Super massive, super hot, temp - 25000 - 50000k (89540f)
                {
                    starColor = new Color(12.55f, 8.63f, 35.69f);   //UV
                    break;
                }
            case StarType.ClassB:   //Fast rotation, silicon?, temp - 10000k to 25000k (44540f)
                {
                    starColor = new Color(0.0f, 87.5f, 100.0f);   //Blue Luminous
                    break;
                }
            case StarType.ClassA:   //ionized metals, 1/160 chance, temp - 7400k to 10000k
                {
                    starColor = new Color(80.0f, 97.3f, 100.0f);   //White/Bluewhite
                    break;
                }
            case StarType.ClassG:   //neutral metals, 1/13, SOL, temp - 5000 to 6000k, hab zone - 0.9 to 1.2 AU
                {
                    starColor = Color.yellow;   //Yellow
                    break;
                }
            case StarType.ClassK:   //Neutral metals, Best chance of life, temp - 3500 to 5000k, hab zone - 0.7 to 1.0 AU
                {
                    starColor = new Color(100.0f, 76.1f, 7.8f);   //Orange
                    break;
                }
            case StarType.ClassM:   //oxide, lower neutral metals, 76% chance that star is class M, temp - 3000k, hab zone - 0.3 au to 0.6 au
                {
                    starColor = Color.red;   //Red
                    break;
                }
            case StarType.ClassL:   //Low gravity, alkali metals prominent, temp - 1500 to 2500k, hab zone - .007 to .044 AU (1050km - 6700)
                {
                    starColor = new Color(69.8f, 22.7f, 0.0f);   //Brown
                    break;
                }
            case StarType.ClassC:   //High carbon atmosphere, high alkaline metals, temp 31.15 kelvin to 3000k
                {
                    starColor = new Color(34.1f, 11.0f, 0.0f);   //Carbon (Dark Brown)
                    break;
                }
            case StarType.Neutron:
                {
                    starColor = Color.grey;   //Dark blue
                    break;
                }
            case StarType.BlackHole:
                {
                    starColor = Color.black;
                    break;
                }
            case StarType.Quasar:
                {
                    starColor = Color.grey;   //Red Orange
                    break;
                }
            case StarType.Pulsar:
                {
                    starColor = Color.grey;   //Purple
                    break;
                }
            case StarType.Nova:
                {
                    starColor = Color.white;   //RGB
                    break;
                }
        }
    }

    public void OnMouseDown()
    {
        
    }

    public void OnMouseOver()
    {
        
    }

    public void OnMouseExit()
    {
        
    }
}
