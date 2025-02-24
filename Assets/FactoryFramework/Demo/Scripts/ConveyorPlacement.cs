using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FactoryFramework;
using System.Linq;
using System;
using UnityEngine.Splines;
using Unity.VisualScripting;

public class ConveyorPlacement : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer;
    public float snapRadius = 1f;
    public float heightOffset = 0.25f;

    [Header("Placement Events")]
    public VoidEventChannel_SO startPlacementEvent;
    public VoidEventChannel_SO finishPlacementEvent;
    public VoidEventChannel_SO cancelPlacementEvent;

    [Header("Conveyor Setup")]
    public ConveyorBelt conveyorPrefab;
    public BeltMeshSO frameBM;
    public BeltMeshSO beltBM;
    private ConveyorBelt _current;
    public bool IsCreatingPath => _current != null;
    private GlobalLogisticsSettings _settings;
    private ConveyorBelt _dummyVisual;

    private float startHeight;
    private List<Vector3> Points;
    private LogisticComponent _from;
    private LogisticComponent _to;
    private int _fromIndex;
    private int _toIndex;

    [Header("Visual Feedback Materials")]
    public Material originalFrameMat;
    public Material originalBeltMat;
    public Material greenGhostMat;
    public Material redGhostMat;

    // Controls
    private const float LONG_PRESS_DURATION = 0.5f;
    private bool wasPointerDown;
    private bool isLongPressTriggered;
    private float pointerDownTime;
    private Vector2 pointerPosition;

    public event Action<Vector2> OnTap;
    public event Action<Vector2> OnLongPress;
    public event Action<Vector2> OnRelease;

    [Header("Custom Functions")]
    [SerializeField] private GridCheck gridCheck;

    private void Awake()
    {
        _settings = ConveyorLogisticsUtils.settings;
    }

    bool IsPointerDown()
    {
        return Input.GetMouseButton(0) || (
            Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    }
    bool isPointerReleased()
    {
       return Input.GetMouseButtonUp(0) || (
            Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
    }
    Vector2 GetPointerPosition()
    {
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).position;
        } else
        {
            return Input.mousePosition;
        }
    }
    void TapDetected(Vector2 pos)
    {
        OnTap?.Invoke(pos);
        HandlePress(pos);
    }
    void LongPressDetected(Vector2 pos)
    {
        OnLongPress?.Invoke(pos);
    }
    void ReleaseDetected(Vector2 pos)
    {
        OnRelease?.Invoke(pos);
        HandleRelease();
    }

    private void Update()
    {
        // detect touches
        if (IsPointerDown())
        {
            if (!wasPointerDown)  // Pointer just pressed
            {
                wasPointerDown = true;
                pointerDownTime = Time.time;
                isLongPressTriggered = false;

                // Get pointer position (for both mouse and touch)
                pointerPosition = GetPointerPosition();
                TapDetected(pointerPosition);
            }

            // Check if long press condition is met
            if (Time.time - pointerDownTime > LONG_PRESS_DURATION && !isLongPressTriggered)
            {
                isLongPressTriggered = true;
                LongPressDetected(pointerPosition);
            }
        }
        else if (wasPointerDown && !IsPointerDown())  // Pointer released
        {
            ReleaseDetected(pointerPosition);
            wasPointerDown = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            HandleCancel();
        }
    }

    private void HandleCancel()
    {
        StopPlacingConveyor();
    }

    private void HandleRelease()
    {
    }

    private void HandleHold()
    {
    }

    private void HandlePress(Vector2 screenPos)
    {
        // get the world point of the press
        Vector3 worldPoint = GetMouseWorldPoint(screenPos); //No, breaks things
        if (IsCreatingPath)
        {
            // check for termination point in an input hook
            (LogisticComponent lc, int nearestInput) = GetNearestInputHook(worldPoint);
            if (nearestInput != -1)
            {
                // terminate the thing
                _to = lc;
                _toIndex = nearestInput;
                Transform inputHook = lc.InputHooks[nearestInput];
                Vector3 dir = inputHook.transform.forward;
                Points.AddRange(new Vector3[] { inputHook.position + dir*_settings.BELT_TURN_RADIUS, inputHook.position });
                StopPlacingConveyor();
            } else
            {
                Points.Add(worldPoint);
                AssignPath(Points);

                _current.UpdateMesh(beltBM, frameBM);

            }

        } else
        {
            // create new conveyor
            StartPlacingConveyor();

            //sphere cast to check for LogisticComponent
            (LogisticComponent lc, int nearestOutput) = GetNearestOutputHook(worldPoint);
            if (nearestOutput != -1)
            {
                _from = lc;
                _fromIndex = nearestOutput;
                // add 2 starter points to allow for natural connections and rounded edges
                Transform outputHook = lc.OutputHooks[nearestOutput];
                Vector3 dir = outputHook.forward;
                Points = new List<Vector3> { outputHook.position, outputHook.position + dir*_settings.BELT_TURN_RADIUS};
            }
            else
            {
                Points = new List<Vector3> { worldPoint };
            }
        }
    }

    private void OnEnable()
    {
        // listen to the cancel event to force cancel placement from elsewhere in the code
        cancelPlacementEvent.OnEvent += ForceCancel;

        // create dummy visual
        _dummyVisual = Instantiate(conveyorPrefab);
        _dummyVisual.gameObject.name = "DummyVisual";
        _dummyVisual.AssignPath<RoundedPolygon>(new Vector3[] { Vector3.zero, Vector3.forward });
        _dummyVisual.UpdateMesh(beltBM, frameBM);
        _dummyVisual.GetComponent<ConveyorBelt>().enabled = false;
    }
    private void OnDisable()
    {
        if (_dummyVisual != null)
            _dummyVisual.transform.position = new Vector3(0, -10f, 0f);
         
        // stop listening
        cancelPlacementEvent.OnEvent -= ForceCancel;

        if (_dummyVisual != null)
            Destroy(_dummyVisual.gameObject);
    }

    private void LateUpdate()
    {
        if (!IsCreatingPath)
        {
            var worldPos = gridCheck.gridPoint;//GetMouseWorldPoint(GetPointerPosition());    //No, breaks things

            //check for starting port
            (LogisticComponent lc, int nearestOutput) = GetNearestOutputHook(worldPos);
            if (nearestOutput != -1)
            {
                _dummyVisual.transform.position = lc.OutputHooks[nearestOutput].position;
                _dummyVisual.transform.forward = lc.OutputHooks[nearestOutput].forward;
                return;
            }

            _dummyVisual.transform.position = worldPos;
            return;
        }
        _dummyVisual.transform.position = new Vector3(0,-10f,0f);
        if (Points.Count>0)
        {
            var tempPoints = Points.ToList();
            var worldPoint = /*gridCheck.gridPoint;/*/GetMouseWorldPoint(GetPointerPosition());  //No, breaks things
            
            // check for teminus
            (LogisticComponent lc, int nearestInput) = GetNearestInputHook(worldPoint);
            if (nearestInput != -1)
            {
                // add 2 starter points to allow for natural connections and rounded edges
                Transform hook = lc.InputHooks[nearestInput];
                Vector3 dir = hook.forward;
                tempPoints.AddRange(new List<Vector3> { hook.position + dir * _settings.BELT_TURN_RADIUS, hook.position });
            }
            else
            {
                var newPoint = worldPoint;
                if (Vector3.Distance(newPoint, Points.Last()) < 1f)
                {
                    if (newPoint == Points.Last())
                        newPoint = Points.Last() + Vector3.right;
                }
                tempPoints.Add(newPoint);
            }

            AssignPath(tempPoints);
            _current.UpdateMesh(beltBM, frameBM);
        } else if (Points.Count == 0)
        {
            var tempPoints = Points.ToList();
            var newPoint = /*gridCheck.gridPoint;/*/GetMouseWorldPoint(GetPointerPosition());
            tempPoints.Add(newPoint);
            tempPoints.Add(newPoint + Vector3.right);
            AssignPath(tempPoints);
            _current.UpdateMesh(beltBM, frameBM);
        }

        if (_current.Path.IsValid)
        {
            _current.AssignBeltMaterial(greenGhostMat);
            _current.AssignFrameMaterial(greenGhostMat);
        } else
        {
            _current.AssignBeltMaterial(redGhostMat);
            _current.AssignFrameMaterial(redGhostMat);
        }
    }

    private void ForceCancel()
    {
        if (_current != null)
        {
            Destroy(_current.gameObject);
        }
        _current = null;
        _from = null;
        _to = null;
        _fromIndex = -1;
        _toIndex = -1;
    }

    public void StartPlacingConveyor() {
        //cancel any placement _currently happening
        cancelPlacementEvent?.Raise();
        // instantiate a belt to place
        _current = Instantiate(conveyorPrefab);
        
        // trigger event
        startPlacementEvent?.Raise();
    }
    void StopPlacingConveyor()
    {
        if (Points.Count < 2)
        {
            finishPlacementEvent?.Raise();
            if (_current != null) Destroy(_current.gameObject);
            _current = null;

            this.enabled = false;
            return;
        }
        AssignPath(Points);
        if (!_current.Path.IsValid)
        {
            finishPlacementEvent?.Raise();
            Destroy(_current.gameObject);
            _current = null;

            this.enabled = false;
            return;
        }

        _current.UpdateMesh(beltBM, frameBM);

        _current.AssignBeltMaterial(originalBeltMat);
        _current.AssignFrameMaterial(originalFrameMat);

        if (_from != null)
        {
            _from.ConnectOutput(_current, _fromIndex);
            _current.ConnectInput(_from);
        }
        if (_to != null)
        {
            _to.ConnectInput(_current, _toIndex);
            _current.ConnectOutput(_to);
        }
        
        finishPlacementEvent?.Raise();
        _current = null;
        _from = null;
        _to = null;
        _fromIndex = -1;
        _toIndex = -1;

        this.enabled = false;
    }

    private bool ValidLocation(Vector3 pos)
    {
        return _current.Path.IsValid;
    }

    Vector3 GetMouseWorldPoint(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        var raycasts = Physics.RaycastAll(ray, Mathf.Infinity, terrainLayer).OrderBy(rc=>rc.distance).Reverse();
        foreach (RaycastHit hit in raycasts)
        {
            if (hit.collider.TryGetComponent(out Terrain _))
            {
                return hit.point + Vector3.up * heightOffset;
            }
            else
            {
                Debug.Log("Raycast hits: " + hit.collider.name);
            }
        }
        // raycast onto the y=0 XZ plane
        //Debug.Log("Did not find terrain");
        return ray.origin + ray.direction * (ray.origin.y / -ray.direction.y);
        //return screenPos = gridCheck.gridPoint;   //Worth a try...
    }
    LogisticComponent[] FindNearby(Vector3 worldPos)
    {
        // spherecast world point to find nearby logisticcomponent objects. Order by distance
        var hits = Physics.SphereCastAll(worldPos, snapRadius, Vector3.up, snapRadius).OrderBy(h => h.distance);
        var components = new List<LogisticComponent>();
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out LogisticComponent component))
            {
                if (_current!=null && hit.collider.transform.root == _current.transform) continue;
                if (_dummyVisual == component) continue;
                components.Add(component);
            }
        }
        return components.ToArray();
    }
    (LogisticComponent, int) GetNearestOutputHook(Vector3 worldPos)
    {
        var outputs = FindNearby(worldPos);
        if (outputs.Length == 0) { return (null, -1); }
        foreach (var output in outputs)
        {
            var curr = output.GetNearestOutput(worldPos);
            if (curr != -1)
                return (output, curr);
        }
        return (null,-1);
    }
    (LogisticComponent, int) GetNearestInputHook(Vector3 worldPos)
    {
        var inputs = FindNearby(worldPos);
        if (inputs.Length == 0) return (null,-1);
        foreach (var input in inputs)
        {
            var curr = input.GetNearestInput(worldPos);
            if (curr != -1)
                return (input, curr);
        }
        return (null,-1);
    }

    private void OnDrawGizmos()
    {
        if (!this.enabled) return;
        //Vector3 pos = GetMouseWorldPoint(GetPointerPosition());
        Vector3 pos = gridCheck.gridPoint;
        Gizmos.DrawWireSphere(pos, snapRadius);
    }

    void AssignPath(List<Vector3> pathPoints)
    {
        if (_settings.PATHTYPE == GlobalLogisticsSettings.PathSolveType.SMART)
        {
            _current.AssignPath<RoundedPolygon>(pathPoints.ToArray());
        }
        else if (_settings.PATHTYPE == GlobalLogisticsSettings.PathSolveType.SPLINE)
        {
            _current.AssignPath<PointSplinePath>(pathPoints.ToArray());
        } else
        {
            Debug.LogError("Path type not supported");
        }
    }
}
