using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private WorldGrid grid;
    [SerializeField] private GridCursor cursor;

    [Header("Configuration")]
    [SerializeField] Vector2Int gridSize = new Vector2Int(5, 5);

    [Header("GUI Input Fields")]
    [SerializeField] private TextMeshProUGUI defaultX;
    [SerializeField] private TextMeshProUGUI defaultY;
    [SerializeField] private TextMeshProUGUI inputX;
    [SerializeField] private TextMeshProUGUI inputY;


    // Start is called before the first frame update
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

        // Update Componenets
        cursor.VisibleRegion = gridSize;
        grid.GridSize = gridSize;
        Debug.Log("grid size: " + grid.GridSize);
        grid.DestroyGrid();
        grid.InitGrid();
    }

    private void updateGridSizeFromInput()
    {
        int x = Utility.Tmp2Int(inputX);
        int y = Utility.Tmp2Int(inputY);

        Debug.Log("x input: " + x);
        Debug.Log("y input: " + y);

        if(x > 0 && x != gridSize.x)
        {
            gridSize.x = x;
        }

        if (y > 0 && y != gridSize.y)
        {
            gridSize.y = y;
        }
    } 

    // Update is called once per frame
    void Update()
    {
    }
 

}
