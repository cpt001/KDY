using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;
using System;
using NUnit.Framework.Internal;
using UnityEngine.Splines;

namespace FactoryFramework
{

    [RequireComponent(typeof(MeshCollider))]
    public class ConveyorBelt : LogisticComponent
    {
        public float speed;
        public IPath Path { get; private set; }
        public float Length { get; private set; }

        MeshCollider _meshCollider;
        [SerializeField] MeshFilter beltMeshFilter;
        [SerializeField] MeshRenderer beltMeshRenderer;
        [SerializeField] MeshFilter frameMeshFilter;
        [SerializeField] MeshRenderer frameMeshRenderer;

        public List<ItemOnBelt> Items { get; private set; }

        #region Job Parameters
        JobHandle _jobHandle;
        NativeArray<float> _itemPositions;
        NativeArray<int> _itemQueueIndex;
        TransformAccessArray _transforms;
        float accumulatedDeltaTime = 0f;
        #endregion

        #region Item Transfer
        public override Item OutputItem
        {
            get
            {
                if (Items.Count == 0)
                    return null;
                var lastItem = Items.Where(item => item.queueIndex == 0).First();
                if (math.distancesq(lastItem.position, Path.TotalLength)<.001f)
                    return lastItem.item;
                return null;
            }
        }
        #endregion
        public void AssignPath(IPath path) => this.Path = path;
        void SetHooks()
        {
            if (InputHooks != null && OutputHooks!=null)
            {
                foreach (var hook in InputHooks.Union(OutputHooks))
                {
                    if (Application.isPlaying)
                    {
                        Destroy(hook.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(hook.gameObject);
                    }
                }
            }
            InputHooks = new Transform[1] { new GameObject("InputHook").transform };
            InputHooks[0].SetParent(transform);
           
            OutputHooks = new Transform[1] { new GameObject("OutputHook").transform };
            OutputHooks[0].SetParent(transform);
           
            InputHooks[0].position = Path.GetPositionAtPoint(0);
            InputHooks[0].rotation = Path.GetRotationAtPoint(0);
            InputHooks[0].forward *= -1f;
            OutputHooks[0].position = Path.GetPositionAtPoint(Length);
            OutputHooks[0].rotation = Path.GetRotationAtPoint(Length);

            if (InputHookVisuals!= null && InputHookVisuals.Length > 0)
            {
                InputHookVisuals[0].transform.position = InputHooks[0].position;
                InputHookVisuals[0].transform.rotation = InputHooks[0].rotation;
            }
            if (OutputHookVisuals!=null && OutputHookVisuals.Length > 0)
            {
                OutputHookVisuals[0].transform.position = OutputHooks[0].position;
                OutputHookVisuals[0].transform.rotation = OutputHooks[0].rotation;
            }
        }
        public void AssignPath<T>(Vector3[] pathPoints) where T : struct,IPath
        {
            Path?.CleanUp();
            Path = (T)Activator.CreateInstance(typeof(T), new object[] { pathPoints, settings.BELT_TURN_RADIUS, 5 });
            Length = Path.TotalLength;
            SetHooks();
        }

        public void UpdateMesh()
        {
            if (Path == null) return;
            var mesh = BeltMeshGenerator.GenerateFlatProcedural(Path, settings.BELT_SCALE);
            beltMeshFilter.sharedMesh = mesh;
            if (_meshCollider != null)
                _meshCollider.sharedMesh = mesh;
        }

        public void UpdateMesh(BeltMeshSO beltConfig, BeltMeshSO frameConfig)
        {
            if (Path == null) return;
            if (!Path.IsValid)
            {
                if (settings.SHOW_DEBUG_LOGS) Debug.Log("Invalid Conveyor due to path");
            }
            int length = Mathf.Max(1, (int)(Path.TotalLength * settings.BELT_SEGMENTS_PER_UNIT));
            //Debug.Log($"Length is {length}"); // development debug

            //bool collision = PathFactory.CollisionAlongPath(p, 0.5f, ConveyorLogisticsUtils.settings.BELT_SCALE / 2f, ~0, ignored, startskip: startskip, endskip: endskip); //only collide belt collideable layer
            //if (collision)
            //{
            //    if (settings.SHOW_DEBUG_LOGS) Debug.Log("Invalid Conveyor due to collision");
            //    _validMesh = false;
            //}

            if (frameMeshFilter != null) frameMeshFilter.sharedMesh?.Clear();
            beltMeshFilter.sharedMesh?.Clear();

            if (Application.isEditor && Application.isPlaying)
            {
                if (frameMeshFilter != null) Destroy(frameMeshFilter?.sharedMesh);
                Destroy(beltMeshFilter.sharedMesh);
            }
            else
            {
                if (frameMeshFilter != null) DestroyImmediate(frameMeshFilter?.sharedMesh);
                DestroyImmediate(beltMeshFilter.sharedMesh);
            }

            frameMeshFilter.mesh = BeltMeshGenerator.Generate(Path, frameConfig, length, ConveyorLogisticsUtils.settings.BELT_SCALE);
            beltMeshFilter.mesh = BeltMeshGenerator.Generate(Path, beltConfig, length, ConveyorLogisticsUtils.settings.BELT_SCALE / 4f, 1f, true);

            //combing belt and frame meshes for mesh collider
            if (_meshCollider != null)
            {
                CombineInstance[] combine = new CombineInstance[2];
                combine[0].mesh = frameMeshFilter.mesh;
                combine[0].transform = frameMeshFilter.transform.localToWorldMatrix;
                combine[1].mesh = beltMeshFilter.mesh;
                combine[1].transform = beltMeshFilter.transform.localToWorldMatrix;
                var combinedMesh = new Mesh();
                combinedMesh.CombineMeshes(combine);
                _meshCollider.sharedMesh = combinedMesh;
            }

            beltMeshRenderer = beltMeshFilter.gameObject.GetComponent<MeshRenderer>();
        }

        public void SetMaterials(Material frameMat, Material beltMat)
        {
            if (frameMeshFilter != null) frameMeshFilter.gameObject.GetComponent<MeshRenderer>().material = frameMat;
            beltMeshFilter.gameObject.GetComponent<MeshRenderer>().material = beltMat;
        }

        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
            if (!Application.isPlaying) return;
            beltMeshRenderer.sharedMaterial.SetFloat("_Speed", speed);
        }

