using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AStarPathfinder: PathfinderInterface
{

    PathfinderFinnishStatus finnishStatus = PathfinderFinnishStatus.NONE;
    private string pathfinderName = "A*";

    private Node nodeStart;
    private Node nodeEnd;

    private HashSet<Node> openNodeSet;
    private HashSet<Node> closedNodeSet;

    private bool isPathfinderDone;

    public AStarPathfinder(Node nodeStart, Node nodeEnd)
    {
        this.nodeStart = nodeStart;
        this.nodeEnd = nodeEnd;

        openNodeSet = new HashSet<Node>();
        closedNodeSet = new HashSet<Node>();

        // Calculate startnode costs
        int h = PathfinderManager.Cost(nodeStart.Position, nodeEnd.Position);
        nodeStart.SetCost(0, h, h);

        // Start from start node.
        openNodeSet.Add(this.nodeStart);

        isPathfinderDone = false;
    }

    public override string ToString()
    {
        return pathfinderName;
    }

    public void RunIteration()
    {
        if (openNodeSet.Count == 0)
        {   
            isPathfinderDone = true;
            Debug.Log("Failed to find optimal path.");
            finnishStatus = PathfinderFinnishStatus.NO_PATH_FOUND;
            return;
        }
        
        // Find node with best f cost.
        Node nodeCurrent = openNodeSet.Min();
        openNodeSet.Remove(nodeCurrent);

        // Found end
        if (nodeCurrent == nodeEnd)
        {
            Debug.Log("Found optimal path.");
            finnishStatus = PathfinderFinnishStatus.PATH_FOUND;
            isPathfinderDone = true;
            return;
        }

        // Close the best node for potential best path.
        CloseNode(nodeCurrent);

        // Open neighbours for best node.
        OpenNeighbours(nodeCurrent);
    }

    private void OpenNeighbours(Node node)
    {
        foreach (Node neighbour in node.GetNeighbours())
        {
            if (neighbour.NodeState != NodeState.OBSTACLE &&
                neighbour.NodeState != NodeState.CLOSED &&
                neighbour.NodeState != NodeState.START
                )
            {
                // criteria to choose best path
                //if (neighbour.NodeState == NodeState.OPEN)
                //Debug.Log(neighbour);
                int tentativeGScore = node.GCost + PathfinderManager.Cost(node.Position, neighbour.Position);

                if(tentativeGScore < neighbour.GCost || neighbour.NodeState != NodeState.OPEN)
                {
                    neighbour.CameFromNode = node;
                    
                    int h = PathfinderManager.Cost(neighbour.Position, nodeEnd.Position);
                    int f = tentativeGScore + h;
                    neighbour.SetCost(tentativeGScore, h, f);

                    if (neighbour.NodeState != NodeState.END)
                    {
                        neighbour.NodeState = NodeState.OPEN;
                        neighbour.UpdateAppearance();
                    }
                    openNodeSet.Add(neighbour);
                }
            }
        }
    }

    private void OpenNode(Node node)
    {
        node.NodeState = NodeState.OPEN;

        int h = PathfinderManager.Cost(node.Position, nodeStart.Position);
        int g = PathfinderManager.Cost(node.Position, nodeEnd.Position);
        int f = h + g;

        node.SetCost(h, g, f);
        node.UpdateAppearance();
    }

    private void CloseNode(Node node)
    {
        if (node.NodeState != NodeState.START)
        {
            closedNodeSet.Add(node);
            node.NodeState = NodeState.CLOSED;
            node.UpdateAppearance();
        }
    }

    public bool IsPathfinderDone => isPathfinderDone;
    public HashSet<Node> OpenNodeSet => openNodeSet;
    public HashSet<Node> ClosedNodeSet => closedNodeSet;
}
