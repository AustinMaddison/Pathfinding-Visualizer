using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PathfinderFinnishStatus
{
    NONE,
    NO_PATH_FOUND,
    PATH_FOUND
}

public class PathfinderManager : MonoBehaviour
{
    // GRID MANAGER
    [SerializeField] private GridManager gridManager;

    // ALGORITHM
    [SerializeField] private PathfindersEnum selectedAlgorithm = PathfindersEnum.A_STAR;
    [SerializeField] private PathfinderInterface pathfinder;

    // RUN ITERATION SCHEDULING
    [SerializeField] private float runIterationSpeedSeconds = 1.0f;
    private float timePrevious;

    // ALGORITHM STATS
    [SerializeField] private int iteration;
    [SerializeField] private int distance;
    
    // FLAGS
    [SerializeField] private bool isPathfinderActive;
    [SerializeField] private bool isRunnning;
    [SerializeField] private bool isPathfinderFinnnished;
    //private bool isGridDirty => pathfinder == null;

    // BACKTRACK
    private Node backtrackNodeCurrent;

    void Start()
    {
        isRunnning = false;
        isPathfinderActive = false;
        pathfinder = null;
        backtrackNodeCurrent = null;
        timePrevious = Time.time;
        iteration = 0;
        isPathfinderFinnnished = false;
    }

    private void Reset()
    {
        ResetGridNodes();
        Start();
    }

    private void ResetGridNodes()
    {
        foreach(Node node in pathfinder.ClosedNodeSet)
        {
            if (node.NodeState == NodeState.START || 
                node.NodeState == NodeState.END)
                continue;

            node.Reset();
        }
        foreach(Node node in pathfinder.OpenNodeSet)
        {
            if (node.NodeState == NodeState.START ||
                node.NodeState == NodeState.END)
                continue;

            node.Reset();
        }
    }

    void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (isPathfinderFinnnished)
            return;

        if (Time.time - timePrevious >= runIterationSpeedSeconds)
        {
            if (isRunnning)
            {
                timePrevious = Time.time;
                RunIteration();
            }

            else if(isPathfinderActive)
            {
                if (pathfinder.IsPathfinderDone)
                    BacktrackPathIteration();
            }
        }
    }

    private void HandleInput()
    {
        // Change Algorithm
        if (!isPathfinderActive)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedAlgorithm = selectedAlgorithm.Previous();
                Debug.Log(Enum.GetName(selectedAlgorithm.GetType(), selectedAlgorithm));
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedAlgorithm = selectedAlgorithm.Next();
                Debug.Log(Enum.GetName(selectedAlgorithm.GetType(), selectedAlgorithm));
            }
        }

        // Run Pathfinder
        if (!isPathfinderFinnnished)
        {
            // step iteration 
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (!isRunnning)
                    RunIteration();
                else
                    isRunnning = false;
            }

            // run toggle
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isRunnning = !isRunnning;
            }
        }

        // Reset Pathdfinding
        if (isPathfinderActive && !isRunnning)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Reset();
            }
        }
    }

    private void RunIteration()
    {
        // instantiate algorithm
        if(!isPathfinderActive)
        {
            InitPathfinder();
            isPathfinderActive = true;
        }

        // Run Iteration
        pathfinder.RunIteration();

        // Path found -> back track
        if (pathfinder.IsPathfinderDone) 
        {
            backtrackNodeCurrent = gridManager.NodeEnd.GetComponent<Node>().CameFromNode;
            isRunnning = false;
        }

        iteration++;
    }

    private void InitPathfinder()
    {
        switch (selectedAlgorithm)
        {
            case PathfindersEnum.A_STAR:
                pathfinder = new AStarPathfinder(gridManager.NodeStart.GetComponent<Node>(), gridManager.NodeEnd.GetComponent<Node>());
                break;
        }
    }

    private void BacktrackPathIteration()
    {
        if (backtrackNodeCurrent.NodeState == NodeState.START)
        {
            isPathfinderFinnnished = true;
            return;
        }

        backtrackNodeCurrent.NodeState = NodeState.PATH;
        backtrackNodeCurrent.UpdateAppearance();
        backtrackNodeCurrent = backtrackNodeCurrent.CameFromNode;
    }

    static public int Cost(Vector2Int p1, Vector2Int p2)
    { 
        int distX = Mathf.Abs(p1.x - p2.x);
        int distY = Mathf.Abs(p1.y - p2.y);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }
}
