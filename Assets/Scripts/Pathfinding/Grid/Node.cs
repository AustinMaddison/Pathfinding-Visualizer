using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum NodeState
{
    DEFAULT,
    START,
    END,
    OBSTACLE,
    OPEN,
    CLOSED,
    PATH
}

public enum Direction
{
    NONE, NORTH, SOUTH, WEST, EAST, NORTH_WEST, NORTH_EAST, SOUTH_WEST, SOUTH_EAST
}

public class Node : MonoBehaviour, IComparable<Node>
{
    [Header("Node Type Materials")]
    [SerializeField] private Material defaultMat;
    [SerializeField] private Material obstacleMat;
    [SerializeField] private Material startEndMat;
    [SerializeField] private Material openMat;
    [SerializeField] private Material closeMat;

    // To backtrack to get final path.
    [SerializeField] private Node cameFromNode;

    // Node State
    [SerializeField] public NodeState state { get; set; }
    [SerializeField] private Direction cameFromDirection;


    // Position
    private Vector2Int postition;

    // Cost
    private int gCost; // cost from start -> node
    private int hCost; // estimate cost this node -> end
    private int fCost; // distance g + h 

    // Labels
    [SerializeField] private GameObject labelHCost;
    [SerializeField] private GameObject labelGCost;
    [SerializeField] private GameObject labelFCost;
    [SerializeField] private GameObject labelStart;
    [SerializeField] private GameObject labelEnd;

    // Direction Arrow
    [SerializeField] private GameObject arrow;

    public void Start()
    {
        //gridManager = null;
        cameFromNode = null;
        cameFromDirection = Direction.NONE;

        state = NodeState.DEFAULT;
        SetCost(0, 0, 0);
    }

    public void Reset()
    {
        Start();
        UpdateAppearance();
    }

    public Node CameFromNode
    {
        get { return cameFromNode; }
        set 
        {    
            cameFromNode = value;
            UpdateCameFromNodeDirection();
        }
    }

    private void UpdateCameFromNodeDirection()
    {
        Vector2Int direction = postition - cameFromNode.postition;
        int hashedDirection = direction.x * 5 + direction.y;

        switch (hashedDirection)
        {
            case 0 + 1:
                cameFromDirection = Direction.SOUTH;
                break;
            case 1*5 + 0:
                cameFromDirection = Direction.EAST;
                break;
            case -1*5 + 0:
                cameFromDirection = Direction.WEST;
                break;
            case 0 - 1:
                cameFromDirection = Direction.NORTH;
                break;
            case 1*5 + 1:
                cameFromDirection = Direction.SOUTH_WEST;
                break;
            case -1*5 + 1:
                cameFromDirection = Direction.SOUTH_EAST;
                break;
            case 1*5 - 1:
                cameFromDirection = Direction.NORTH_EAST;
                break;
            case -1*5 - 1:
                cameFromDirection = Direction.NORTH_WEST;
                break;
        }
    }

    // Update Appearance: Material and Labels 
    public void UpdateAppearance()
    {
        //Debug.Log($"node[{postition.x}, {postition.y}] = state: {state}");
        UpdateMaterial();
        UpdateLabel();
        //UpdateArrow();
    }

    private void UpdateMaterial()
    {
        switch (state)
        {
            case NodeState.DEFAULT:
                ChangeMaterial(defaultMat);
                break;

            case NodeState.START:
                ChangeMaterial(startEndMat);
                break;

            case NodeState.END:
                ChangeMaterial(startEndMat);
                break;

            case NodeState.OBSTACLE:
                ChangeMaterial(obstacleMat);
                break;

            case NodeState.OPEN:
                ChangeMaterial(openMat);
                break;

            case NodeState.CLOSED:
                 ChangeMaterial(closeMat);
                break;

            case NodeState.PATH:
                ChangeMaterial(startEndMat);
                break;
        }
    }

    private void UpdateLabel()
    {
        UpdateLabelVisibility();
        UpdateCostLabelValues();
    }

