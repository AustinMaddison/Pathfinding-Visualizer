using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BFSPathfinder: PathfinderInterface
{
    private string pathfinderName = "Breadth First Search";

    private Node nodeStart;
    private Node nodeEnd;
    Node nodeCurrent;

    private Queue<Node> queue;
    private HashSet<Node> visited;

    public PathfinderStatus Status { get; private set; }
    public bool IsDone { get; private set; }

    public BFSPathfinder(Node nodeStart, Node nodeEnd)
    {
        this.nodeCurrent = null;
        this.nodeStart = nodeStart;
        this.nodeEnd = nodeEnd;

        queue = new Queue<Node>();
        visited = new HashSet<Node>();

        // Calculate startnode costs
        int h = PathfinderManager.Cost(nodeStart.Position, nodeEnd.Position);
        nodeStart.SetCost(0, h, h);

        // Start from start node.
        queue.Enqueue(this.nodeStart);
        IsDone = false;
    }
    
    public override string ToString()
    {
        return pathfinderName;
    }

    public void RunIteration()
    {
        if (queue.Count == 0)
        {   
            IsDone = true;
            Debug.Log("Failed to find optimal path.");
            Status = PathfinderStatus.NO_PATH_FOUND;
            return;
        }
        
        // Find node with best f cost.
        nodeCurrent = queue.Dequeue();
        visited.Add(nodeCurrent);

        //// Found end
        //if (nodeCurrent == nodeEnd)
        //{
        //    Debug.Log("Found optimal path.");
        //    Status = PathfinderStatus.PATH_FOUND;
        //    IsDone = true;
        //    return;
        //}

        // Close the best node for potential best path.
        CloseNode(nodeCurrent);

        //// Open neighbours for best node.
        OpenNeighbours(nodeCurrent);
    }

    private void OpenNeighbours(Node node)
    {
        foreach (Node neighbour in node.GetNeighbours())
        {
            if (visited.Contains(neighbour))
                continue;

            if (neighbour.state != NodeState.OBSTACLE &&
                neighbour.state != NodeState.CLOSED &&
                neighbour.state != NodeState.START
                )
            {
                // Calculate Cost
                neighbour.CameFromNode = node;

             

                int tentativeGScore = node.GCost + PathfinderManager.Cost(node.Position, neighbour.Position);
                int h = 0;
                int f = tentativeGScore + h;
                neighbour.SetCost(tentativeGScore, h, f);
                
                // Found end
                if (neighbour == nodeEnd)
                {
                    Debug.Log("Found optimal path.");
                    Status = PathfinderStatus.PATH_FOUND;
                    IsDone = true;
                    return;
                }

                if (neighbour.state != NodeState.END)
                {
                    neighbour.state = NodeState.OPEN;
                    neighbour.UpdateAppearance();
                }

                queue.Enqueue(neighbour);
                visited.Add(neighbour);

            }
        }
    }

    private void CloseNode(Node node)
    {
        if (node.state != NodeState.START)
        {
            node.state = NodeState.CLOSED;
            node.UpdateAppearance();
        }
    }

    public HashSet<Node> OpenNodeSet => new HashSet<Node>(queue);
    public HashSet<Node> ClosedNodeSet => visited;

    public int OpenNodesTotal => queue.Count;
    public int ClosedNodesTotal => visited.Count;



    public int Distance => nodeCurrent.GCost;
}
