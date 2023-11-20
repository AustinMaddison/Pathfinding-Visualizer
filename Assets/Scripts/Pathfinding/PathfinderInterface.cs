using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public interface PathfinderInterface
{
    // Returns true if end is found.
    void RunIteration();
    bool IsDone { get; }
    PathfinderStatus Status { get; }
    string ToString();
    HashSet<Node> OpenNodeSet{ get; }
    HashSet<Node> ClosedNodeSet{ get; }
    public int OpenNodesTotal { get; }
    public int ClosedNodesTotal { get; }

    int Distance{ get; }
}