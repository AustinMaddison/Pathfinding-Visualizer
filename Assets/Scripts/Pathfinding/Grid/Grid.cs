using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public static Grid Instance { get; private set; }

    [SerializeField] private GridManager gridManager;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2 nodeSize = Vector2.one;
    [SerializeField] private Vector3 originPosition  = Vector3.zero;

    [SerializeField] private GameObject nodePrefab;
    private GameObject[,] grid = null;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void InitGrid()
    {
        grid = new GameObject[gridSize.x, gridSize.y];

        GameObject tmp;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {

                tmp = Instantiate(nodePrefab, new Vector3(x, 0f, y), Quaternion.identity);

                Node node = tmp.GetComponent<Node>();
                node.Position = new Vector2Int(x, y);
                tmp.transform.SetParent(transform, true);
                grid[x, y] = tmp;
            }
        }
    }

    public void DestroyGrid()
    {
        if (grid != null)
        {
            foreach (GameObject node in grid) Destroy(node);
        }
    }

    public Vector2Int GridSize
    {
        get
        {
            return gridSize;
        }

        set
        {
            gridSize = value;
        }
    }
  
    public Vector2 GetNodeSize()
    {
        return nodeSize;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        Vector3 worldPos = new Vector3(x, y);

        worldPos.x *= nodeSize.x;
        worldPos.y *= nodeSize.y;

        return worldPos + originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / nodeSize.x);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / nodeSize.y);
    }

    public GameObject GetValue(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.y >= 0 && pos.x < grid.GetLength(0) && pos.y < grid.GetLength(1))
        {
            return grid[pos.x, pos.y];
        }
        else
        {
            return null;
        }

    }

}
