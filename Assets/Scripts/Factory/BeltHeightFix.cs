using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltHeightFix : MonoBehaviour
{
    private void Start()
    {
        Vector3 oldPos = transform.Find("Belt").position;
        transform.Find("Belt").position = new Vector3(oldPos.x, 0.27f, oldPos.z);
    }
}
