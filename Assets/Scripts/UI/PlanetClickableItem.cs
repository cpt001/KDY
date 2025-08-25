using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetClickableItem : MonoBehaviour
{
    private GalaxyMapUI galaxyUI;
    public OrbitingBody buttonBodyAssignment;
    private void Awake()
    {
        galaxyUI = GameObject.Find("Galaxy Generation").GetComponent<GalaxyMapUI>();
    }
    public void DisplayData()
    {
        galaxyUI.PopulatePlanetReadoutPanel(buttonBodyAssignment);
    }
}
