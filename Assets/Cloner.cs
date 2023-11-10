using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;

public class Cloner : MonoBehaviour
{
    public static Cloner SharedInstance;
    public GameObject objectToPool;
    public GameObject[,] pooledObjects;
    public Vector2 objectSize;
    public Vector2Int cloneArraySize;


    // Start is called before the first frame update
    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledObjects = new  GameObject[cloneArraySize.x, cloneArraySize.y];
        GameObject tmp;

        for (int i = 0; i < pooledObjects.GetLength(0); i++)
        {
            for (int j = 0; j < pooledObjects.GetLength(1); j++)
            {
                tmp = Instantiate(objectToPool);
                tmp.SetActive(true);
                tmp.GetComponent<Transform>().SetParent(this.GetComponent<Transform>());
                tmp.GetComponent<Transform>().SetLocalPositionAndRotation(new Vector3(i * objectSize.x, 0f, j * objectSize.y), Quaternion.identity);
                tmp.isStatic = true;
                pooledObjects[i, j] = tmp;
            }
        }

        //GetPooledObject().SetActive(true);
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.GetLength(0); i++)
        {
            for (int j = 0; j < pooledObjects.GetLength(1); j++)
            {
                if (!pooledObjects[i, j].activeInHierarchy)
                {
                    return pooledObjects[i, j];
                }
            }
        }
        return null;
    }

}
