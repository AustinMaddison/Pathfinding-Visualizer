using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridManger : MonoBehaviour
{
    private static GridManger manager;
    [SerializeField] private Vector2Int gridSize = new Vector2Int(5, 5);
    [SerializeField] private Vector2Int gridSizeMax = new Vector2Int(25, 25);
    [SerializeField] private Vector2Int nodeSize = new Vector2Int(1, 1);

    [SerializeField] private GameObject nodeMeshPrefab;
    [SerializeField] private GameObject nodeLabelPrefab;

    [SerializeField] private TextMeshProUGUI inputX;
    [SerializeField] private TextMeshProUGUI inputY;

    private ArrayClone nodeMeshPool = null;
    private ArrayClone labelPool = null;

    // Start is called before the first frame update
    void Start()
    {
        manager = this;
        generatePrefabs();
    }

    public void SetSize(Vector2Int size)
    {
        gridSize = size; 
    }

    public void UpdateGrid()
    {
        if (inputX.text.Length == 1 | inputY.text.Length == 1)
        {
            generatePrefabs();
            return;
        }

        int.TryParse(inputX.text.Replace((char)8203, ' '), out int x);
        int.TryParse(inputY.text.Replace((char)8203, ' '), out int y);

        Debug.Log(inputX.text);
        Debug.Log(inputY.text);
        Debug.Log(x);
        Debug.Log(y);

        if (x > 0 && x <= gridSizeMax.x && y > 0 && y <= gridSizeMax.y)
        {
            SetSize(new Vector2Int(x, y));
            generatePrefabs();
        }
        else
        {
            Debug.Log("Error: input over max grid");
        }
    }

    private void generatePrefabs()
    {
        if(nodeMeshPool == null || labelPool == null)
        {
            nodeMeshPool = new ArrayClone(nodeMeshPrefab, nodeSize, gridSize);
            labelPool = new ArrayClone(nodeLabelPrefab, nodeSize, gridSize);
            return;
        }


        nodeMeshPool.update(nodeMeshPrefab, nodeSize, gridSize);
        labelPool.update(nodeLabelPrefab, nodeSize, gridSize);
        
    }

}
