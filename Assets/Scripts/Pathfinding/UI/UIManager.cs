using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [Header("Statisitcs")]
    [SerializeField] private TextMeshProUGUI iterationTMP;
    [SerializeField] private TextMeshProUGUI distanceTMP;
    [SerializeField] private TextMeshProUGUI openedTMP;
    [SerializeField] private TextMeshProUGUI closedTMP;
    [SerializeField] private TextMeshProUGUI ratioTMP;

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
        PathfinderManager.Instance.PathfinderChanged.AddListener(UpdateAlgorithmLabel);
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

        Debug.Log(Ratio());
    }

    public void UpdateAlgorithmLabel()
    {
        switch (PathfinderManager.Instance.selectedAlgorithm)
        {
            case PathfindersEnum.A_STAR:
                algorithmTMP.text = "A*";
                break;
            case PathfindersEnum.DIJKSTRAS:
                algorithmTMP.text = "Dijktras's";
                break;
            case PathfindersEnum.BFS:
                algorithmTMP.text = "BFS";
                break;
            case PathfindersEnum.GREEDY:
                algorithmTMP.text = "Greedy";
                break;
        }
    }

    private float Ratio() {

        return (float)(PathfinderManager.Instance.OpenNodesTotal + PathfinderManager.Instance.ClosedNodesTotal) / (GridManager.Instance.gridSize.x * GridManager.Instance.gridSize.y);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
