using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GridNodeEditor : MonoBehaviour
{
    [Header("Compenents")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GridCursor cursor;
    [SerializeField] private Grid grid;

    [SerializeField] private bool isActive = true;
    [SerializeField] private bool isDragging = false;
    private Vector2Int lastEditPos = new Vector2Int(-1, -1);

    [SerializeField] private EditMode editMode = EditMode.NONE;

    //private LeftClickDown

    public enum EditMode 
    { 
        NONE,
        SET_START,
        SET_END,
        SET_OBSTACLE
    }

    private void Update()
    {
        if (!isActive) return;
        HandleInput();
    }

    private void HandleInput()
    {
        UpdateMode();
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
            if (newMode == editMode)
            {
                editMode = EditMode.NONE;
                //Debug.Log("Mode: " + editMode);
            }
            else
            {
                editMode = newMode;
                //Debug.Log("Mode: " + editMode);
            }
        }
    }

    // Set Grid Start, End and Obstacle Nodes
    private void UpdateNodeEdit()
    {
        
        if (cursor.IsOutBound || editMode == EditMode.NONE) return;

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

        if (isDragging && cursor.GetMouseNodePos != lastEditPos)
        {
            lastEditPos = cursor.GetMouseNodePos;
            GameObject node = grid.GetValue(cursor.GetMouseNodePos);
            
            switch (editMode) 
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
        NodeState nodeState = nodeData.NodeState;

        // Case start to start -> default
        if(nodeState == NodeState.START)
        {
            nodeData.NodeState = NodeState.DEFAULT;
        }
        RemovePreviousNodeStart();

        // Rest of cases
        switch (nodeState)
        {
            case NodeState.END:
                nodeData.NodeState = NodeState.START;
                SetGridNodeStart(node);
                break;

            case NodeState.OBSTACLE:
                nodeData.NodeState = NodeState.START;
                SetGridNodeStart(node);
                break;

            case NodeState.DEFAULT:
                nodeData.NodeState = NodeState.START;
                SetGridNodeStart(node);   
                break;
        }

        if (nodeData.NodeState == NodeState.START)
        {
            SetGridNodeStart(node);
        }
    }

    private void SetNodeEnd(GameObject node)
    {
        Node nodeData = node.GetComponent<Node>();
        NodeState nodeState = nodeData.NodeState;

        // Case start to end -> end
        if (nodeState == NodeState.END)
        {
            nodeData.NodeState = NodeState.DEFAULT;
        }
        RemovePreviousNodeEnd();

        // Rest of cases
        switch (nodeState)
        {
            case NodeState.START:
                nodeData.NodeState = NodeState.END;
                break;

            case NodeState.OBSTACLE:
                nodeData.NodeState = NodeState.END;
                break;

            case NodeState.DEFAULT:
                nodeData.NodeState = NodeState.END;
                break;

            default:
                break;
        }

        if (nodeData.NodeState == NodeState.END) 
        { 
            SetGridNodeEnd(node);
        }
    }

    private void SetGridNodeStart(GameObject node)
    {
        gridManager.NodeStart = node;
    }

    private void SetGridNodeEnd(GameObject node)
    {
        gridManager.NodeEnd = node;
    }

    private void RemovePreviousNodeStart()
    {
        GameObject startNode = gridManager.NodeStart;
        if (startNode != null)
        {
            startNode.GetComponent<Node>().NodeState = NodeState.DEFAULT;
            startNode.GetComponent<Node>().UpdateAppearance();
            gridManager.NodeStart = null;
        }
    }

    private void RemovePreviousNodeEnd()
    {
        GameObject endNode = gridManager.NodeEnd;
        if(endNode != null)
        {
            endNode.GetComponent<Node>().NodeState = NodeState.DEFAULT;
            endNode.GetComponent<Node>().UpdateAppearance();
            gridManager.NodeEnd = null;
        }    
    }

    private void SetNodeObstacle(GameObject node)
    { 
        Node nodeData = node.GetComponent<Node>();
        NodeState nodeState = nodeData.NodeState;

        switch (nodeState)
        {
            case NodeState.OBSTACLE:
                nodeData.NodeState = NodeState.DEFAULT;
                break;

            case NodeState.START:
                nodeData.NodeState = NodeState.OBSTACLE;
                break;

            case NodeState.END:
                nodeData.NodeState = NodeState.OBSTACLE;
                break;

            case NodeState.DEFAULT:
                nodeData.NodeState = NodeState.OBSTACLE;
                break;

            default:
                break;
        }
    }

    // Setters for GridManger
    public Grid SetGrid 
    {
        set { grid = value; }
    }

    public GridCursor SetCursor
    {
        set { cursor = value; }
    }

    public EditMode GetEditMode
    {
        get { return editMode; }
    }
}
