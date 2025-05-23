using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingBody : MonoBehaviour
{
    [Header("Body Details")]
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
}