        public void DeserializeItems(SerializedItemOnBelt[] itemSaveData)
        {
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    Destroy(item.model.gameObject);
                }
                Items.Clear();
            }else
                Items = new List<ItemOnBelt>();
            foreach (var iob in itemSaveData)
            {
                var itemRef = Resources.Load<Item>(iob.itemResourcePath);
                var spawnedItem = Instantiate(itemRef.prefab, transform);
                spawnedItem.transform.SetPositionAndRotation(Path.GetPositionAtPoint(iob.position), Path.GetRotationAtPoint(iob.position));
                var iobInstance = new ItemOnBelt
                {
                    item = itemRef,
                    position = iob.position,
                    model = spawnedItem.transform,
                    queueIndex = iob.queueIndex,
                };
                Items.Add(iobInstance);
            }
        }

        private void Awake()
        {
            _powerGridComponent ??= GetComponent<PowerGridComponent>();
            _sRef ??= GetComponent<SerializationReference>();
            _meshCollider = GetComponent<MeshCollider>();
            _meshCollider.providesContacts = true;
            Items = new List<ItemOnBelt>();
        }

        public void AssignFrameMaterial(Material m)
        {
            frameMeshRenderer.material = m;
        }
        public void AssignBeltMaterial(Material m)
        {
            beltMeshRenderer.material = m;
        }

        public override void ProcessLoop()
        {
            if (Items.Count == 0)
                return;
            accumulatedDeltaTime = Time.deltaTime;
            ScheduleJob();
            _jobHandle.Complete();
            ExtractJobResults();
        }

        private void ScheduleJob()
        {
            //Debug.Log("scheduling job: " + Items.Count);
            _itemPositions = new NativeArray<float>(Items.Count, Allocator.TempJob);
            _itemQueueIndex = new NativeArray<int>(Items.Count, Allocator.TempJob);
            _transforms = new TransformAccessArray(Items.Select(item=>item.model).ToArray());
            for (int i = 0; i < Items.Count; i++)
            {
                _itemPositions[i] = Items[i].position;
                _itemQueueIndex[i] = Items[i].queueIndex;
            }
            if (Path is RoundedPolygon)
            {
                _jobHandle = new ConveyorItemPositionJob<RoundedPolygon>
                {
                    itemPositions = _itemPositions,
                    itemQueueIndex = _itemQueueIndex,
                    speed = speed,
                    deltaTime = accumulatedDeltaTime,
                    path = (RoundedPolygon)Path,
                    minSpacing = .5f
                }.Schedule(_transforms);
            } else if (Path is PointSplinePath)
            {
                _jobHandle = new ConveyorItemPositionJob<PointSplinePath>
                {
                    itemPositions = _itemPositions,
                    itemQueueIndex = _itemQueueIndex,
                    speed = speed,
                    deltaTime = accumulatedDeltaTime,
                    path = (PointSplinePath)Path,
                    minSpacing = .5f
                }.Schedule(_transforms);
            }
            
        }
        private void ExtractJobResults()
        {
            //Debug.Log("Finishing job");
            if (Items.Count != _itemPositions.Count() - 1)
            {
                // first we need to ignore the
            }
            
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                item.position = _itemPositions[i];
                Items[i] = item;
            }
            _itemPositions.Dispose();
            _itemQueueIndex.Dispose();
            _transforms.Dispose();
        }

        public override bool TransferItem(LogisticComponent output)
        {
            if (Items.Count == 0)
                return false;
            var lastItem = Items.Where(item => item.queueIndex == 0).First();
            Destroy(lastItem.model.gameObject);
            Items.Remove(lastItem);
            for(int i = 0; i < Items.Count; i++)
            {
                var x = Items[i];
                x.queueIndex -=1;
                Items[i] = x;
            }
            Items = Items.OrderBy(item => item.queueIndex).ToList();
            return true;
        }
        public override bool CanRecieveItem(Item item)
        {
            return item!= null && (Items.Count==0 || Items.Min(i=>i.position) >= settings.BELT_SPACING);
        }
        public override bool RecieveItem(Item item)
        {
            var spawnedItem = Instantiate(item.prefab, transform);
            spawnedItem.transform.SetPositionAndRotation(Path.GetPositionAtPoint(0), Path.GetRotationAtPoint(0));
            var iob = new ItemOnBelt
            {
                item = item,
                position = 0,
                model = spawnedItem.transform,
                queueIndex = Items.Count,
            };

            Items.Add(iob);
            return true;
        }

        private void OnDrawGizmosSelected()
        {
            if (Path == null)
                return;
            // draw the path in world space
            try
            {
                for (int i = 0; i < Path.PathPoints.Length - 1; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(Path.PathPoints[i], Path.PathPoints[i + 1]);
                    Gizmos.DrawSphere(Path.PathPoints[i], .025f);
                    var quaternion = Path.PathRotations[i];
                    var right = 0.5f * settings.BELT_TURN_RADIUS * math.mul(quaternion, math.right());
                    //Gizmos.DrawLine(right + Path.PathPoints[i], -right + Path.PathPoints[i]);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(Path.PathPoints[i], right + Path.PathPoints[i]);
                }
                Gizmos.color = Color.red;
                var p = Path.PathPoints[Path.PathPoints.Length - 1];
                Gizmos.DrawSphere(p, .025f);
                var lastquaternion = Path.PathRotations[Path.PathRotations.Length - 1];
                var lastright = 0.5f * settings.BELT_TURN_RADIUS * math.mul(lastquaternion, math.right());
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(p, lastright + p);
            }
            catch (ObjectDisposedException ex)
            {
                // trying to access unbuilt Path
            }
        }

        private void OnDestroy()
        {
            Path?.CleanUp();
        }
    }


    public struct ConveyorItemPositionJob<TPath> : IJobParallelForTransform where TPath : struct, IPath
    {
        public NativeArray<float> itemPositions;
        [ReadOnly]
        public NativeArray<int> itemQueueIndex; 
        [ReadOnly]
        public float minSpacing;
        [ReadOnly]
        public float speed;
        [ReadOnly]
        public float deltaTime;
        [ReadOnly]
        public TPath path;

        public void Execute(int index, TransformAccess transform)
        {
            int queueIndex = itemQueueIndex[index];
            float maxPosition = path.TotalLength - (queueIndex * minSpacing);
            var currPos = itemPositions[index];
            currPos += speed * deltaTime;
            if (currPos > maxPosition)
            {
                currPos = maxPosition;
            }
            itemPositions[index] = currPos;
            //Debug.Log($"queue index {queueIndex} new currPos {currPos}");
            // smoothly lerp to the new position
            transform.position = math.lerp(transform.position, path.GetPositionAtPoint(currPos), .1f);
            transform.rotation = math.slerp(transform.rotation, path.GetRotationAtPoint(currPos), .025f);
        }
    }
}

