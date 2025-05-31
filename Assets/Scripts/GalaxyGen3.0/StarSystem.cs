using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem : MonoBehaviour
{
    private Renderer starRenderer => GetComponent<Renderer>();
    [ColorUsage(true, true)]
    //public Color inputColor;
    private int starTemp;
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
        Nebula,
        Nova,   //14
    }
    public StarType typeOfStar;

    public List<StarSystem> connectedStars = new List<StarSystem>();
    public List<OrbitingBody> satellites = new List<OrbitingBody>();

    public Color starColor;

    public void SetupStar(Color starInputColor) 
    {
        //Sets star probabilities
        float starProbability = Mathf.RoundToInt(Random.Range(0, 100));
        if (starProbability == 1)
        {
            typeOfStar = StarType.ClassO;   //.000003% chance of seeing one of these irl. Adjusted to 1%
        }
        else if (starProbability >= 2 && starProbability <= 4)
        {
            typeOfStar = StarType.ClassB;   //.13% | 2%
        }
        else if (starProbability >= 5 && starProbability <= 9)
        {
            typeOfStar = StarType.ClassA;   //.6% | 4%
        }
        else if (starProbability >= 10 && starProbability <= 18)
        {
            typeOfStar = StarType.ClassG;   //7.6% | 8%
        }
        else if (starProbability >= 19 && starProbability <= 30)
        {
            typeOfStar = StarType.ClassK;   //12.1% | 21%
        }
        else if (starProbability >= 31 && starProbability <= 72)
        {
            typeOfStar = StarType.ClassM;   //76.5% | 41%
        }
        else if (starProbability >= 73 && starProbability <= 89)
        {
            typeOfStar = StarType.ClassL;   //"Common" | 16%
        }
        else if (starProbability >= 90 && starProbability <= 97)
        {
            typeOfStar = StarType.ClassC;   //70 observed   | 7%
        }
        else
        {
            int rand = Mathf.RoundToInt(Random.Range(0, 5));
            if (rand == 0)
            {
                typeOfStar = StarType.Neutron;
            }
            if (rand == 1)
            {
                typeOfStar = StarType.BlackHole;
            }
            if (rand == 2)
            {
                typeOfStar = StarType.Quasar;
            }
            if (rand == 3)
            {
                typeOfStar = StarType.Pulsar;
            }
            if (rand == 4)
            {
                typeOfStar = StarType.Nebula;
            }
            if (rand == 5)
            {
                typeOfStar = StarType.Nova;
            }
        }



        StartCoroutine(SetStarColor()); 
        //inputColor = starInputColor;

        //typeOfStar = (StarType)Random.Range(1, 13);
        ///To do: 
        ///Then star temp
        ///Then star age
        ///Then star size

    }

    public IEnumerator SetStarColor()
    {
        yield return new WaitForSeconds(0.1f);
        //Debug.Log("Star Input color " + inputColor);

        switch (typeOfStar)
        {
            case StarType.Null:
                {
                    starColor = new Color(0f, 255f, 10f);   //Debug green
                    Debug.Log("Star @" + transform.name + " is null");
                    break;
                }
            case StarType.ClassO:   //Super massive, super hot, temp - 25000 - 50000k (89540f) = .000003% 
                {
                    SetStarScale(1.5f);
                    starTemp = Random.Range(25000, 50000);
                    //Star Element - Helium (He)
                    //Rotation rate (cell speed on material) - Fast
                    starColor = new Color(12.55f, 8.63f, 35.69f);   //UV
                    break;
                }
            case StarType.ClassB:   //Fast rotation, silicon?, temp - 10000k to 25000k (44540f) = .13% -- No corona
                {
                    SetStarScale(1f);
                    starColor = new Color(0.0f, 87.5f, 100.0f);   //Blue Luminous
                    starTemp = Random.Range(10000, 25000);
                    //Element - Helium/Hydrogen (He/H)
                    //Rotation - Very Fast
                    break;
                }
            case StarType.ClassA:   //ionized metals, 1/160 chance, temp - 7400k to 10000k = .6%
                {
                    SetStarScale(.75f);
                    starColor = new Color(189f, 219f, 224f);   //White/Bluewhite
                    starTemp = Random.Range(7400, 10000);
                    //Element - Helium/Hydrogen (He/H)
                    //Rotation - Very FastNR
                    break;
                }
            case StarType.ClassG:   //neutral metals, 1/13, SOL, temp - 5000 to 6000k, hab zone - 0.9 to 1.2 AU = 7.6%
                {
                    SetStarScale(.6f);
                    starColor = new Color(255f, 255f, 0f);   //Yellow
                    starTemp = Random.Range(5000, 6000);
                    //Element - Helium/Hydrogen (He/H)
                    //Rotation - Very FastNR
                    break;
                }
            case StarType.ClassK:   //Neutral metals, Best chance of life, temp - 3500 to 5000k, hab zone - 0.7 to 1.0 AU = 12.1%
                {
                    SetStarScale(.5f);
                    starColor = new Color(100.0f, 76.1f, 7.8f);   //Orange
                    starTemp = Random.Range(3500, 5000);
                    //Element - Hydrogen (H)
                    //Rotation - Very FastNR
                    break;
                }
            case StarType.ClassM:   //oxide, lower neutral metals, 76% chance that star is class M, temp - 3000k, hab zone - 0.3 au to 0.6 au = 76.5%
                {
                    SetStarScale(.4f);
                    starColor = new Color(255f, 0f, 0f);   //Red
                    //Temp - ~30000
                    //Element - Titanium Oxide  (TiO2)
                    //Rotation - Very FastNR
                    break;
                }
            case StarType.ClassL:   //Low gravity, alkali metals prominent, temp - 1500 to 2500k, hab zone - .007 to .044 AU (1050km - 6700) = "Common"
                {
                    SetStarScale(.3f);
                    starColor = new Color(69.8f, 22.7f, 0.0f);   //Brown
                    //Temp - 1500, 2500
                    //Element - Hydride Bands/Alkalide metals (FeH, CrH, MgH, CaH)/(Na, K, Rb, Cs)
                    //Rotation - Very FastNR
                    break;
                }
            case StarType.ClassC:   //High carbon atmosphere, high alkaline metals, temp 31.15 kelvin to 3000k - "70 observed"
                {
                    SetStarScale(.3f);
                    starColor = new Color(34.1f, 11.0f, 0.0f);   //Carbon (Dark Brown/Red)
                    //Temp - 3000
                    //Element - Carbon (C)
                    //Rotation - Very FastNR
                    break;
                }
            case StarType.Neutron:
                {
                    starColor = new Color(0f, 0f, 139f);   //Dark blue
                    //Element - Neutronium
                    break;
                }
            case StarType.BlackHole:
                {
                    starColor = new Color(0f, 0f, 0f);
                    break;
                }
            case StarType.Quasar:
                {
                    starColor = new Color(255f, 69f, 0f);   //Red Orange
                    break;
                }
            case StarType.Pulsar:
                {
                    starColor = new Color(128f, 0f, 128f);   //Purple
                    break;
                }
            case StarType.Nebula:
                {
                    starColor = new Color(128f, 0f, 128f);   //Ranges. Red, pink, green, bright blue. Can probably implement this with a cascading implementation
                    //Element: Hydrogen, Helium, Dust
                    break;
                }
            case StarType.Nova:
                {
                    starColor = new Color(255f, 255f, 255f);   //RGB
                    break;
                }
        }
        /*float hdrIntensityR = Mathf.Pow(2, inputColor.r);
        float hdrIntensityG = Mathf.Pow(2, inputColor.g);
        float hdrIntensityB = Mathf.Pow(2, inputColor.b);
        Color adjustedColor = new Color(hdrIntensityR, hdrIntensityG, hdrIntensityB);
        */
        starRenderer.material.SetColor("_BaseColor", starColor);
        //starRenderer.material.SetColor("_CellColor", adjustedColor);
        
    }

    void SetStarScale(float TargetSize)
    {
        transform.localScale = new Vector3(TargetSize, TargetSize, TargetSize);

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
