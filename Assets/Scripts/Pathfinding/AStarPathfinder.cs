using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder: MonoBehaviour {

    [SerializeField] private GridManager gridManager;
    [SerializeField] private float runIterationSpeed = 1.0f;
    [SerializeField] private bool isRunnning = false;
    [SerializeField] private float timePrevious;
    
    private void Start()
    {
        timePrevious = Time.time;
    }

    private void FixedUpdate()
    {
        if (Time.time - timePrevious >= runIterationSpeed)
        {
            timePrevious = Time.time;
            RunIteration();
        }
    }

    private void RunIteration()
    {



        var p1 = new Vector2Int(0, 0);
        var p2 = new Vector2Int(1, 1);

        Debug.Log(CalculateDistanceCost(p1, p2));
    }

    private int CalculateDistanceCost(Vector2Int p1, Vector2Int p2)
    {
        //int DIAGONAL_DISTANCE = 14;
        //int UNIT_DISTANCE = 10;

        return Mathf.FloorToInt(Vector2Int.Distance(p1*10, p2*10));
    }

}
