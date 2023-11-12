using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private WorldGrid grid;
    [SerializeField] private GridCursor cursor; 


    // Start is called before the first frame update
    void Start()
    {
        cursor.VisibleRegion = grid.GetGridSize();
    }

    public void UpdateGrid()
    {
        cursor.VisibleRegion = grid.GetGridSize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
