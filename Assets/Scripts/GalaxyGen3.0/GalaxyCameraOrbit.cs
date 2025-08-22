using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyCameraOrbit : MonoBehaviour
{
    public Transform currentFocus;
    private float rotationSpeed = 500f;

    private void Update()
    {
        //Retarget to system
        if (Input.GetMouseButtonDown(0))
        {
            
        }
        //Orbit target
        if (Input.GetMouseButton(1))
        {
            CamOrbit();
        }
        //Zoom
    }

    private void CamOrbit()
    {
        if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
        {
            float vertInput = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            float horiInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.right, vertInput);

        }

    }

}
