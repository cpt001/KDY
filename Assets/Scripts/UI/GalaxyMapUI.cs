using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GalaxyMapUI : MonoBehaviour
{
    //Hover info
    //EG: "NAME" | Hostile | Type M
    //public TextMeshProUGUI PlayerSetName;
    public TextMeshProUGUI StarGlanceData;

    //Clicked UI
    public GameObject largeSystemDisplayPanel;
    public GameObject planetDisplayContainer;
    public TextMeshProUGUI StarClickData;

    //Planet Readout Panel
    public GameObject planetReadoutPanel;
    public TextMeshProUGUI bodyName;
    public TextMeshProUGUI atmoText;
    public TextMeshProUGUI endemicText;
    public TextMeshProUGUI colonizationText;
    public TextMeshProUGUI survivorText;


    public void PopulateHoverUI(StarSystem targetStar)
    {
        StarGlanceData.text = targetStar.name;
        /*if (targetStar.typeOfStar == StarSystem.StarType.Neutron ||
            targetStar.typeOfStar == StarSystem.StarType.BlackHole ||
            targetStar.typeOfStar == StarSystem.StarType.Pulsar ||
            targetStar.typeOfStar == StarSystem.StarType.Quasar ||
            targetStar.typeOfStar == StarSystem.StarType.Nebula ||
            targetStar.typeOfStar == StarSystem.StarType.Nova)
        {
            StarGlanceData.text = targetStar.contestation.ToString() + " | " + targetStar.typeOfStar.ToString() + " | @" + targetStar.transform;
        }
        else
        {
            StarGlanceData.text = targetStar.contestation.ToString() + " | Type " + targetStar.typeOfStar.ToString() + " | @" + targetStar.transform;
        }*/
    }
    public void DepopulateHoverUI()
    {
        StarGlanceData.text = "Waiting for star data...";
        
    }
    //Only works if UI is closed and reopened, cannot currently reset itself.
    public void PopulateClickedUI(StarSystem targetStar)
    {
        //Resets the planet display
        foreach (Transform t in planetDisplayContainer.transform)
        {
            t.gameObject.SetActive(false);
        }
        largeSystemDisplayPanel.SetActive(true);
        //Get each satellite under that star. Activate, redesignate info, set visual data
        //This isnt getting the data correctly. 
        Debug.Log(targetStar + " | "+ targetStar.satellites.Count);
        for (int i = 0; i < targetStar.satellites.Count; i++)
        {
            GameObject planetDisplayObject = planetDisplayContainer.transform.GetChild(i).gameObject;
            planetDisplayObject.SetActive(true);
            planetDisplayObject.GetComponentInChildren<TextMeshProUGUI>().text = targetStar.satellites[i].name;
            planetDisplayObject.GetComponent<PlanetClickableItem>().buttonBodyAssignment = targetStar.satellites[i];
        }
    }
    public void CloseClickUI()
    {

        largeSystemDisplayPanel.SetActive(false);
        planetReadoutPanel.SetActive(false);
    }

    //Doesn't seem to refresh
    public void PopulatePlanetReadoutPanel(OrbitingBody body)
    {
        //name, atmo, endemic life, colonization, survivors, resources
        if (!planetReadoutPanel.activeInHierarchy)
        {
            planetReadoutPanel.SetActive(true);
        }
        bodyName.text = body.name;
        atmoText.text = body.atmosphereHostility.ToString();
        endemicText.text = body.endemicHabitation.ToString();
        colonizationText.text = body.colonization.ToString();
        survivorText.text = body.survivors.ToString();
    }
}
