using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86.Avx;

public class ArrayClone
{
    //private GameObject objectToPool;
    private GameObject[,] pooledObjects;
    //private static ArrayClone instance;
    private GameObject parent;
    //private Vector2 objectSize;
    //private Vector2Int cloneArraySize;

    public ArrayClone(GameObject objectToPool, Vector2 objectSize, Vector2Int cloneArraySize)
    {
        //instance = this;
        parent = GameObject.Instantiate(new GameObject(objectToPool.name + "Clone"));
        create(objectToPool, objectSize, cloneArraySize);
    }

    public void create(GameObject objectToPool, Vector2 objectSize, Vector2Int cloneArraySize) {
        pooledObjects = new GameObject[cloneArraySize.x, cloneArraySize.y];
        GameObject tmp;

        for (int i = 0; i < pooledObjects.GetLength(0); i++)
        {
            for (int j = 0; j < pooledObjects.GetLength(1); j++)
            {
                tmp = GameObject.Instantiate(objectToPool);
                tmp.SetActive(true);
                tmp.GetComponent<Transform>().SetParent(parent.GetComponent<Transform>());
                tmp.GetComponent<Transform>().SetLocalPositionAndRotation(new Vector3(i * objectSize.x, 0f, j * objectSize.y), Quaternion.identity);
                tmp.isStatic = true;
                pooledObjects[i, j] = tmp;
            }
        }

    }

    public void update(GameObject objectToPool, Vector2 objectSize, Vector2Int cloneArraySize)
    {
        Destroy();
        create(objectToPool, objectSize, cloneArraySize);
    }

    public void Destroy()
    {
        for (int i = 0; i < pooledObjects.GetLength(0); i++)
        {
            for (int j = 0; j < pooledObjects.GetLength(1); j++)
            {
                GameObject.Destroy(pooledObjects[i, j]);

            }
        }
    }






}
