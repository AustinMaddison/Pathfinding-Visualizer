using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class StateManager : MonoBehaviour
{
    // Singleton
    public static StateManager Instance { get; private set; }

    // Events
    public UnityEvent ResetPathfinder;

    // Settings
    [Header("Grid Settings")]
    [SerializeField] public InputActions inputActions;
    [SerializeField] public Vector2Int gridSize = new Vector2Int(5, 5);


    // Statistics
    [SerializeField] public int Distance { get; set; }
    [SerializeField] public int OpenNodeTotal { get; private set; }
    [SerializeField] public int CloseNodeTotal { get; private set; }


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

        inputActions = new InputActions();

    }

    
    // Start is called before the first frame update
    void Start()
    {
        inputActions.Pathfinder.Enable();
        inputActions.GridEditor.Enable();
        InputHandler();

        ResetPathfinder.AddListener(() =>
        {
            if (PathfinderManager.Instance.IsActive)
            {
                GridNodeEditor.Instance.GridNodeEditorEnable.Invoke();
                PathfinderManager.Instance.Reset();
            }
        });
    }

    void InputHandler()
    {
        static bool isSafeToRun()
        {
            return (GridManager.Instance.NodeStart != null && GridManager.Instance.NodeEnd != null) && !PathfinderManager.Instance.IsDone;
        }
        
        // Run Pathfinder Algorithm
        inputActions.Pathfinder.RunIteration.started += context =>
        {
            // Start and Node is set then can run iteration
            if (isSafeToRun())
            {
                GridNodeEditor.Instance.GridNodeEditorDisable.Invoke();
                if (PathfinderManager.Instance.IsIterationsRunnning)
                {
                    PathfinderManager.Instance.ToggleRun();
                    return;
                }

                PathfinderManager.Instance.RunIteration();
            }

        };

        inputActions.Pathfinder.ToggleRun.started += context =>
        {
            // Start and Node is set then can run
            if (isSafeToRun())
            {
                GridNodeEditor.Instance.GridNodeEditorDisable.Invoke();
                PathfinderManager.Instance.ToggleRun();
            }
        };

        inputActions.Pathfinder.Reset.started += context =>
        {
            ResetPathfinder.Invoke();
        };

        // Change Algorithms
        inputActions.Pathfinder.NextAlgorithm.started += context =>
        {
            if (!PathfinderManager.Instance.IsActive)
            {
                PathfinderManager.Instance.SelectNextAlgorithm();
            }
        };

        inputActions.Pathfinder.PreviousAlgorithm.started += context =>
        {
            if (!PathfinderManager.Instance.IsActive)
            {
                PathfinderManager.Instance.SelectPreviousAlgorithm();
            }
        };

        // Grid Editor Modes
        inputActions.GridEditor.ToggleModeStart.started += context =>
        {
            if (GridNodeEditor.Instance.Mode == GridNodeEditor.EditMode.SET_START)
                GridNodeEditor.Instance.Mode = GridNodeEditor.EditMode.NONE;
            else
                GridNodeEditor.Instance.Mode = GridNodeEditor.EditMode.SET_START;
        };

        inputActions.GridEditor.ToggleModeEnd.started += context =>
        {
            if (GridNodeEditor.Instance.Mode == GridNodeEditor.EditMode.SET_END)
                GridNodeEditor.Instance.Mode = GridNodeEditor.EditMode.NONE;
            else
                GridNodeEditor.Instance.Mode = GridNodeEditor.EditMode.SET_END;
        };

        inputActions.GridEditor.ToggleModeObstacles.started += context =>
        {
            if (GridNodeEditor.Instance.Mode == GridNodeEditor.EditMode.SET_OBSTACLE)
                GridNodeEditor.Instance.Mode = GridNodeEditor.EditMode.NONE;
            else
                GridNodeEditor.Instance.Mode = GridNodeEditor.EditMode.SET_OBSTACLE;
        };
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void ResetStats() { OpenNodeTotal++; }
    public void IncrementOpenNode() { OpenNodeTotal++; }
    public void IncrementCloseNode() { OpenNodeTotal++; }
}
