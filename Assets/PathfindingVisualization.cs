using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PathfindingVisualization {

    private int width;
    private int height;

    // Grid node data
    private Grid2d<Node> gridNodeData;

    // Text rendering
    private GameObject nodeLabelObject;
    private  GameObject[,] nodeLabelObjectArr;
    
    // Grid rendering
    private GameObject gridObject;
    private Texture2D gridNodeColorTex;
    private Material gridMaterial;

    public PathfindingVisualization(Grid2d<Node> gridNodeData)
    {
        int width = gridNodeData.GetWidth();
        int height = gridNodeData.GetHeight();

        // Grid node label
        nodeLabelObjectArr = new GameObject[width, height];

        // Grid primitive
        nodeLabelObject = Resources.Load<GameObject>("NodeLabels");
        gridObject = GameObject.CreatePrimitive(PrimitiveType.Plane);

        // Grid node color texture
        gridNodeColorTex = new Texture2D(gridNodeData.GetHeight(), gridNodeData.GetHeight(), TextureFormat.Alpha8, false);
        gridNodeColorTex.filterMode = FilterMode.Point;
        gridMaterial.SetTexture("gridNodeColorTex", gridNodeColorTex); // Based on node state, start, end, open, closed, final path.

        //UpdateGridNodeColorTex();
    
    }

    //public void Update()
    //{
    //    UpdateGridNodeColorTex();
    //    UpdateGridNodeLabel();
    //}

    public void Update(List<Node> nodesToUpdate)
    {
        UpdateGridNodeColorTex(nodesToUpdate);
        UpdateGridNodeLabel(nodesToUpdate);
    }

    private void SetNodeColorTex()
    {
        gridNodeColorTex.Apply();
    }

    private void UpdateGridNodeColorTex(List<Node> nodesToUpdate)
    {
        gridNodeColorTex.Apply();
    }

    private void UpdateGridNodeLabel()
    {

    }

    private void UpdateGridNodeLabel(List<Node> nodesToUpdate)
    {

    }

}
