using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Watenk;

public class LightManager : IPhysicsUpdateable
{
    // Cache
    private int minLight;
    private int maxLight;

    // Dependencies
    private IGrid grid;

    //----------------------------------------

    public LightManager(){
        grid = GameManager.GetService<GridManager>();
        minLight = Settings.Instance.MinLight;
        maxLight = Settings.Instance.MaxLight;
    }

    public void OnPhysicsUpdate(){

        // LightSource
        for (int y = 0; y < 1; y++){
            for(int x = 0; x < grid.GridSize.x; x++){
                Cell currentCell = grid.GetCell(new Vector2Int(x, y));

                currentCell.Lightlevel = Random.Range(minLight, maxLight);
            }
        }

        // Other
        for (int y = 1; y < grid.GridSize.y; y++){
            for(int x = 0; x < grid.GridSize.x; x++){
                
                Vector2Int currentPos = new Vector2Int(x, y);
                Cell upCell = grid.GetCell(currentPos + -Vector2Int.up); 
                Cell currentCell = grid.GetCell(currentPos);

                if (upCell.Lightlevel == 0){
                    currentCell.Lightlevel = 0;
                }
                else if (currentCell.Cells != Cells.air){
                    currentCell.Lightlevel = upCell.Lightlevel - 1;
                }
                else{
                    currentCell.Lightlevel = upCell.Lightlevel;
                }
            }
        }
    }
}
