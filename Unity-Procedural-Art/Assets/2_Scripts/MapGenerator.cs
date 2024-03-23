using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    // Dependencies
    private IGrid grid;

    //--------------------------------------

    public MapGenerator(){
        grid = GameManager.GetService<GridManager>();

        for (int y = grid.GridSize.y - 5; y < grid.GridSize.y; y++){
            for(int x = 0; x < grid.GridSize.x; x++){
                Cell currentCell = grid.GetCell(new Vector2Int(x, y));
                currentCell.Cells = Cells.dirt;
            }
        }
    }
}
