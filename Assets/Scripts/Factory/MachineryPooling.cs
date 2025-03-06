using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineryPooling : MonoBehaviour
{
    public static MachineryPooling sharedInstance;
    public List<GameObject> pooledStructures;
    public GameObject structureToPool;
    public int amountToPool;

    private void Awake()
    {
        sharedInstance = this;
    }

    private void Start()
    {
        StartCoroutine(AddToStructurePool(true));
    }

    private IEnumerator AddToStructurePool(bool firstSpawn)
    {
        pooledStructures = new List<GameObject>();
        GameObject tmp;
        if (firstSpawn)
        {
            for (int i = 0; i < amountToPool; i++)
            {
                tmp = Instantiate(structureToPool, this.transform);
                tmp.SetActive(false);
                pooledStructures.Add(tmp);
            }
        }
        else
        {
            tmp = Instantiate(structureToPool, this.transform);
            tmp.SetActive(false);
            pooledStructures.Add(tmp);
        }


        
        yield return null;
    }

    public GameObject GetPooledStructures()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledStructures[i].activeInHierarchy)
            {
                return pooledStructures[i];
            }
            if (i > amountToPool)
            {
                Debug.Log("Building pool limit exceeded, adding additional");
                StartCoroutine(AddToStructurePool(false));
            }
        }
        return null;
    }

    void SpawnNewStructure()
    {

    }
}

