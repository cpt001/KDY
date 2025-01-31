using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public KeyCode fwd = KeyCode.W;
    public KeyCode back = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode CW = KeyCode.E;
    public KeyCode CCW = KeyCode.Q;

    public float moveSpeed = 15f;
    public float rotateSpeed = 90f;
    public float zoomSpeed = 15f;
    public float dampening = 5f;


    private Vector3 _desiredPosition;
    private Vector3 _desiredRotation;

    private float minCameraDist = 5f;
    private float maxCameraDist = 50f;

    private Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
