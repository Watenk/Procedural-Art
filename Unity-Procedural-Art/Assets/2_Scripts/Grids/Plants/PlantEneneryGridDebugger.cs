using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantEnergyGridDebugger : IPhysicsUpdateable
{
    private Text[,] energyTexts;
    private GameObject plantEnergyGridDebugPrefab;

    // Dependencies
    private IPlantGrid plantGrid;

    //--------------------------------

    public  PlantEnergyGridDebugger(){
        plantGrid = GameManager.GetService<PlantManager>();
        plantEnergyGridDebugPrefab = Settings.Instance.PlantEnergyGridDebugPrefab;

        energyTexts = new Text[plantGrid.GridSize.x, plantGrid.GridSize.y];
        for (int y = 0; y < plantGrid.GridSize.y; y++){
            for(int x = 0; x < plantGrid.GridSize.x; x++){
                GameObject gameObject = GameObject.Instantiate(plantEnergyGridDebugPrefab, new Vector3(x, -y, 0), Quaternion.identity);
                Text text = gameObject.GetComponentInChildren<Text>();
                energyTexts[x, y] = text;
            }
        }
    }

    public void OnPhysicsUpdate(){
        for (int y = 0; y < plantGrid.GridSize.y; y++){
            for(int x = 0; x < plantGrid.GridSize.x; x++){

                PlantCell plant = plantGrid.GetCell(new Vector2Short(x, y));
                if (plant == null) continue;
                
                energyTexts[x, y].text = plant.Energy.ToString();
            }
        }
    }
}
