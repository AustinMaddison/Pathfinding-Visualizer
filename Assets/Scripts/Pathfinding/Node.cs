using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private Grid2d<Node> grid;
    private Node cameFromNode;
    private int x;
    private int y;
    private bool isClosed;
    private bool isWalkable;

    // Heuristics
    private int hCost; // distance from start pos
    private int gCost; // distance from end pos
    private int fCost; // distance h + g

    public Node(Grid2d<Node> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.isClosed = false;
    }

    public void GetScore(out int h, out int g, out int f)
    {
        h = this.hCost;
        g = this.gCost;
        f = this.fCost;
    }

    public void SetScore(int h, int g, int f)
    {
        this.hCost = h;
        this.gCost = g;
        this.fCost = f;
    }

    public void SetSetCame(Node node)
    {
        this.cameFromNode = node;
    }

    public Node SetCameFrom(Node node)
    {
        return this.cameFromNode;
    }

    public override string ToString()
    {
        return x + ", " + y;
    }

}
