using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GridNodeEditor : MonoBehaviour
{
    public enum EditMode 
    { 
        NONE,
        SET_START,
        SET_END,
        SET_OBSTACLE
    }

    // Singleton
    public static GridNodeEditor Instance { get; private set; }

    // Events
    public UnityEvent GridNodeEditorModeChanged;
    public UnityEvent GridNodeEditorEnable;
    public UnityEvent GridNodeEditorDisable;

    // Flags
    [SerializeField] public bool isActive = true;
    [SerializeField] private bool isDragging = false;
    private Vector2Int lastEditPos = new Vector2Int(-1, -1);
    [SerializeField] public EditMode Mode { get; set; }


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

        SetEventListeners();
    }

    private void Start()
    {
        Mode = EditMode.NONE;
    }

    private void SetEventListeners() 
    { 
        GridNodeEditorEnable.AddListener(() => isActive = true);
        GridNodeEditorEnable.AddListener(() => StateManager.Instance.inputActions.GridEditor.Enable());
        GridNodeEditorEnable.AddListener(() => Debug.Log("GridNodeEditor: Enabled"));

        GridNodeEditorDisable.AddListener(() => isActive = false);
        GridNodeEditorDisable.AddListener(() => StateManager.Instance.inputActions.GridEditor.Disable());
        GridNodeEditorDisable.AddListener(() => Debug.Log("GridNodeEditor: Disable"));
    }

    private void Update()
    {
        if (!isActive) return;
        HandleInput();
    }

    private void HandleInput()
    {
        //UpdateMode();
        UpdateNodeEdit();
    }
   
    // Update Edit Mode
    private void UpdateMode()
    {
        EditMode newMode = EditMode.NONE;
        bool pressedDown = false;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        { 
            newMode = EditMode.SET_START;
            pressedDown = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            newMode = EditMode.SET_END;
            pressedDown = true;
        }    
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            newMode = EditMode.SET_OBSTACLE;
            pressedDown = true;
        }

        // Modify only if a change occured;
        if (pressedDown)
        {
            if (newMode == Mode)
            {
                Mode = EditMode.NONE;
                //Debug.Log("Mode: " + editMode);
            }
            else
            {
                Mode = newMode;
                //Debug.Log("Mode: " + editMode);
            }
        }
    }

    // Set Grid Start, End and Obstacle Nodes
    private void UpdateNodeEdit()
    {
        
        if (Cursor.Instance.IsOutBound || Mode == EditMode.NONE) return;

        // Hold Drag Mechanic
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            lastEditPos = new Vector2Int(-1, -1); // reset
        }

        //if (Input.GetMouseButtonUp(0))
        //    isDragging = false;

        if (isDragging && Cursor.Instance.GetMouseNodePos != lastEditPos)
        {
            lastEditPos = Cursor.Instance.GetMouseNodePos;
            GameObject node = Grid.Instance.GetValue(Cursor.Instance.GetMouseNodePos);
            
            switch (Mode) 
            {   
                case EditMode.SET_START:
                    SetNodeStart(node);
                    break;

                case EditMode.SET_END:
                    SetNodeEnd(node);
                    break;

                case EditMode.SET_OBSTACLE:
                    SetNodeObstacle(node);
                    break;
            }
            node.GetComponent<Node>().UpdateAppearance();
        }
    }

    private void SetNodeStart(GameObject node)
    {
        Node nodeData = node.GetComponent<Node>();
        NodeState nodeState = nodeData.state;

        // Case start to start -> default
        if(nodeState == NodeState.START)
        {
            nodeData.state = NodeState.DEFAULT;
        }
        RemovePreviousNodeStart();

        // Rest of cases
        switch (nodeState)
        {
            case NodeState.END:
                nodeData.state = NodeState.START;
                RemovePreviousNodeEnd();
                SetGridNodeStart(node);
                break;

            case NodeState.OBSTACLE:
                nodeData.state = NodeState.START;
                SetGridNodeStart(node);
                break;

            case NodeState.DEFAULT:
                nodeData.state = NodeState.START;
                SetGridNodeStart(node);   
                break;
        }

        if (nodeData.state == NodeState.START)
        {
            SetGridNodeStart(node);
        }
    }

    private void SetNodeEnd(GameObject node)
    {
        Node nodeData = node.GetComponent<Node>();
        NodeState nodeState = nodeData.state;

        // Case start to end -> end
        if (nodeState == NodeState.END)
        {
            nodeData.state = NodeState.DEFAULT;
        }
        RemovePreviousNodeEnd();

        // Rest of cases
        switch (nodeState)
        {
            case NodeState.START:
                RemovePreviousNodeStart();
                nodeData.state = NodeState.END;
                break;

            case NodeState.OBSTACLE:
                nodeData.state = NodeState.END;
                break;

            case NodeState.DEFAULT:
                nodeData.state = NodeState.END;
                break;

            default:
                break;
        }

        if (nodeData.state == NodeState.END) 
        { 
            SetGridNodeEnd(node);
        }
    }

    private void SetGridNodeStart(GameObject node)
    {
        GridManager.Instance.NodeStart = node;
    }

    private void SetGridNodeEnd(GameObject node)
    {
        GridManager.Instance.NodeEnd = node;
    }

    private void RemovePreviousNodeStart()
    {
        GameObject startNode = GridManager.Instance.NodeStart;
        if (startNode != null)
        {
            startNode.GetComponent<Node>().state = NodeState.DEFAULT;
            startNode.GetComponent<Node>().UpdateAppearance();
            GridManager.Instance.NodeStart = null;
        }
    }

    private void RemovePreviousNodeEnd()
    {
        GameObject endNode = GridManager.Instance.NodeEnd;
        if(endNode != null)
        {
            endNode.GetComponent<Node>().state = NodeState.DEFAULT;
            endNode.GetComponent<Node>().UpdateAppearance();
            GridManager.Instance.NodeEnd = null;
        }    
    }

    private void SetNodeObstacle(GameObject node)
    { 
        Node nodeData = node.GetComponent<Node>();
        NodeState nodeState = nodeData.state;

        switch (nodeState)
        {
            case NodeState.OBSTACLE:
                nodeData.state = NodeState.DEFAULT;
                break;

            case NodeState.START:
                nodeData.state = NodeState.OBSTACLE;
                break;

            case NodeState.END:
                nodeData.state = NodeState.OBSTACLE;
                break;

            case NodeState.DEFAULT:
                nodeData.state = NodeState.OBSTACLE;
                break;

            default:
                break;
        }
    }

}
