using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    // Dependencies
    private GridManager gridManager;

    //--------------------------------------

    public MapGenerator(){
        gridManager = GameManager.GetService<GridManager>();

        for (int y = gridManager.GridSize.y - 5; y < gridManager.GridSize.y; y++){
            for(int x = 0; x < gridManager.GridSize.x; x++){
                gridManager.SetCell(new Vector2Int(x, y), Cells.dirt);
            }
        }
    }
}
