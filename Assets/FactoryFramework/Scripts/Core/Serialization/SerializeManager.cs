using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Splines;

namespace FactoryFramework
{
    public class SerializeManager : MonoBehaviour
    {
        [SerializeField] private bool _debugInfo;

        // Event is triggered when loading completes
        // Boolean value is returned depending on success
        [SerializeField]
        private UnityEvent<bool> _onLoadComplete;
        public UnityEvent<bool> OnLoadComplete
        {
            get { return _onLoadComplete; }
        }

        // Event is triggered when saving completes
        // Boolean value is returned depending on success
        [SerializeField]
        private UnityEvent<bool> _onSaveComplete;
        public UnityEvent<bool> OnSaveComplete
        {
            get { return _onSaveComplete; }
        }

        private string saveFilePath;
        private void Awake()
        {
            saveFilePath = Application.persistentDataPath + "/";
        }

        public void Load() => Load("save.json");
        public async void Load(string path)
        {
            var settings = ConveyorLogisticsUtils.settings;
            string filePath = Path.Combine(saveFilePath, path);

            if (!File.Exists(filePath))
            {
                OnLoadComplete.Invoke(false);
                return;
            }
            try
            {

                // Deserialize data into a FactorySaveData object
                string saveString = File.ReadAllText(filePath);
                var data = JsonUtility.FromJson<FactorySaveData>(saveString);

                // remove all existing buildings and cables
                CableRendererManager.instance?.Clear();
                foreach (SerializationReference obj in GameObject.FindObjectsOfType<SerializationReference>()) Destroy(obj.gameObject);

                var powerGridNodes = data.powerGridNodes;
                var powerGridEdges = data.powerGridEdges;

                /// steps to recreate objects in level
                // 1. Spawn all objects
                // 2. Connect logisticComponents to right GUID-identified LogisticComponents
                // 3. connect cables between GUID_identified PowerGridcomponents
                ///

                // build a lookup of Guid -> LogisticComponent
                // this is used to re-link the conveyor belts to buildings
                Dictionary<Guid, SerializationReference> lookup = new Dictionary<Guid, SerializationReference>();
               

                foreach (SerializedLogisticComponent obj in data.components)
                {
                    // spawn the prefab
                    SerializationReference sRef = InstantiateBuildingData(obj);
                    lookup.Add(sRef.GUID, sRef);

                    if (sRef.TryGetComponent(out LogisticComponent lc))
                    {
                        switch (lc)
                        {
                            case Producer producer:
                                producer.SetResource(Resources.Load<Item>(obj.resourceReference));
                                producer.internalStorage.itemStack = new ItemStack()
                                {
                                    item = Resources.Load<Item>(obj.internalOutput[0].itemResourcePath),
                                    amount = obj.internalOutput[0].amount
                                };
                                //producer.SecondsSinceLastResource = obj.timeSinceLastJob;
                                break;
                            case Processor processor:
                                processor.recipe = Resources.Load<Recipe>(obj.recipeReference);
                                processor.inputItems = obj.internalInput.Select(i => new LocalStorage()
                                {
                                    itemStack = new ItemStack()
                                    {
                                        item = Resources.Load<Item>(i.itemResourcePath),
                                        amount = i.amount
                                    }
                                }).ToArray();
                                processor.outputItems = obj.internalOutput.Select(i => new LocalStorage()
                                {
                                    itemStack = new ItemStack()
                                    {
                                        item = Resources.Load<Item>(i.itemResourcePath),
                                        amount = i.amount
                                    }
                                }).ToArray();
                                //processor.RecipeStartTime = obj.timeSinceLastJob;
                                break;
                            case Storage storage:
                                storage.storage = obj.internalOutput.Select(i => new ItemStack()
                                {
                                    item = Resources.Load<Item>(i.itemResourcePath),
                                    amount = i.amount
                                }).ToArray();
                                break;
                            case ConveyorBelt conveyor:
                                conveyor.speed = obj.speed;
                                if (settings.PATHTYPE == GlobalLogisticsSettings.PathSolveType.SMART)
                                    conveyor.AssignPath<RoundedPolygon>(obj.pathPoints);
                                else if (settings.PATHTYPE == GlobalLogisticsSettings.PathSolveType.SPLINE) 
                                    conveyor.AssignPath<PointSplinePath>(obj.pathPoints);
                                conveyor.UpdateMesh(settings.BELT_MESH_SO, settings.FRAME_MESH_SO); //FIXME beltmesh and frame mesh from somewhere. Settings perhaps
                                conveyor.DeserializeItems(obj.iobs);
                                break;
                        }
                    }   
                }
                //wait one frame to allow all objects to run their Start() calls
                await Task.Yield(); // FIXME if you want to use webgl
                // second pass to connect all logistic components now
                foreach (SerializedLogisticComponent slc in data.components)
                {
                    SerializationReference sRef = lookup[new Guid(slc.guid)];

                    if (sRef.TryGetComponent(out LogisticComponent self))
                    {
                        for (int i = 0; i < slc.InputReferences.Length; i++)
                        {
                            var guid = slc.InputReferences[i];
                            if (string.IsNullOrEmpty(guid)) continue;
                            if (lookup.TryGetValue(new Guid(guid), out SerializationReference input))
                            {
                                if (self as Merger != null)
                                {
                                    self.ConnectInput(input.GetComponent<LogisticComponent>(), i);
                                }
                                else
                                    self.ConnectInput(input.GetComponent<LogisticComponent>());
                            }
                        }
                            
                        
                        for (int i = 0; i < slc.OutputReferences.Length; i++)
                        {
                            var guid = slc.OutputReferences[i];
                            if (string.IsNullOrEmpty(guid)) continue;
                            if (lookup.TryGetValue(new Guid(guid), out SerializationReference output))
                            {
                                if (self as Splitter != null)
                                {
                                    self.ConnectOutput(output.GetComponent<LogisticComponent>(), i);
                                }
                                else
                                    self.ConnectOutput(output.GetComponent<LogisticComponent>());
                            }
                        }
                    }
                }

                //wait one frame to allow all objects to run their Start() calls
                await Task.Yield(); // FIXME if you want to use webgl

                // zip powergrid nodes and edges together
                var powerGrids = powerGridNodes.Zip(powerGridEdges, (nodes, edges) => (nodes, edges));
                for (int i = 0; i < powerGrids.Count(); i++)
                {
                    var (nodesStr, edgesStr) = powerGrids.ElementAt(i);
                    var nodes = nodesStr.Split('\n');
                    var edges = string.IsNullOrEmpty(edgesStr) ? new (string, string)[0] : edgesStr.Split('\n').Select(e => e.Split(',')).Select(
                        e => (e[0].Substring(1), e[1].Substring(0, e[1].Length-1))).Where(e=>lookup.ContainsKey(new Guid(e.Item1)) && lookup.ContainsKey(new Guid(e.Item2))).ToArray();
                    PowerGrid grid = new PowerGrid();
                    PowerGridComponent[] pgcs = nodes.Where(n=>lookup.ContainsKey(new Guid(n))).Select(n => lookup[new Guid(n)].GetComponent<PowerGridComponent>()).ToArray();
                    foreach (var pgc in pgcs)
                    {
                        pgc.grid = grid;
                        grid.AddNode(pgc);
                    }
                    foreach (var edge in edges)
                    {
                        var a = lookup[new Guid(edge.Item1)].GetComponent<PowerGridComponent>();
                        var b = lookup[new Guid(edge.Item2)].GetComponent<PowerGridComponent>();
                        a.Connect(b);
                    }
                }

                OnLoadComplete.Invoke(true);
            }
            catch (Exception e)
            {
                Debug.LogError("FactoryFramework load failed! - " + e.ToString());
                OnLoadComplete.Invoke(false);
                throw e;
            }
        }
        public void Save() => Save("save.json");
        public void Save(string path)
        {
            List<SerializedLogisticComponent> serializedComponents = new List<SerializedLogisticComponent>();
            string filePath = Path.Combine(saveFilePath, path);

            // collect and sort buildings
            var objects = FindObjectsOfType<SerializationReference>();
            

            foreach (var obj in objects)
            {
                var saveData = new SerializedLogisticComponent()
                {
                    type = obj.GetType().Name,
                    position = obj.transform.position,
                    rotation = obj.transform.rotation,
                    guid = obj.GUID.ToString(),
                    resourcesPath = obj.resourcesPath,
                };
                if (obj.TryGetComponent(out LogisticComponent lc))
                {

                    saveData.InputReferences = lc.Inputs.Select(i => i?.GUID.ToString() ?? null).ToArray();
                    saveData.OutputReferences = lc.Outputs.Select(i => i?.GUID.ToString() ?? null).ToArray();

                    switch (lc)
                    {
                        case Producer producer:
                            saveData.resourceReference = producer.resource?.resourcesPath ?? null;
                            saveData.internalOutput = new SerializedItemStack[1]{new SerializedItemStack()
                            {
                                itemResourcePath = producer.internalStorage.itemStack.item?.resourcesPath ?? null,
                                amount = producer.internalStorage.itemStack.amount
                            } };
                            saveData.timeSinceLastJob = producer.SecondsSinceLastResource;
                            break;
                        case Processor processor:
                            saveData.recipeReference = processor.recipe?.resourcesPath ?? null;
                            saveData.internalInput = processor.inputItems.Select(i => new SerializedItemStack()
                            {
                                itemResourcePath = i.ItemType?.resourcesPath ?? null,
                                amount = i.itemStack.amount
                            }).ToArray();
                            saveData.internalOutput = processor.outputItems.Select(i => new SerializedItemStack()
                            {
                                itemResourcePath = i.ItemType?.resourcesPath ?? null,
                                amount = i.itemStack.amount
                            }).ToArray();
                            saveData.timeSinceLastJob = processor.RecipeStartTime;
                            break;
                        case Storage storage:
                            saveData.internalOutput = storage.storage.Select(i => new SerializedItemStack()
                            {
                                itemResourcePath = i.item?.resourcesPath ?? null,
                                amount = i.amount
                            }).ToArray();
                            break;
                        case ConveyorBelt conveyor:
                            saveData.speed = conveyor.speed;
                            saveData.pathPoints = conveyor.Path.OriginalPoints;
                            saveData.iobs = conveyor.Items.Select(i => new SerializedItemOnBelt()
                            {
                                itemResourcePath = i.item.resourcesPath,
                                position = i.position,
                                queueIndex = i.queueIndex
                            }).ToArray();
                            break;
                    }
                }
                serializedComponents.Add(saveData);
            }

            // Serialize PowerGrid data
            HashSet<List<string>> powerGridNodes = new HashSet<List<string>>(new ListComparer());
            HashSet<List<(string, string)>> powerGridEdges = new HashSet<List<(string, string)>>(new TupleListComparer());
            
            foreach(var pgc in FindObjectsOfType<PowerGridComponent>())
            {
                var (nodes, edges) = pgc.grid.ToDOT();
                powerGridNodes.Add(nodes.ToList());
                powerGridEdges.Add(edges.ToList());
            }

            var data = new FactorySaveData()
            {
                // cannot serializae polymorphic list, must convert all to string representation
                components = serializedComponents.ToArray(),
                powerGridNodes = powerGridNodes.Select(l => string.Join('\n', l)).ToArray(),
                powerGridEdges = powerGridEdges.Select(l => string.Join('\n', l)).ToArray()
            };

            var jsonString = JsonUtility.ToJson(data, true);

            File.WriteAllText(filePath, jsonString);
            print($"saving data to {filePath}");

            OnSaveComplete.Invoke(true);
        }

