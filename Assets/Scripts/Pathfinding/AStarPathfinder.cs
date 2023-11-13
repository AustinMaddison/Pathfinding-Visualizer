using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class AStarPathfinder: PathfinderInterface
{
    private string pathfinderName = "A*";

    private Node nodeStart;
    private Node nodeEnd;

    private List<Node> nodeOpenList;
    private Grid grid;

    private bool isDone;

    public AStarPathfinder(Node nodeStart, Node nodeEnd, Grid grid)
    {
        this.nodeStart = nodeStart;
        this.nodeEnd = nodeEnd;
        this.grid = grid;

        // Start from start node.
        nodeOpenList = new List<Node>
        {
            this.nodeStart
        };

        isDone = false;
    }

    public override string ToString()
    {
        return pathfinderName;
    }
    public void RunIteration()
    {



        // Find best open node.
        Node nodeCurrent = nodeOpenList.Min();


        Debug.Log($"Minimum Node: {nodeCurrent}");


        // Close the best node for potential best path.
        CloseNode(nodeCurrent);
        nodeOpenList.Remove(nodeCurrent);

        // Open neighbours for best node.
        OpenNeighbours(nodeCurrent);


        string s = "OpenList: ";
        foreach (Node node in nodeOpenList) 
        {
            node.GetCost(out int h, out int g, out int f);
            s += $"{node}: {f}, "; 
        }
        Debug.Log(s);

    }

    private int CalculateDistanceCost(Vector2Int p1, Vector2Int p2)
    {
        int distX = Mathf.Abs(p1.x - p2.x);
        int distY = Mathf.Abs(p1.y - p2.y);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }

    private void OpenNeighbours(Node node)
    {
        Vector2Int pivot = node.Position;

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

        for (int i = 0; i < offsets.GetLength(0); i++ )
        {
            Vector2Int neighbourPos = new Vector2Int(pivot.x + offsets[i, 0], pivot.y + offsets[i, 1]);
            GameObject neighbour = grid.GetValue(neighbourPos);

            if (neighbour == null)
            {
                continue;
            }

            Node neighbourNode = neighbour.GetComponent<Node>();
            if (neighbourNode.NodeState != NodeState.OBSTACLE && 
                neighbourNode.NodeState != NodeState.CLOSED &&
                neighbourNode.NodeState != NodeState.START
                )
            {

                // Process neibour
                neighbourNode.SetCameFromNode(node);
                OpenNode(neighbourNode);
                if(!nodeOpenList.Contains(neighbourNode))
                    nodeOpenList.Add(neighbourNode);
            }
        }
    }

    private void OpenNode(Node node)
    {
        node.NodeState = NodeState.OPEN;

        int h = CalculateDistanceCost(node.Position, nodeStart.Position);
        int g = CalculateDistanceCost(node.Position, nodeEnd.Position);
        int f = h + g;
        node.SetCost(h, g, f);
        
        //Debug.Log($"startNode:{nodeStart}, node:{node}, {h}, {g}, {f}");

        node.UpdateAppearance();
    }

    private void CloseNode(Node node)
    {
        if (node.NodeState != NodeState.START)
        {
            node.NodeState = NodeState.CLOSED;
            node.UpdateAppearance();
        }
    }

}
