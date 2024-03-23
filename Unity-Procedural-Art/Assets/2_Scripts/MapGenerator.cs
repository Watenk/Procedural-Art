using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    // Dependencies
    private ICellGrid grid;

    //--------------------------------------

    public MapGenerator(){
        grid = GameManager.GetService<CellGridManager>();

        for (int y = grid.GridSize.y - 5; y < grid.GridSize.y; y++){
            for(int x = 0; x < grid.GridSize.x; x++){
                ref Cell currentCell = ref grid.GetCell(new Vector2Int(x, y));
                currentCell = Cell.dirt;
            }
        }
    }
}