        public SerializationReference InstantiateBuildingData(SerializedLogisticComponent obj)
        {
            GameObject prefab = Resources.Load<GameObject>(obj.resourcesPath);
            GameObject instantiated = Instantiate(prefab, obj.position, obj.rotation);

            SerializationReference sRef = instantiated.GetComponent<SerializationReference>();
            sRef.GUID = new Guid(obj.guid);
            sRef.resourcesPath = obj.resourcesPath;

            return sRef;
        }


        private void OnGUI()
        {
            if (!_debugInfo) return;
            GUILayout.Space(100);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                this.Save();
            }
            if (GUILayout.Button("Load"))
            {
                this.Load();
            }
            GUILayout.EndHorizontal();
        }

        #region SAVE_DATA_TYPES

        [Serializable]
        public class FactorySaveData
        {
            public SerializedLogisticComponent[] components;
            public string[] powerGridNodes;
            public string[] powerGridEdges;
        }

        public class ListComparer : IEqualityComparer<List<string>>
        {
            public bool Equals(List<string> x, List<string> y)
            {
                if (x == null || y == null)
                    return false;

                if (x.Count != y.Count)
                    return false;

                for (int i = 0; i < x.Count; i++)
                {
                    if (x[i] != y[i])
                        return false;
                }

                return true;
            }

            public int GetHashCode(List<string> obj)
            {
                int hash = 17;

                foreach (string s in obj)
                {
                    hash = hash * 31 + (s?.GetHashCode() ?? 0);
                }

                return hash;
            }
        }

        public class TupleListComparer : IEqualityComparer<List<(string, string)>>
        {
            public bool Equals(List<(string, string)> x, List<(string, string)> y)
            {
                if (x == null || y == null)
                    return false;

                if (x.Count != y.Count)
                    return false;

                for (int i = 0; i < x.Count; i++)
                {
                    if (x[i] != y[i])
                        return false;
                }

                return true;
            }

            public int GetHashCode(List<(string, string)> obj)
            {
                int hash = 17;

                foreach ((string,string) s in obj)
                {
                    hash = hash * 31 + (s.Item1?.GetHashCode() ?? 0) + (s.Item2?.GetHashCode() ?? 0);
                }

                return hash;
            }
        }


        #endregion
    }
}
