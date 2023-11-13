using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    // Grid
    [SerializeField] private GridManager gridManager;

    // ALGORITHM
    [SerializeField] private PathfindersEnum selectedAlgoritm = PathfindersEnum.A_STAR;
    [SerializeField] private PathfinderInterface pathfinder;

    // RUN ITERATION SCHEDULING
    [SerializeField] private float runIterationSpeedSeconds = 1.0f;
    private float timePrevious;
    private bool isRunnning;

    // ALGORITHM STATS
    private int iteration;
    private float timeElapsed;
    
    // FLAGS
    private bool isPathfinderActive;

    // CONSTANTS
    private int DIAGONAL_DISTANCE = 14;
    private int UNIT_DISTANCE = 10;

    void Start()
    {
        isRunnning = false;
        isPathfinderActive = false;

        timePrevious = Time.time;

        iteration = 0;
        timeElapsed = 0f;
    }

    private void Reset()
    {
        Start();
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Change Algorithm

        // Step 
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (!isRunnning)
                RunIteration();
            else
                isRunnning = false;
        }

        // Run toggle
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRunnning = !isRunnning;
        }

        // Reset
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Reset();
        }
    }

    private void FixedUpdate()
    {
        if (isRunnning && Time.time - timePrevious >= runIterationSpeedSeconds)
        {
            timePrevious = Time.time;
            RunIteration();
        }
    }

    private void initPathfinder()
    {
        switch(selectedAlgoritm)
        {
            case PathfindersEnum.A_STAR:
                pathfinder = new AStarPathfinder(gridManager.NodeStart.GetComponent<Node>(), gridManager.NodeEnd.GetComponent<Node>(), gridManager.Grid);
                break;
        }
    }

    private void RunIteration()
    {
        // instantiate algorithm
        if(!isPathfinderActive)
        {
            initPathfinder();
            isPathfinderActive = true;
        }
        
        // timer start
        float startTime = Time.time;
        isPathfinderActive = true;

        // Run Iteration
        pathfinder.RunIteration();

        // timer stop
        timeElapsed += Time.time - startTime;

        iteration++;
    }
}
