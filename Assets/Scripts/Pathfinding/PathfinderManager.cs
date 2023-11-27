using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PathfinderStatus
{
    NONE,
    NO_PATH_FOUND,
    PATH_FOUND
}

public class PathfinderManager : MonoBehaviour
{
    // Singleton
    public static PathfinderManager Instance { get; private set; }

    // ALGORITHM
    [SerializeField] public PathfindersEnum selectedAlgorithm { get; private set; }
    private PathfinderInterface pathfinder;

    // RUN ITERATION SCHEDULING
    [SerializeField] private float runIterationSpeedSeconds = 1.0f;
    private float timePrevious;

    // ALGORITHM STATS
    [SerializeField] public int Iteration { get; set; }
    [SerializeField] public int OpenNodesTotal { get { if (pathfinder == null) { return 0; } else { return pathfinder.OpenNodesTotal; } } }
    [SerializeField] public int ClosedNodesTotal { get { if (pathfinder == null) { return 0; } else { return pathfinder.ClosedNodesTotal; } } }
    [SerializeField] public int Distance { get; set; }
    
    // FLAGS
    [SerializeField] public bool IsActive { get; private set; }
    [SerializeField] public bool IsIterationsRunnning { get; private set; }
    [SerializeField] public bool IsDone { get; private set; }
    [SerializeField] public bool IsBacktrackDone { get; private set; }

    // EVENTS
    public UnityEvent NoPathFoundEvent;
    public UnityEvent PathFoundEvent;
    public UnityEvent StatsChanged;
    public UnityEvent StatsReset;
    public UnityEvent PathfinderChanged;

    // BACKTRACK
    private Node backtrackNodeCurrent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

    }
    
    void Start()
    {
        selectedAlgorithm = PathfindersEnum.A_STAR;
        IsIterationsRunnning = false;
        IsActive = false;
        IsDone = false;
        IsBacktrackDone = false;

        pathfinder = null;
        backtrackNodeCurrent = null;
        timePrevious = Time.time;

        Iteration = 0;
        Distance = 0;
        
    }

    public void Reset()
    {
        PathfindersEnum tmp = selectedAlgorithm; 
        
        ResetGridNodes();
        Start();
        selectedAlgorithm = tmp;
        
        StatsReset.Invoke();
    }

    void Update()
    {
        if (!IsBacktrackDone)
        {
            if (Time.time - timePrevious >= runIterationSpeedSeconds)
            {
                if (IsIterationsRunnning)
                {
                    timePrevious = Time.time;
                    RunIteration();
                }

                else if (IsDone)
                {
                    if (pathfinder.Status == PathfinderStatus.PATH_FOUND)
                        BacktrackPathIteration();
                }
            }
        }
    }

    private void FixedUpdate()
    {
      
    }

    public void RunIteration()
    {
        // instantiate algorithm
        if (!IsActive)
        {
            InitPathfinder();
            IsActive = true;
        }
        
        // Run Iteration
        if (!IsDone)
        {
            pathfinder.RunIteration();
        }

        // Path found -> back track
        if (pathfinder.IsDone)
        {
            backtrackNodeCurrent = GridManager.Instance.NodeEnd.GetComponent<Node>().CameFromNode;
            IsDone = true;
            IsIterationsRunnning = false;
        }

        Distance = pathfinder.Distance;
        Iteration++;
        StatsChanged.Invoke();
    }

    public void ToggleRun()
    {
        IsIterationsRunnning = !IsIterationsRunnning;
    }

    private void ResetGridNodes()
    {
        foreach (Node node in pathfinder.ClosedNodeSet)
        {
            if (node.state == NodeState.START ||
                node.state == NodeState.END)
                continue;

            node.Reset();
        }
        foreach (Node node in pathfinder.OpenNodeSet)
        {
            if (node.state == NodeState.START ||
                node.state == NodeState.END)
                continue;

            node.Reset();
        }
    }

    private void InitPathfinder()
    {
        switch (selectedAlgorithm)
        {
            case PathfindersEnum.A_STAR:
                pathfinder = new AStarPathfinder(GridManager.Instance.NodeStart.GetComponent<Node>(), GridManager.Instance.NodeEnd.GetComponent<Node>());
                break;
            case PathfindersEnum.DIJKSTRAS:
                pathfinder = new DijkstrasPathfinder(GridManager.Instance.NodeStart.GetComponent<Node>(), GridManager.Instance.NodeEnd.GetComponent<Node>());
                break;
            case PathfindersEnum.BFS:
                pathfinder = new BFSPathfinder(GridManager.Instance.NodeStart.GetComponent<Node>(), GridManager.Instance.NodeEnd.GetComponent<Node>());
                break;
            case PathfindersEnum.GREEDY:
                pathfinder = new GreedyPathfinder(GridManager.Instance.NodeStart.GetComponent<Node>(), GridManager.Instance.NodeEnd.GetComponent<Node>());
                break;



        }
    }

    private void BacktrackPathIteration()
    {
        if(backtrackNodeCurrent == GridManager.Instance.NodeStart.GetComponent<Node>())
        {
            IsBacktrackDone = true;
            return;
        }

        backtrackNodeCurrent.state = NodeState.PATH;
        backtrackNodeCurrent.UpdateAppearance();
        backtrackNodeCurrent = backtrackNodeCurrent.CameFromNode;
    }

    // Select Algorithm
    public void SelectPreviousAlgorithm()
    {
        selectedAlgorithm = selectedAlgorithm.Previous();
        PathfinderChanged.Invoke();
    }

    public void SelectNextAlgorithm()
    {
        selectedAlgorithm = selectedAlgorithm.Next();
        PathfinderChanged.Invoke();
    }

    // Utilities
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
