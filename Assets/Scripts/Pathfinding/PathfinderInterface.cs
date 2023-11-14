using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PathfinderInterface
{
    // Returns true if end is found.
    void RunIteration();
    bool IsPathfinderDone { get; }
    string ToString();
    HashSet<Node> OpenNodeSet{ get; }
    HashSet<Node> ClosedNodeSet{ get; }
}