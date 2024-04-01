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
        Vector2Short gridsSize = Settings.Instance.GridSize;
        energyGridDebugger = new EnergyGridDebugger(gridsSize);
        lightGridDebugger = new LightGridDebugger(gridsSize);
        geneGridDebugger = new GeneGridDebugger(gridsSize);
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
    protected readonly Vector2Short size;

    //-------------------------------------

    public SingleValueGridDebugger(Vector2Short size){
        this.size = size;

        debugTexts = new Text[size.x, size.y];
        for (int y = 0; y < size.y; y++){
            for(int x = 0; x < size.x; x++){
                GameObject gameObject = GameObject.Instantiate(Settings.Instance.SingleValueGridPrefab, new Vector3(x, -y, 0), Quaternion.identity);
                debugTexts[x, y] = gameObject.GetComponentInChildren<Text>();
            }
        }
    }   

    public abstract void OnPhysicsUpdate();
}

public class EnergyGridDebugger : SingleValueGridDebugger
{
    // Dependencies
    private IGrid<GrowingPlant> growingPlantGrid;
    private IGrid<LivingPlant> livingPlantGrid;

    //--------------------------------------

    public EnergyGridDebugger(Vector2Short size) : base(size){
        growingPlantGrid = GameManager.GetService<GridManager>().GetGrid<GrowingPlant>();
        livingPlantGrid = GameManager.GetService<GridManager>().GetGrid<LivingPlant>();
    }
    public override void OnPhysicsUpdate(){
        for (int y = 0; y < size.y; y++){
            for(int x = 0; x < size.x; x++){
                if (growingPlantGrid.Get(new Vector2Short(x, y)).Equals(default(GrowingPlant))) { 
                    debugTexts[x, y].text = "";
                }
                else{
                    debugTexts[x, y].text = growingPlantGrid.Get(new Vector2Short(x, y)).Energy.ToString();
                }

                if (livingPlantGrid.Get(new Vector2Short(x, y)).Equals(default(LivingPlant))) {
                    debugTexts[x, y].text = "";
                }
                else{
                    debugTexts[x, y].text = livingPlantGrid.Get(new Vector2Short(x, y)).Energy.ToString();
                }
            }
        }
    }
}

public class LightGridDebugger : SingleValueGridDebugger
{
    private IGrid<Light> lightGrid;

    //--------------------------------------

    public LightGridDebugger(Vector2Short size) : base(size){
        lightGrid = GameManager.GetService<GridManager>().GetGrid<Light>();
    }

    public override void OnPhysicsUpdate(){
        for (int y = 0; y < size.y; y++){
            for(int x = 0; x < size.x; x++){
                debugTexts[x, y].text = lightGrid.Get(new Vector2Short(x, y)).LightLevel.ToString();
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

    private Vector2Short size;

    // Cache
    private GameObject geneGridDebugPrefab;

    // Dependencies
    private IGrid<GrowingPlant> growingPlantGrid;

    //-------------------------------------

    public GeneGridDebugger(Vector2Short size){
        growingPlantGrid = GameManager.GetService<GridManager>().GetGrid<GrowingPlant>();
        geneGridDebugPrefab = Settings.Instance.GeneGridDebugPrefab;

        mainDebugTexts = new Text[size.x, size.y];
        upDebugTexts = new Text[size.x, size.y];
        leftDebugTexts = new Text[size.x, size.y];
        downDebugTexts = new Text[size.x, size.y];
        rightDebugTexts = new Text[size.x, size.y];
        for (int y = 0; y < size.y; y++){
            for(int x = 0; x < size.x; x++){
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
        for (int y = 0; y < growingPlantGrid.GridSize.y; y++){
            for(int x = 0; x < growingPlantGrid.GridSize.x; x++){

                GrowingPlant growingPlant = growingPlantGrid.Get(new Vector2Short(x, y));
                if (growingPlant.Equals(default(GrowingPlant))) continue;
                
                mainDebugTexts[x, y].text = growingPlant.Gene.ToString();
                upDebugTexts[x, y].text = growingPlant.Genome.GetDirectionChromosome(growingPlant.Gene).GetGene(Vector2Short.Up).ToString();
                leftDebugTexts[x, y].text = growingPlant.Genome.GetDirectionChromosome(growingPlant.Gene).GetGene(Vector2Short.Left).ToString();;
                downDebugTexts[x, y].text = growingPlant.Genome.GetDirectionChromosome(growingPlant.Gene).GetGene(Vector2Short.Down).ToString();;
                rightDebugTexts[x, y].text = growingPlant.Genome.GetDirectionChromosome(growingPlant.Gene).GetGene(Vector2Short.Right).ToString();;
            }
        }
    }
}