using NUnit.Framework;
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
    CLOSED
}

public class Node : MonoBehaviour
{
   

    [Header("Node Type Materials")]
    [SerializeField] private Material defaultMat;
    [SerializeField] private Material obstacleMat;
    [SerializeField] private Material startEndMat;
    [SerializeField] private Material openMat;
    [SerializeField] private Material closeMat;

    private GridManager gridManager = null;

    // To backtrack to get final path.
    private Node cameFromNode = null;

    // Node State
    NodeState state = NodeState.DEFAULT;


    // Position
    private int x;
    private int y;

    // Heuristics
    private int hCost = 0; // distance from start pos
    private int gCost = 0; // distance from end pos
    private int fCost = 0; // distance h + g

    // Labels
    [SerializeField] private GameObject labelHCost;
    [SerializeField] private GameObject labelGCost;
    [SerializeField] private GameObject labelFCost;
    [SerializeField] private GameObject labelStart;
    [SerializeField] private GameObject labelEnd;

    public void Start()
    {
        
    }
 
    public void SetCellType()
    {

    }

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void GetPosition(out int x, out int y)
    {
        x = this.x;
        y = this.y;
    }

    public void GetScore(out int h, out int g, out int f)
    {
        h = hCost;
        g = gCost;
        f = fCost;
    }

    public void SetScore(int h, int g, int f)
    {
        hCost = h;
        gCost = g;
        fCost = f;
    }

    public void SetCameFromNode(Node node)
    {
        cameFromNode = node;
    }

    public Node GetCameFromNode(Node node)
    {
        return cameFromNode;
    }

    // Update Appearance: Material and Labels 
    public void UpdateAppearance()
    {
        Debug.Log($"node[{x}, {y}] = state: {state}");
        UpdateMaterial();
        UpdateLabel();
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
        }
    }

    private void UpdateLabel()
    {
        UpdateLabelVisibility();
        UpdateCostLabelValues();
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
        labelCostVisibiliity(state == NodeState.OPEN || state == NodeState.CLOSED);
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

    public override string ToString()
    {
        return Mathf.FloorToInt(transform.position.x) + ", " + Mathf.FloorToInt(transform.position.y);
    }

    public GridManager SetGridManager
    {
        set { gridManager = value; }
    }

    public NodeState NodeState { get { return state; } set { state = value; } }

    public bool isStart
    {
        get { return gridManager.StartNode == GetComponent<GameObject>(); }
    }

    public bool isEnd
    {
        get { return gridManager.EndNode == GetComponent<GameObject>(); }
    }
}
