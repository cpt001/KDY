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
    public bool clickUIActive;
    public TextMeshProUGUI StarClickData;

    public void PopulateHoverUI(StarSystem targetStar)
    {
        if (targetStar.typeOfStar == StarSystem.StarType.Neutron ||
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
        }
    }
    public void PopulateClickedUI(StarSystem targetStar)
    {

    }
}
