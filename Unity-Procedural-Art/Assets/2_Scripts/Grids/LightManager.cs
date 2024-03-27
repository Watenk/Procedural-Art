using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Watenk;

public class LightManager : IPhysicsUpdateable
{
    public Vector2Short GridSize { get; private set; }

    // Cache
    private int minLight;
    private int maxLight;

    // Dependencies
    private IGrid grid;

    //----------------------------------------

    public LightManager(){
        grid = GameManager.GetService<GridManager>();
        GridSize = grid.GridSize;
        minLight = Settings.Instance.MinLight;
        maxLight = Settings.Instance.MaxLight + 1;
    }

    public void OnPhysicsUpdate(){
        // LightSource
        for (int y = 0; y < 1; y++){
            for(int x = 0; x < grid.GridSize.x; x++){
                ref Cell currentCell = ref grid.GetCell(new Vector2Short(x, y));

                if (currentCell.CellType == CellTypes.air){
                    currentCell.LightLevel = (byte)Random.Range(minLight, maxLight);
                }
                else{
                    currentCell.LightLevel= 0;
                }
            }
        }

        // Other
        for (int y = 1; y < grid.GridSize.y; y++){
            for(int x = 0; x < grid.GridSize.x; x++){
                ref Cell upCell = ref grid.GetCell(new Vector2Short(x, y - 1));
                ref Cell currentCell = ref grid.GetCell(new Vector2Short(x, y));
                
                if (upCell.LightLevel != 0){
                    if (upCell.CellType != CellTypes.air){
                        currentCell.LightLevel = (byte)(upCell.LightLevel - 1);
                    }
                    else{
                        currentCell.LightLevel = upCell.LightLevel;
                    }
                }
                else{
                    currentCell.LightLevel = 0;
                }
            }
        }
    }
}
