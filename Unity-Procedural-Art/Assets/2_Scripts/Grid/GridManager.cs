using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watenk;

public class GridManager
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
                cells[x, y] = new Cell(Cells.air);
            }
        }
    }

    public Cell GetCell(Vector2Int pos){
        if (!isInBounds(pos)) return null;

        return cells[pos.x, pos.y];
    }

    public void SetCell(Vector2Int pos, Cells cells){
        if (!isInBounds(pos)) return;

        Cell cell = GetCell(pos);
        cell.SetCells(cells);
        changedCells.Add(pos);
    }

    public bool isInBounds(Vector2Int pos){
        if (GridUtility.IsInBounds(pos, Vector2Int.zero, GridSize)) return true;
        else return false;
    }

    public List<Vector2Int> GetChangedCells(){
        return changedCells;
    }

    public void ClearChangedCells(){
        changedCells.Clear();
    }
}
