using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour 
{
    private GridNode cameFromNode = null;

    private bool isClosed;
    private bool isWalkable;

    private int x;
    private int y;

    // Heuristics
    private int hCost; // distance from start pos
    private int gCost; // distance from end pos
    private int fCost; // distance h + g

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

    public void SetCameFromNode(GridNode node)
    {
        cameFromNode = node;
    }

    public GridNode GetCameFromNode(GridNode node)
    {
        return cameFromNode;
    }

    public override string ToString()
    {
        return Mathf.FloorToInt(transform.position.x) + ", " + Mathf.FloorToInt(transform.position.y);
    }

}
