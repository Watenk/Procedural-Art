using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watenk;

public class GridManager : IGrid, IGridDrawable
{
    public Vector2Int GridSize { get; private set; }
    
    private Cell[,] cells;

    // Grid Rendering
    private List<Vector2Int> changedCells = new List<Vector2Int>();

    //-------------------------------------------

    public GridManager(){

        GridSize = Settings.Instance.GridSize;

        cells = new Cell[GridSize.x, GridSize.y];
        for (int y = 0; y < GridSize.y; y++){
            for(int x = 0; x < GridSize.x; x++){
                Cell newCell = new Cell(new Vector2Int(x, y), this);
                newCell.Cells = Cells.air;
                cells[x, y] = newCell;
            }
        }
    }

    public ref Cell GetCell(Vector2Int pos){
        if (!IsInBounds(pos)) Debug.Log("Tried to get Cell out of bounds");

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
