using UnityEngine;
using Watenk;

public class LightManager : IPhysicsUpdateable
{
    public Vector2Short GridSize { get; private set; }

    // Cache
    private int minLight;
    private int maxLight;

    // Dependencies
    private IGrid<Light> lightGrid;
    private IGrid<Cell> cellGrid;

    //----------------------------------------

    public LightManager(){
        GridManager gridManager = GameManager.GetService<GridManager>();
        lightGrid = gridManager.GetGrid<Light>();
        cellGrid = gridManager.GetGrid<Cell>();
        GridSize = gridManager.GridSize;
        minLight = Settings.Instance.MinLight;
        maxLight = Settings.Instance.MaxLight + 1;
    }

    public void OnPhysicsUpdate(){

        // LightSource
        for (int y = 0; y < 1; y++){
            for(int x = 0; x < lightGrid.GridSize.x; x++){

                Light light = lightGrid.Get(new Vector2Short(x, y));

                light.LightLevel = (byte)Random.Range(minLight, maxLight);
                lightGrid.Set(new Vector2Short(x, y), light);
            }
        }

        // Other
        for (int y = 1; y < lightGrid.GridSize.y; y++){
            for(int x = 0; x < lightGrid.GridSize.x; x++){
                Cell upCell = cellGrid.Get(new Vector2Short(x, y - 1));
                Light upLight = lightGrid.Get(new Vector2Short(x, y - 1));
                Light light = lightGrid.Get(new Vector2Short(x, y));
                
                if (upLight.LightLevel != 0){
                    if (upCell.CellType != CellTypes.air){
                        light.LightLevel = (byte)(upLight.LightLevel - 1);
                    }
                    else{
                        light.LightLevel = upLight.LightLevel;
                    }
                }
                else{
                    light.LightLevel = 0;
                }

                lightGrid.Set(new Vector2Short(x, y), light);
            }
        }
    }
}
