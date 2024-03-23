using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Watenk;

public class LightManager : IPhysicsUpdateable
{
    private int[,] lightLevels;

    // Dependencies
    private GridManager gridManager;

    //----------------------------------------

    public LightManager(){
        gridManager = GameManager.GetService<GridManager>();

        lightLevels = new int[gridManager.GridSize.x, gridManager.GridSize.y];

        for (int y = 0; y < 1; y++){
            for(int x = 0; x < gridManager.GridSize.x; x++){
                lightLevels[x, y] = 5;
            }
        }

        for (int y = 1; y < gridManager.GridSize.y; y++){
            for(int x = 0; x < gridManager.GridSize.x; x++){
                lightLevels[x, y] = 0;
            }
        }
    }

    public void OnPhysicsUpdate(){
        CalcLightLevels();
    }

    public int GetLightLevel(Vector2Int pos){
        
        if (!GridUtility.IsInBounds(pos, Vector2.zero, new Vector2Int(gridManager.GridSize.x, gridManager.GridSize.y))) return -1;
        
        return lightLevels[pos.x, pos.y];
    }

    //--------------------------------------

    private void CalcLightLevels(){

        for (int y = 1; y < gridManager.GridSize.y; y++){
            for(int x = 0; x < gridManager.GridSize.x; x++){
                
                int upLightLevel = lightLevels[x, y - 1]; 
                if (upLightLevel == 0){
                    lightLevels[x, y] = 0;
                    break;
                }

                if (gridManager.GetCell(new Vector2Int(x, y)).Cells != Cells.air){
                    lightLevels[x, y] = upLightLevel - 1;
                }
                else{
                    lightLevels[x, y] = upLightLevel;
                }
            }
        }
    }
}
