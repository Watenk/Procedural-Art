using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightGridDebugger : IPhysicsUpdateable
{
    private Text[,] debugTexts;

    // Cache
    private GameObject lightGridDebugPrefab;

    // Dependencies
    private ILightGrid lightGrid;

    //-------------------------------------

    public LightGridDebugger(){
        lightGrid = GameManager.GetService<LightManager>();
        lightGridDebugPrefab = Settings.Instance.LightGridDebugPrefab;

        debugTexts = new Text[lightGrid.GridSize.x, lightGrid.GridSize.y];
        for (int y = 0; y < lightGrid.GridSize.y; y++){
            for(int x = 0; x < lightGrid.GridSize.x; x++){
                GameObject gameObject = GameObject.Instantiate(lightGridDebugPrefab, new Vector3(x, -y, 0), Quaternion.identity);
                debugTexts[x, y] = gameObject.GetComponentInChildren<Text>();
            }
        }
    }

    public void OnPhysicsUpdate(){
        for (int y = 0; y < lightGrid.GridSize.y; y++){
            for(int x = 0; x < lightGrid.GridSize.x; x++){
                ref byte lightLevel = ref lightGrid.GetCell(new Vector2Short(x, y));
                debugTexts[x, y].text = lightLevel.ToString();
            }
        }
    }
}
