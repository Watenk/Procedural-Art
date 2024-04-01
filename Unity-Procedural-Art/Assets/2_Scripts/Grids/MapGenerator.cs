using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    // Dependencies
    private IGrid<Cell> cellGrid;
    private IGridRenderer gridRenderer;

    //--------------------------------------

    public MapGenerator(){
        cellGrid = GameManager.GetService<GridManager>().GetGrid<Cell>();
        gridRenderer = GameManager.GetService<GridRenderer>();

        for (int y = cellGrid.GridSize.y - 5; y < cellGrid.GridSize.y; y++){
            for(int x = 0; x < cellGrid.GridSize.x; x++){
                Cell currentCell = cellGrid.Get(new Vector2Short(x, y));
                currentCell.CellType = CellTypes.dirt;
                cellGrid.Set(new Vector2Short(x, y), currentCell);
                gridRenderer.Update(new Vector2Short(x, y));
            }
        }
    }
}
