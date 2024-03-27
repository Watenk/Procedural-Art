using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridDebugger : IPhysicsUpdateable
{
    private EnergyGridDebugger energyGridDebugger;
    private LightGridDebugger lightGridDebugger;
    private GeneGridDebugger geneGridDebugger;

    //----------------------------------------------

    public GridDebugger(){
        energyGridDebugger = new EnergyGridDebugger();
        lightGridDebugger = new LightGridDebugger();
        geneGridDebugger = new GeneGridDebugger();
    }

    public void OnPhysicsUpdate(){
        energyGridDebugger.OnPhysicsUpdate();
        lightGridDebugger.OnPhysicsUpdate();
        geneGridDebugger.OnPhysicsUpdate();
    }
}

public abstract class SingleValueGridDebugger : IPhysicsUpdateable
{
    protected Text[,] debugTexts;

    // Cache
    private GameObject singleValueGridPrefab;

    // Dependencies
    protected IGrid grid;

    //-------------------------------------

    public SingleValueGridDebugger(){
        grid = GameManager.GetService<GridManager>();
        singleValueGridPrefab = Settings.Instance.SingleValueGridPrefab;

        debugTexts = new Text[grid.GridSize.x, grid.GridSize.y];
        for (int y = 0; y < grid.GridSize.y; y++){
            for(int x = 0; x < grid.GridSize.x; x++){
                GameObject gameObject = GameObject.Instantiate(singleValueGridPrefab, new Vector3(x, -y, 0), Quaternion.identity);
                debugTexts[x, y] = gameObject.GetComponentInChildren<Text>();
            }
        }
    }   

    public abstract void OnPhysicsUpdate();
}

public class EnergyGridDebugger : SingleValueGridDebugger
{
    public override void OnPhysicsUpdate(){
        for (int y = 0; y < grid.GridSize.y; y++){
            for(int x = 0; x < grid.GridSize.x; x++){
                if (grid.GetCell(new Vector2Short(x, y)).PlantCell == null) {
                    debugTexts[x, y].text = "";
                }
                else{
                    debugTexts[x, y].text = grid.GetCell(new Vector2Short(x, y)).PlantCell.Energy.ToString();
                }
            }
        }
    }
}

public class LightGridDebugger : SingleValueGridDebugger
{
    public override void OnPhysicsUpdate(){
        for (int y = 0; y < grid.GridSize.y; y++){
            for(int x = 0; x < grid.GridSize.x; x++){
                debugTexts[x, y].text = grid.GetCell(new Vector2Short(x, y)).LightLevel.ToString();
            }
        }
    }
}

public class GeneGridDebugger : IPhysicsUpdateable
{
    private Text[,] mainDebugTexts;
    private Text[,] upDebugTexts;
    private Text[,] leftDebugTexts;
    private Text[,] downDebugTexts;
    private Text[,] rightDebugTexts;

    // Cache
    private GameObject geneGridDebugPrefab;

    // Dependencies
    private IGrid grid;

    //-------------------------------------

    public GeneGridDebugger(){
        grid = GameManager.GetService<GridManager>();
        geneGridDebugPrefab = Settings.Instance.GeneGridDebugPrefab;

        mainDebugTexts = new Text[grid.GridSize.x, grid.GridSize.y];
        upDebugTexts = new Text[grid.GridSize.x, grid.GridSize.y];
        leftDebugTexts = new Text[grid.GridSize.x, grid.GridSize.y];
        downDebugTexts = new Text[grid.GridSize.x, grid.GridSize.y];
        rightDebugTexts = new Text[grid.GridSize.x, grid.GridSize.y];
        for (int y = 0; y < grid.GridSize.y; y++){
            for(int x = 0; x < grid.GridSize.x; x++){
                GameObject gameObject = GameObject.Instantiate(geneGridDebugPrefab, new Vector3(x, -y, 0), Quaternion.identity);
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
        for (int y = 0; y < grid.GridSize.y; y++){
            for(int x = 0; x < grid.GridSize.x; x++){

                ref Cell cell = ref grid.GetCell(new Vector2Short(x, y));
                if (cell.PlantCell == null) continue;
                
                mainDebugTexts[x, y].text = cell.PlantCell.ThisGene.ToString();
                upDebugTexts[x, y].text = cell.PlantCell.Genome.GetDirectionChromosome(cell.PlantCell.ThisGene).GetGene(Vector2Short.Up).ToString();
                leftDebugTexts[x, y].text = cell.PlantCell.Genome.GetDirectionChromosome(cell.PlantCell.ThisGene).GetGene(Vector2Short.Left).ToString();;
                downDebugTexts[x, y].text = cell.PlantCell.Genome.GetDirectionChromosome(cell.PlantCell.ThisGene).GetGene(Vector2Short.Down).ToString();;
                rightDebugTexts[x, y].text = cell.PlantCell.Genome.GetDirectionChromosome(cell.PlantCell.ThisGene).GetGene(Vector2Short.Right).ToString();;
            }
        }
    }
}