    private void UpdateArrow()
    {
        arrow.SetActive(true);
        if(cameFromNode == null)
            return;

        Quaternion angle = new Quaternion();

        switch (cameFromDirection)
        {

            case Direction.NORTH:
                angle = Quaternion.Euler(90, 0f, 0f);
                break;
            case Direction.EAST:
                angle = Quaternion.Euler(90f, -90f,  0f);
                break;
            case Direction.SOUTH:
                angle = Quaternion.Euler(90, 180f, 0f);
                break;
            case Direction.WEST:
                angle = Quaternion.Euler(90, 90f, 0f);
                break;
            case Direction.NORTH_WEST:
                angle = Quaternion.Euler(90, 45f, 0f);
                break;
            case Direction.NORTH_EAST:
                angle = Quaternion.Euler(90,- 45f, 0f);
                break;
            case Direction.SOUTH_WEST:
                angle = Quaternion.Euler(90, -135f, 0f);
                break;
            case Direction.SOUTH_EAST:
                angle = Quaternion.Euler(90, 135f, 0f);
                break;
        }

        arrow.transform.transform.rotation = angle;
    }

    private void UpdateLabelVisibility()
    {
        // Start and End
        labelStart.SetActive(state == NodeState.START);
        labelEnd.SetActive(state == NodeState.END);
        
        // Cost
        void labelCostVisibiliity(bool b)
        {
            labelHCost.SetActive(b);
            labelGCost.SetActive(b);
            labelFCost.SetActive(b);
        }
        labelCostVisibiliity(state == NodeState.OPEN || state == NodeState.CLOSED || state == NodeState.PATH);
    }

    private void UpdateCostLabelValues()
    {
        labelHCost.GetComponent<TextMeshPro>().text = hCost.ToString();
        labelGCost.GetComponent<TextMeshPro>().text = gCost.ToString();
        labelFCost.GetComponent<TextMeshPro>().text = fCost.ToString();
    }

    private void ChangeMaterial(Material materialToChangeTo)
    {
        Renderer renderer = transform.Find("NodeMesh").GetComponent<Renderer>();

        Material[] materials = renderer.materials;
        if (materials[0].name != materialToChangeTo.name)
        {
            materials[0] = materialToChangeTo;
        }
        else
        {
            materials[0] = defaultMat;
        }
        renderer.materials = materials;
    }

    public List<Node> GetNeighbours()
    {
        Vector2Int pivot = Position;
        List<Node> neighbours = new List<Node>();

        // Neighbourhood Offset Positions
        int[,] offsets =
         {
            { -1,  0 },
            {  1,  0 },
            {  0,  1 },
            {  0, -1 },
            { -1,  1 },
            {  1,  1 },
            { -1, -1 },
            {  1, -1 },
        };

        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            Vector2Int neighbourPos = new Vector2Int(pivot.x + offsets[i, 0], pivot.y + offsets[i, 1]);
            GameObject neighbour = Grid.Instance.GetValue(neighbourPos);

            if (neighbour != null)
            {
                neighbours.Add(neighbour.GetComponent<Node>());
            }
        }
        return neighbours;
    }

    public override string ToString()
    {
        return $"({postition.x}, {postition.y})";
    }

    //public NodeState NodeState { get { return state; } set { state = value; } }

    public bool IsStart
    {
        get { return GridManager.Instance.NodeStart == GetComponent<GameObject>(); }
    }

    public bool IsEnd
    {
        get { return GridManager.Instance.NodeEnd == GetComponent<GameObject>(); }
    }

    public Vector2Int Position
    {
        get
        {
            return postition;
        }
        set 
        {
            postition = value;
        }
    }

    public int CompareTo(Node other)
    {
        if (fCost <= other.fCost)
            return -1;
        if (fCost == other.fCost)
            return 0;
        else
            return 1;
    }

    public void GetCost(out int g, out int h, out int f)
    {
        h = hCost;
        g = gCost;
        f = fCost;
    }

    public void SetCost(int g, int h, int f)
    {
        hCost = h;
        gCost = g;
        fCost = f;
    }

    public int HCost
    {
        get { return hCost; }
    }

    public int GCost
    {
        get { return gCost; }
    }

    public int FCost
    {
        get { return fCost; }
    }
}
