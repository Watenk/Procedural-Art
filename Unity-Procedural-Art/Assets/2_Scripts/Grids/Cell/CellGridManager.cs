using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watenk;

public class CellGridManager : ICellGrid, ICellGridDrawable
{
    public Vector2Int GridSize { get; private set; }
    
    private Cell[,] cells;

    // Grid Rendering
    private List<Vector2Int> changedCells = new List<Vector2Int>();

    //-------------------------------------------

    public CellGridManager(){

        GridSize = Settings.Instance.GridSize;

        cells = new Cell[GridSize.x, GridSize.y];
        for (int y = 0; y < GridSize.y; y++){
            for(int x = 0; x < GridSize.x; x++){
                cells[x, y] = Cell.air;
            }
        }
    }

    public ref Cell GetCell(Vector2Int pos){
        return ref cells[pos.x, pos.y];
    }

    public bool IsInBounds(Vector2Int pos){
        if (GridUtility.IsInBounds(pos, Vector2Int.zero, GridSize)) return true;
        else return false;
    }

    // Changed Cells
    public List<Vector2Int> GetChangedCells(){
        return changedCells;
    }

    public void ClearChangedCells(){
        changedCells.Clear();
    }

    public void AddChangedCell(Vector2Int pos){
        changedCells.Add(pos);
    }
}
