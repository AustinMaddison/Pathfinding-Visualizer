using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Grid grid;
    [SerializeField] private GridCursor cursor;
    [SerializeField] private GridNodeEditor editor;

    [Header("Configuration")]
    [SerializeField] Vector2Int gridSize = new Vector2Int(5, 5);

    [Header("GUI Input Fields")]
    [SerializeField] private TextMeshProUGUI defaultX;
    [SerializeField] private TextMeshProUGUI defaultY;
    [SerializeField] private TextMeshProUGUI inputX;
    [SerializeField] private TextMeshProUGUI inputY;

    private GameObject nodeStart = null;
    private GameObject nodeEnd = null;

    void Start()
    {
        // GUI fields
        defaultX.text = "" + gridSize.x;
        defaultY.text = "" + gridSize.y;

        UpdateGrid();
    }

    public void UpdateGrid()
    {
        updateGridSizeFromInput();

        // Cursor
        cursor.VisibleRegion = gridSize;

        // Grid 
        grid.GridSize = gridSize;
        grid.DestroyGrid();
        grid.InitGrid();
    }

    private void updateGridSizeFromInput()
    {
        int x = Utility.Tmp2Int(inputX);
        int y = Utility.Tmp2Int(inputY);

        Debug.Log("x input: " + x);
        Debug.Log("y input: " + y);

        if (x > 0 && x != gridSize.x)
        {
            gridSize.x = x;
        }

        if (y > 0 && y != gridSize.y)
        {
            gridSize.y = y;
        }
    }

    //// Update is called once per frame
    //void Update()
    //{
    //}

    public GameObject NodeStart
    {
        get { return nodeStart; }
        set { nodeStart = value; }
    }
    public GameObject NodeEnd
    {
        get { return nodeEnd; }
        set { nodeEnd = value; }
    }

    public Grid Grid{ get{ return grid;} }
}
