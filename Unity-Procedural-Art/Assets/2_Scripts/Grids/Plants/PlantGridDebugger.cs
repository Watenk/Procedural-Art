using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantGridDebugger : IPhysicsUpdateable
{
    private Text[,] mainDebugTexts;
    private Text[,] upDebugTexts;
    private Text[,] leftDebugTexts;
    private Text[,] downDebugTexts;
    private Text[,] rightDebugTexts;

    // Cache
    private GameObject plantGridDebugPrefab;

    // Dependencies
    private IPlantGrid plantGrid;

    //-------------------------------------

    public PlantGridDebugger(){
        plantGrid = GameManager.GetService<PlantManager>();
        plantGridDebugPrefab = Settings.Instance.PlantGridDebugPrefab;

        mainDebugTexts = new Text[plantGrid.GridSize.x, plantGrid.GridSize.y];
        upDebugTexts = new Text[plantGrid.GridSize.x, plantGrid.GridSize.y];
        leftDebugTexts = new Text[plantGrid.GridSize.x, plantGrid.GridSize.y];
        downDebugTexts = new Text[plantGrid.GridSize.x, plantGrid.GridSize.y];
        rightDebugTexts = new Text[plantGrid.GridSize.x, plantGrid.GridSize.y];
        for (int y = 0; y < plantGrid.GridSize.y; y++){
            for(int x = 0; x < plantGrid.GridSize.x; x++){
                GameObject gameObject = GameObject.Instantiate(plantGridDebugPrefab, new Vector3(x, -y, 0), Quaternion.identity);
                Text[] texts = gameObject.GetComponentsInChildren<Text>();
                mainDebugTexts[x, y] = texts[0];
                upDebugTexts[x, y] = texts[1];
                leftDebugTexts[x, y] = texts[2];
                downDebugTexts[x, y] = texts[3];
                rightDebugTexts[x, y] = texts[4];
            }
        }
    }

    public void OnPhysicsUpdate(){
        for (int y = 0; y < plantGrid.GridSize.y; y++){
            for(int x = 0; x < plantGrid.GridSize.x; x++){
                ref Plant plant = ref plantGrid.GetCell(new Vector2Int(x, y));
                if (plant == null) return;
                
                mainDebugTexts[x, y].text = plant.Gene.ThisGene.ToString();
                upDebugTexts[x, y].text = plant.Gene.GetChromosome(Vector2Int.up).ToString();
                leftDebugTexts[x, y].text = plant.Gene.GetChromosome(Vector2Int.left).ToString();
                downDebugTexts[x, y].text = plant.Gene.GetChromosome(Vector2Int.down).ToString();
                rightDebugTexts[x, y].text = plant.Gene.GetChromosome(Vector2Int.right).ToString();
            }
        }
    }
}
