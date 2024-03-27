using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    // Dependencies
    private IGrid grid;
    private IRenderableGrid renderableGrid;

    //--------------------------------------

    public MapGenerator(){
        grid = GameManager.GetService<GridManager>();
        renderableGrid = GameManager.GetService<GridManager>();

        for (int y = grid.GridSize.y - 5; y < grid.GridSize.y; y++){
            for(int x = 0; x < grid.GridSize.x; x++){
                ref Cell currentCell = ref grid.GetCell(new Vector2Short(x, y));
                currentCell.CellType = CellTypes.dirt;
                renderableGrid.AddChangedCell(new Vector2Short(x, y));
            }
        }
    }
}
