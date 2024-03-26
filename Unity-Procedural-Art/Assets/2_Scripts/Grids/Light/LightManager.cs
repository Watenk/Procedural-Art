using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Watenk;

public class LightManager : IPhysicsUpdateable, ILightGrid
{
    public Vector2Short GridSize { get; private set; }

    private byte[,] lightLevels;

    // Cache
    private int minLight;
    private int maxLight;

    // Dependencies
    private ICellGrid cellGrid;

    //----------------------------------------

    public LightManager(){
        cellGrid = GameManager.GetService<CellGridManager>();
        GridSize = cellGrid.GridSize;
        minLight = Settings.Instance.MinLight;
        maxLight = Settings.Instance.MaxLight + 1;

        lightLevels = new byte[GridSize.x, GridSize.y];
    }

    public ref byte GetCell(Vector2Short pos){
        return ref lightLevels[pos.x, pos.y];
    }

    public bool IsInBounds(Vector2Short pos){
        if (GridUtility.IsInBounds(pos, Vector2Short.Zero, GridSize)) return true;
        else return false;
    }

    public void OnPhysicsUpdate(){
        // LightSource
        for (int y = 0; y < 1; y++){
            for(int x = 0; x < cellGrid.GridSize.x; x++){
                ref Cell currentCell = ref cellGrid.GetCell(new Vector2Short(x, y));

                if (currentCell == Cell.air){
                    lightLevels[x, y] = (byte)Random.Range(minLight, maxLight);
                }
                else{
                    lightLevels[x, y] = 0;
                }
            }
        }

        // Other
        for (int y = 1; y < cellGrid.GridSize.y; y++){
            for(int x = 0; x < cellGrid.GridSize.x; x++){
                
                ref byte upLightLevel = ref lightLevels[x, y - 1];
                if (upLightLevel != 0){
                    ref Cell upCell = ref cellGrid.GetCell(new Vector2Short(x, y - 1));
                    if (upCell != Cell.air){
                        lightLevels[x, y] = (byte)(upLightLevel - 1);
                    }
                    else{
                        lightLevels[x, y] = upLightLevel;
                    }
                }
                else{
                    lightLevels[x, y] = 0;
                }
            }
        }
    }
}
