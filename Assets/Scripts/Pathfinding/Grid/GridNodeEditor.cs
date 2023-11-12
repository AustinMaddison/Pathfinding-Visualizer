using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridNodeEditor : MonoBehaviour
{
    [Header("Compenents")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GridCursor cursor;
    [SerializeField] private Grid grid;

    private bool isActive = true;

    private EditMode editMode = EditMode.NONE;

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
                Debug.Log("Mode: " + editMode);
            }
            else
            {
                editMode = newMode;
                Debug.Log("Mode: " + editMode);
            }
        }
    }


    // Set Grid Start, End and Obstacle Nodes
    private void UpdateNodeEdit()
    {
        if (cursor.IsOutBound || editMode == EditMode.NONE) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int nodePos = cursor.GetMouseNodePos;
            GameObject nodeToEdit = grid.GetValue(nodePos);

            GameObject tmpOldNode;
            switch (editMode)
            {
                // Cases for setting node state.
                case EditMode.SET_START:

                    tmpOldNode = null;
                    // start to start -> set deafult
                    if (nodeToEdit == gridManager.StartNode)
                    {
                        gridManager.StartNode = null;
                    }

                    // start to end 
                    else if (nodeToEdit == gridManager.EndNode)
                    {
                        tmpOldNode = gridManager.EndNode;

                        gridManager.EndNode = null;
                        gridManager.StartNode = nodeToEdit;
                    }

                    // obstacle to end
                    else if (!nodeToEdit.GetComponent<Node>().IsWalkable)
                    {
                        tmpOldNode = gridManager.StartNode;

                        nodeToEdit.GetComponent<Node>().IsWalkable = false;
                        gridManager.StartNode = nodeToEdit;
                    }

                    // empty
                    else
                    {
                        Debug.Log("change to start");
                        tmpOldNode = gridManager.StartNode;
                        gridManager.StartNode = nodeToEdit;

                        gridManager.StartNode.GetComponent<Node>().GetPosition(out int x, out int y);
                        Debug.Log($"GridmanagerStart: {x} , {y}");
                        Debug.Log($"isNodeTOEditStart?: {nodeToEdit.GetComponent<Node>().isStart}");
                    }

                    // Update Changed Node Appearance
                    nodeToEdit.GetComponent<Node>().UpdateAppearance();
                    if (tmpOldNode != null)
                        tmpOldNode.GetComponent<Node>().UpdateAppearance();

                    break;

                case EditMode.SET_END:

                    tmpOldNode = null;
                    // end to end -> set deafult
                    if (nodeToEdit == gridManager.EndNode)
                    {
                        gridManager.EndNode = null;
                    }

                    // start to end 
                    else if (nodeToEdit == gridManager.StartNode)
                    {
                        tmpOldNode = gridManager.StartNode;

                        gridManager.StartNode = null;
                        gridManager.EndNode = nodeToEdit;
                    }

                    // obstacle to end
                    else if (!nodeToEdit.GetComponent<Node>().IsWalkable)
                    {
                        tmpOldNode = gridManager.EndNode;

                        nodeToEdit.GetComponent<Node>().IsWalkable = false;
                        gridManager.EndNode = nodeToEdit;
                    }

                    // empty
                    else
                    {
                        tmpOldNode = gridManager.EndNode;
                        gridManager.EndNode = nodeToEdit;
                    }

                    // Update Changed Node Appearance
                    nodeToEdit.GetComponent<Node>().UpdateAppearance();
                    if (tmpOldNode != null)
                        tmpOldNode.GetComponent<Node>().UpdateAppearance();

                    break;

                case EditMode.SET_OBSTACLE:

                    tmpOldNode = null;
                    // obstacle to obstacle -> set deafult
                    if (!nodeToEdit.GetComponent<Node>().IsWalkable)
                    {
                        nodeToEdit.GetComponent<Node>().IsWalkable = false;
                    }

                    // start to obstacle 
                    else if (nodeToEdit == gridManager.StartNode)
                    {
                        tmpOldNode = gridManager.StartNode;

                        gridManager.StartNode = null;
                        nodeToEdit.GetComponent<Node>().IsWalkable = true;
                    }

                    // end to obstacle
                    else if (nodeToEdit == gridManager.EndNode)
                    {
                        tmpOldNode = gridManager.EndNode;

                        gridManager.EndNode = null;
                        nodeToEdit.GetComponent<Node>().IsWalkable = true;
                    }

                    // empty
                    else
                    {
                        nodeToEdit.GetComponent<Node>().IsWalkable = true;
                    }

                    // Update Changed Node Appearance
                    nodeToEdit.GetComponent<Node>().UpdateAppearance();
                    if (tmpOldNode != null)
                        tmpOldNode.GetComponent<Node>().UpdateAppearance();
                    break;
            }
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
