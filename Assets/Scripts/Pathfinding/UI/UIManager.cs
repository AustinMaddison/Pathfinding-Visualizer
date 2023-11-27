using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using XCharts.Runtime;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    // Statistics

    [Header("Table")]
    [SerializeField] private TextMeshProUGUI iterationTMP;
    [SerializeField] private TextMeshProUGUI distanceTMP;
    [SerializeField] private TextMeshProUGUI openedTMP;
    [SerializeField] private TextMeshProUGUI closedTMP;
    [SerializeField] private TextMeshProUGUI ratioTMP;
    
    [Header("Charts")]
    [SerializeField] private LineChart distanceChart;
    [SerializeField] private LineChart openedChart;
    [SerializeField] private LineChart closedChart;
    [SerializeField] private LineChart ratioChart;

    [Header("Algorithm Selector")]
    [SerializeField] private TextMeshProUGUI algorithmTMP;

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

    private void Start()
    {
        PathfinderManager.Instance.StatsChanged.AddListener(UpdateStats);
        PathfinderManager.Instance.StatsChanged.AddListener(UpdateStats);
        PathfinderManager.Instance.PathfinderChanged.AddListener(UpdateAlgorithmLabel);
        PathfinderManager.Instance.StatsReset.AddListener(ResetStats);

        ResetStats();
    }

    public void ResetStats()
    {
        // Reset Chart
        distanceChart.ClearData();
        openedChart.ClearData();
        closedChart.ClearData();
        ratioChart.ClearData();

        UpdateStats();
        UpdateAlgorithmLabel();
    }

    public void UpdateStats()
    {
        iterationTMP.text = $"{PathfinderManager.Instance.Iteration}";
        distanceTMP.text = $"{PathfinderManager.Instance.Distance}";
        openedTMP.text = $"{PathfinderManager.Instance.OpenNodesTotal}";
        closedTMP.text = $"{PathfinderManager.Instance.ClosedNodesTotal}";
        ratioTMP.text = $"{Ratio()}";

        distanceChart.AddData(0, PathfinderManager.Instance.Distance);
        openedChart.AddData(0, PathfinderManager.Instance.OpenNodesTotal);
        closedChart.AddData(0, PathfinderManager.Instance.ClosedNodesTotal);
        ratioChart.AddData(0, Ratio());


        Debug.Log(Ratio());
    }


    public void UpdateAlgorithmLabel()
    {
        switch (PathfinderManager.Instance.selectedAlgorithm)
        {
            case PathfindersEnum.A_STAR:
                algorithmTMP.text = "A* Pathfinder";
                break;
            case PathfindersEnum.DIJKSTRAS:
                algorithmTMP.text = "Dijktras's Pathfinder";
                break;
            case PathfindersEnum.BFS:
                algorithmTMP.text = "Breadth-First-Search";
                break;
            case PathfindersEnum.GREEDY:
                algorithmTMP.text = "Greedy-Best-First-Search";
                break;
        }
    }

    private double Ratio() {

        float res = (float)(PathfinderManager.Instance.OpenNodesTotal + PathfinderManager.Instance.ClosedNodesTotal) / (GridManager.Instance.gridSize.x * GridManager.Instance.gridSize.y);

        return (double)System.Math.Round((decimal)res, 2);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
