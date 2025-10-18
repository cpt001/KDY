using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem : MonoBehaviour
{
    public GameObject lineDrawingTool;
    public GalaxyMapUI mapUI => transform.root.GetComponent<GalaxyMapUI>();
    public Sector systemSector => transform.parent.GetComponent<Sector>();
    public enum ContestationStatus
    {
        Hostile,
        Contested,
        Friendly,
        Neutral,
    }
    public ContestationStatus contestation;

    private Renderer starRenderer => GetComponent<Renderer>();
    [ColorUsage(true, true)]
    //public Color inputColor;
    private int starTemp;
    public List<FleetData> fleetsInSystem = new List<FleetData>();
    [SerializeField] private GameObject satellitePrefab;

    public enum StarType
    {
        Null,
        O,     //UV Star           -- very rare -- massive stars -- very hot -- may damage ships in system, even with shields and through armor
        B,     //Blue Luminous     -- silicon mentioned -- unusual properties of faster rotations around, maybe introduces issues with ship control?
        A,     //White/BlueWhite   -- ionized metals -- ~1 in 160
        G,     //Yellow            -- neutral metals -- ~1 in 13 -- our sun is classed a g star
        K,     //Orange            -- best chance of life -- neutral metals
        M,     //Red Dwarf         -- 76% of main sequence stars are m -- lower neutral metals, oxide visible
        L,     //Brown Dwarf       -- dark red in color -- low gravity of star -- alkali metals prominent
        C,     //Carbon star       -- nearly dead start -- high amounts of carbon from burned material present in atmosphere -- usually giants or super giants
        Neutron,    //Dead star         -- small and cold -- larger it is, more neutrino flux carried
        BlackHole,  //Dead star         -- an explosion so massive that a hole in space has replaced it -- event horizon is lethal
        Pulsar,     //Rotating star, super magnetized -- lethal to ships caught in magnetization
        Quasar,     //Supermassive black hole, with large accretion disk
        Nebula,
        Nova,   //14
    }
    public StarType typeOfStar;

    public bool allowExternalConnections;
    public int maxConnections;
    public List<StarSystem> possibleStarConnections = new List<StarSystem>();
    public List<StarSystem> starsConnected = new List<StarSystem>();
    private int maxSatelliteCount;
    public List<OrbitingBody> satellites = new List<OrbitingBody>();

    public Color starColor;

    //Sets star type based on probability matrix
    public void SetupStar(Color starInputColor) 
    {
        //This adds the system to the sector object for later usage
        systemSector.systemsInSector.Add(transform);

        //Sets star probabilities
        float starProbability = Mathf.RoundToInt(Random.Range(0, 100));
        if (starProbability == 1)
        {
            typeOfStar = StarType.O;   //.000003% chance of seeing one of these irl. Adjusted to 1%
        }
        else if (starProbability >= 2 && starProbability <= 4)
        {
            typeOfStar = StarType.B;   //.13% | 2%
        }
        else if (starProbability >= 5 && starProbability <= 9)
        {
            typeOfStar = StarType.A;   //.6% | 4%
        }
        else if (starProbability >= 10 && starProbability <= 18)
        {
            typeOfStar = StarType.G;   //7.6% | 8%
        }
        else if (starProbability >= 19 && starProbability <= 30)
        {
            typeOfStar = StarType.K;   //12.1% | 21%
        }
        else if (starProbability >= 31 && starProbability <= 72)
        {
            typeOfStar = StarType.M;   //76.5% | 41%
        }
        else if (starProbability >= 73 && starProbability <= 89)
        {
            typeOfStar = StarType.L;   //"Common" | 16%
        }
        else if (starProbability >= 90 && starProbability <= 97)
        {
            typeOfStar = StarType.C;   //70 observed   | 7%
        }
        else
        {
            int rand = Mathf.RoundToInt(Random.Range(0, 5));
            if (rand == 0)
            {
                typeOfStar = StarType.Neutron;
                contestation = ContestationStatus.Neutral;
            }
            if (rand == 1)
            {
                typeOfStar = StarType.BlackHole;
                contestation = ContestationStatus.Neutral;
            }
            if (rand == 2)
            {
                typeOfStar = StarType.Quasar;
                contestation = ContestationStatus.Neutral;
            }
            if (rand == 3)
            {
                typeOfStar = StarType.Pulsar;
                contestation = ContestationStatus.Neutral;
            }
            if (rand == 4)
            {
                typeOfStar = StarType.Nebula;
            }
            if (rand == 5)
            {
                typeOfStar = StarType.Nova;
                contestation = ContestationStatus.Neutral;
            }
        }



        StartCoroutine(SetStarParameters()); 
        //inputColor = starInputColor;

        //typeOfStar = (StarType)Random.Range(1, 13);
        ///To do: 
        ///Then star temp
        ///Then star age
        ///Then star size

    }
    //Sets the star up based on the setupstar parameter chosen
    public IEnumerator SetStarParameters()
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
            case StarType.O:   //Super massive, super hot, temp - 25000 - 50000k (89540f) = .000003% 
                {
                    SetStarScale(1.5f);
                    starTemp = Random.Range(25000, 50000);
                    //Star Element - Helium (He)
                    //Rotation rate (cell speed on material) - Fast
                    starColor = new Color(12.55f, 8.63f, 35.69f);   //UV
                    break;
                }
            case StarType.B:   //Fast rotation, silicon?, temp - 10000k to 25000k (44540f) = .13% -- No corona
                {
                    SetStarScale(1f);
                    starColor = new Color(0.0f, 87.5f, 100.0f);   //Blue Luminous
                    starTemp = Random.Range(10000, 25000);
                    //Element - Helium/Hydrogen (He/H)
                    //Rotation - Very Fast
                    break;
                }
            case StarType.A:   //ionized metals, 1/160 chance, temp - 7400k to 10000k = .6%
                {
                    SetStarScale(.75f);
                    starColor = new Color(189f, 219f, 224f);   //White/Bluewhite
                    starTemp = Random.Range(7400, 10000);
                    //Element - Helium/Hydrogen (He/H)
                    //Rotation - Very FastNR
                    break;
                }
            case StarType.G:   //neutral metals, 1/13, SOL, temp - 5000 to 6000k, hab zone - 0.9 to 1.2 AU = 7.6%
                {
                    SetStarScale(.6f);
                    starColor = new Color(255f, 255f, 0f);   //Yellow
                    starTemp = Random.Range(5000, 6000);
                    //Element - Helium/Hydrogen (He/H)
                    //Rotation - Very FastNR
                    break;
                }
            case StarType.K:   //Neutral metals, Best chance of life, temp - 3500 to 5000k, hab zone - 0.7 to 1.0 AU = 12.1%
                {
                    SetStarScale(.5f);
                    starColor = new Color(100.0f, 76.1f, 7.8f);   //Orange
                    starTemp = Random.Range(3500, 5000);
                    //Element - Hydrogen (H)
                    //Rotation - Very FastNR
                    break;
                }
            case StarType.M:   //oxide, lower neutral metals, 76% chance that star is class M, temp - 2300 to 3900k, hab zone - 0.3 au to 0.6 au = 76.5%
                {
                    SetStarScale(.4f);
                    starColor = new Color(255f, 0f, 0f);   //Red
                    starTemp = Random.Range(2300, 3900);
                    //Element - Titanium Oxide  (TiO2)
                    //Rotation - Very FastNR
                    break;
                }
            case StarType.L:   //Low gravity, alkali metals prominent, temp - 1500 to 2500k, hab zone - .007 to .044 AU (1050km - 6700) = "Common"
                {
                    SetStarScale(.3f);
                    starColor = new Color(69.8f, 22.7f, 0.0f);   //Brown
                    starTemp = Random.Range(1500, 2500);
                    //Element - Hydride Bands/Alkalide metals (FeH, CrH, MgH, CaH)/(Na, K, Rb, Cs)
                    //Rotation - Very FastNR
                    break;
                }
            case StarType.C:   //High carbon atmosphere, high alkaline metals, temp 31.15 kelvin to 3000k - "70 observed"
                {
                    SetStarScale(.3f);
                    starColor = new Color(34.1f, 11.0f, 0.0f);   //Carbon (Dark Brown/Red)
                    starTemp = Random.Range(2800, 5000);
                    //Element - Carbon (C)
                    //Rotation - Very FastNR
                    break;
                }
            case StarType.Neutron:
                {
                    starColor = new Color(0f, 0f, 139f);   //Dark blue
                    starTemp = 1000000;
                    //Element - Neutronium
                    break;
                }
            case StarType.BlackHole:
                {
                    starColor = new Color(0f, 0f, 0f);
                    starTemp = 0;
                    break;
                }
            case StarType.Quasar:
                {
                    starColor = new Color(255f, 69f, 0f);   //Red Orange
                    starTemp = 1000000000;
                    break;
                }
            case StarType.Pulsar:
                {
                    starColor = new Color(128f, 0f, 128f);   //Purple
                    starTemp = Random.Range(3000, 6200);
                    break;
                }
            case StarType.Nebula:
                {
                    starColor = new Color(128f, 0f, 128f);   //Ranges. Red, pink, green, bright blue. Can probably implement this with a cascading implementation
                    starTemp = Random.Range(0, 7000);
                    //Element: Hydrogen, Helium, Dust
                    break;
                }
            case StarType.Nova:
                {
                    starColor = new Color(255f, 255f, 255f);   //RGB
                    break;
                }
        }
        float hdrIntensityR = Mathf.Pow(2, starColor.r);
        float hdrIntensityG = Mathf.Pow(2, starColor.g);
        float hdrIntensityB = Mathf.Pow(2, starColor.b);
        Color adjustedColor = new Color(hdrIntensityR, hdrIntensityG, hdrIntensityB);
        
        starRenderer.material.SetColor("_BaseColor", starColor);
        //starRenderer.material.SetColor("_CellColor", adjustedColor);
        
        if (typeOfStar == StarType.Null || typeOfStar == StarType.Neutron || typeOfStar == StarType.Nebula || typeOfStar == StarType.Nova)
        {
            //No planets allowed
            maxSatelliteCount = 0;
            name = typeOfStar + " | T:" + starTemp + "K | C:" + transform.position;
        }
        else if (typeOfStar == StarType.BlackHole || typeOfStar == StarType.Quasar || typeOfStar == StarType.Pulsar)
        {
            //Low chance, low planet count
            int rand = Random.Range(0, 10);
            if (rand <= 2)
            {
                maxSatelliteCount = Random.Range(1, 4);
                if (maxSatelliteCount != 0)
                {
                    name = contestation.ToString() + " " + typeOfStar + " " + maxSatelliteCount + " | T:" + starTemp + "K | C:" + transform.position;
                }
                else
                {
                    name = contestation.ToString() + " " +  typeOfStar + " | T:" + starTemp + "K | C:" + transform.position;
                }
            }
        }
        else
        {
            //All other stars get planets
            maxSatelliteCount = Random.Range(0, 15);
            name = "Class " + typeOfStar + maxSatelliteCount + " | T:" + starTemp + "K | C:" + transform.position;
        }
        for (int i = 0; i < maxSatelliteCount; i++)
        {
            StartCoroutine(SpawnOrbitingBodies(i));
        }
    }



    void SetStarScale(float TargetSize)
    {
        transform.localScale = new Vector3(TargetSize, TargetSize, TargetSize);
        //Already have color, temp

    }
    /// <summary>
    /// Start working on display aspect, starting with UI
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpawnOrbitingBodies(int iCount)
    {
        int randMoons = Random.Range(0, 8);
        GameObject satellite = Instantiate(satellitePrefab, transform);
        satellite.GetComponent<OrbitingBody>().GenerateBody();
        satellites.Add(satellite.GetComponent<OrbitingBody>());
        satellite.GetComponent<OrbitingBody>().satelliteOrbitalOrder = iCount;

        for (int i = 0; i < randMoons; i++)
        {
            GameObject moon = Instantiate(satellitePrefab, satellite.transform);
            satellite.GetComponent<OrbitingBody>().GenerateBody();
            satellite.GetComponent<OrbitingBody>().moons.Add(moon.GetComponent<OrbitingBody>());
        }

        yield return null;
    }

    public void StartConnectBody(StarSystem starTarget)
    {
        StartCoroutine(ConnectBody(starTarget));
    }

    public IEnumerator ConnectBody(StarSystem target)
    {
        /*target.starsConnected.Add(this);
        //Generate a line renderer from this star to target star
        if (!gameObject.GetComponent<LineRenderer>())
        {
            LineRenderer starLine = gameObject.AddComponent<LineRenderer>();
            starLine.startWidth = 0.3f;
            starLine.endWidth = 0.1f;
            starLine.SetPosition(0, transform.position);
            starLine.SetPosition(1, target.transform.position);
        }
        else
        {
            LineRenderer starLine = gameObject.GetComponent<LineRenderer>();
            //This throws an out of bounds warning?
            starLine.SetPosition(2, target.transform.position);
            starLine.SetPosition(3, target.transform.position);
        }*/

        /*yield return new WaitForSeconds(0.2f);

        if (possibleStarConnections.Count != 0)
        {
            LineRenderer starLine = gameObject.AddComponent<LineRenderer>();
            starLine.SetPosition(0, transform.position);
            starLine.startWidth = 0.2f;
            starLine.endWidth = 0.1f;
            //if this method doesn't work, spawn subobjects with each target selected
            for (int i = 0; i > possibleStarConnections.Count; i++)
            {
                starLine.SetPosition(i, possibleStarConnections[i].transform.position);
            }
        }*/
        GameObject starlineObject = Instantiate(lineDrawingTool,transform.position, Quaternion.identity, this.transform);
        LineRenderer starLine = starlineObject.GetComponent<LineRenderer>();
        starLine.SetPosition(0, gameObject.transform.position);
        starLine.SetPosition(1, target.transform.position);
        switch (contestation)
        {
            case ContestationStatus.Hostile:
                {
                    starLine.startColor = Color.red;
                    if (target.contestation == ContestationStatus.Hostile)
                    {
                        starLine.endColor = Color.red;
                    }
                    if (target.contestation == ContestationStatus.Contested)
                    {
                        starLine.endColor = Color.yellow;
                    }
                    if (target.contestation == ContestationStatus.Friendly)
                    {
                        starLine.endColor = Color.cyan;
                    }
                    if (target.contestation == ContestationStatus.Neutral)
                    {
                        starLine.endColor = Color.white;
                    }
                    break;
                }
            case ContestationStatus.Friendly:
                {
                    starLine.startColor = Color.cyan;
                    if (target.contestation == ContestationStatus.Hostile)
                    {
                        starLine.endColor = Color.red;
                    }
                    if (target.contestation == ContestationStatus.Contested)
                    {
                        starLine.endColor = Color.yellow;
                    }
                    if (target.contestation == ContestationStatus.Friendly)
                    {
                        starLine.endColor = Color.cyan;
                    }
                    if (target.contestation == ContestationStatus.Neutral)
                    {
                        starLine.endColor = Color.white;
                    }
                    break;
                }
            case ContestationStatus.Contested:
                {
                    starLine.startColor = Color.yellow;
                    if (target.contestation == ContestationStatus.Hostile)
                    {
                        starLine.endColor = Color.red;
                    }
                    if (target.contestation == ContestationStatus.Contested)
                    {
                        starLine.endColor = Color.blue;
                    }
                    if (target.contestation == ContestationStatus.Friendly)
                    {
                        starLine.endColor = Color.cyan;
                    }
                    if (target.contestation == ContestationStatus.Neutral)
                    {
                        starLine.endColor = Color.white;
                    }
                    break;
                }
            case ContestationStatus.Neutral:
                {
                    starLine.startColor = Color.white;
                    if (target.contestation == ContestationStatus.Hostile)
                    {
                        starLine.endColor = Color.red;
                    }
                    if (target.contestation == ContestationStatus.Contested)
                    {
                        starLine.endColor = Color.blue;
                    }
                    if (target.contestation == ContestationStatus.Friendly)
                    {
                        starLine.endColor = Color.cyan;
                    }
                    if (target.contestation == ContestationStatus.Neutral)
                    {
                        starLine.endColor = Color.white;
                    }
                    break;
                }
        }
        yield return null;
    }

    public void OnMouseDown()
    {
        mapUI.PopulateClickedUI(this);
    }

    public void OnMouseOver()
    {
        mapUI.PopulateHoverUI(this);
    }

    public void OnMouseExit()
    {
        mapUI.DepopulateHoverUI();
    }
}
