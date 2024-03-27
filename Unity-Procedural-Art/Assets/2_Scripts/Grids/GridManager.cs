using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watenk;

public class GridManager : IGrid, IRenderableGrid, IUpdateable
{
    public Vector2Short GridSize { get; private set; }

    private Cell[,] cells;
    private List<Vector2Short> changedCells = new List<Vector2Short>();
    private GridRenderer gridRenderer;

    //-----------------------------------

    public GridManager(){

        GridSize = Settings.Instance.GridSize;

        cells = new Cell[GridSize.x, GridSize.y];
        for (int y = 0; y < GridSize.y; y++){
            for(int x = 0; x < GridSize.x; x++){
                Cell newCell = new Cell{
                    CellType = CellTypes.air
                };
                cells[x, y] = newCell;
            }
        }

        gridRenderer = new GridRenderer(this, this, Settings.Instance.CellTypeAtlas, Settings.Instance.CellTypeAtlasSize, Settings.Instance.CellTypeSpriteSize);
    }

    public void OnUpdate(){
        gridRenderer.OnUpdate();
    }

    public ref Cell GetCell(Vector2Short pos){
        return ref cells[pos.x, pos.y];
    }

    public bool IsInBounds(Vector2Short pos){
        if (GridUtility.IsInBounds(pos, Vector2Short.Zero, GridSize)) return true;
        else return false;
    }

    // Changed Cells
    public ref List<Vector2Short> GetChangedCells(){
        return ref changedCells;
    }

    public void ClearChangedCells(){
        changedCells.Clear();
    }

    public void AddChangedCell(Vector2Short pos){
        changedCells.Add(pos);
    }

}
