using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using FactoryFramework;


public class StressTest
{
    public List<LogisticComponent> components = new List<LogisticComponent>();

    private List<Item> items = new List<Item>();

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        Camera camera = new GameObject("Camera").AddComponent<Camera>();
        camera.transform.SetPositionAndRotation(new Vector3(0, 10f, -10), Quaternion.Euler(new Vector3(45f,0f,0f)));

        items = Resources.LoadAll<Item>("Items").ToList();
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        foreach (var component in components)
        {
            Object.Destroy(component.gameObject);
        }
        components.Clear();
        yield return null;
    }

    #region builders
    private ConveyorBelt BuildConveyor()
    {
        GameObject conveyor = new GameObject("Conveyor");
        conveyor.AddComponent<SerializationReference>();
        var conveyorComponent = conveyor.AddComponent<ConveyorBelt>();
        conveyor.AddComponent<MeshRenderer>();
        conveyor.AddComponent<MeshFilter>();
        components.Add(conveyorComponent);
        conveyorComponent.Inputs = new LogisticComponent[1];
        conveyorComponent.Outputs = new LogisticComponent[1];
        conveyorComponent.speed = Random.Range(0.1f, 10f);
        return conveyorComponent;
    }
    private Producer BuildProducer()
    {
        GameObject producer = new GameObject("Producer");
        producer.AddComponent<SerializationReference>();
        producer.AddComponent<MeshRenderer>();
        producer.AddComponent<MeshFilter>();
        var producerComponent = producer.AddComponent<Producer>();
        components.Add(producerComponent);
        producerComponent.internalStorage = new LocalStorage() { itemStack = new ItemStack() { item = items[Random.Range(0, items.Count)] } };
        producerComponent.Outputs = new LogisticComponent[1];
        return producerComponent;
    }
    #endregion

    [UnityTest]
    public IEnumerator TestConveyorLoop()
    {
        Vector3[] rectange = new Vector3[]
        {
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 3),
            new Vector3(3, 0, 3),
            new Vector3(3, 0, 0),
            new Vector3(0, 0, 1),
        };
        var conveyor = BuildConveyor();
        yield return null;
        conveyor.AssignPath<RoundedPolygon>(rectange);
        conveyor.ConnectInput(BuildProducer());

        yield return null;
        foreach(var item in items)
        {
            conveyor.RecieveItem(item);
        }

        yield return new WaitForSeconds(1f);
        Assert.Greater(conveyor.Length, 0);
    }

    [UnityTest]
    public IEnumerator TestProducer()
    {
        var producer = BuildProducer();
        var resource = items[Random.Range(0, items.Count)];
        producer.internalStorage = new LocalStorage() { itemStack = new ItemStack() { item = resource } };
        producer.Outputs = new LogisticComponent[1];
        producer.resource = resource;
        producer.resourcesPerSecond = 1f;
        yield return new WaitForSeconds(1f);
        Assert.AreEqual(0, producer.internalStorage.itemStack.amount);
    }
}
