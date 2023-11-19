using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : MonoBehaviour
{
    // Singleton
    public static GridManager Instance { get; private set; }

    // Events
    [SerializeField] public UnityEvent SetGridSizeEvent;

    [Header("Configuration")]
    [SerializeField] public Vector2Int gridSize = new Vector2Int(5, 5);

    [Header("GUI Input Fields")]
    [SerializeField] private TextMeshProUGUI defaultX;
    [SerializeField] private TextMeshProUGUI defaultY;
    [SerializeField] private TextMeshProUGUI inputX;
    [SerializeField] private TextMeshProUGUI inputY;

    private GameObject nodeStart = null;
    private GameObject nodeEnd = null;


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

    void Start()
    {

        // GUI fields
        defaultX.text = "" + gridSize.x;
        defaultY.text = "" + gridSize.y;

        UpdateGrid();
    }

    public void UpdateGrid()
    {
        StateManager.Instance.ResetPathfinder.Invoke();
        NodeEnd = null;
        NodeStart = null;

        updateGridSizeFromInput();

        // Cursor
        Cursor.Instance.VisibleRegion = gridSize;

        // Grid 
        Grid.Instance.GridSize = gridSize;
        Grid.Instance.DestroyGrid();
        Grid.Instance.InitGrid();
    }

    private void updateGridSizeFromInput()
    {
        int x = Utility.Tmp2Int(inputX);
        int y = Utility.Tmp2Int(inputY);

        if (x > 0 && x != gridSize.x)
        {
            gridSize.x = x;
        }

        if (y > 0 && y != gridSize.y)
        {
            gridSize.y = y;
        }
    }

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
}
