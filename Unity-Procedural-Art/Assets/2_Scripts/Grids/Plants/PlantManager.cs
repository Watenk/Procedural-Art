using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Watenk;

public class PlantManager : IPhysicsUpdateable, IPlantGrid
{
    // Settings: Gravity, Atmosphere, Light, wind, radiation, Temperature
    public Vector2Int GridSize { get; private set; }

    private Plant[,] plants;
    private List<Vector2Int> updatedPlants = new List<Vector2Int>();

    // Cache
    private int geneAmount;

    // Dependencies
    private ICellGrid cellGrid;
    private ILightGrid lightGrid;
    private ICellGridDrawable cellGridDrawable;

    //---------------------------------------------------

    public PlantManager(){
        cellGrid = GameManager.GetService<CellGridManager>();
        lightGrid = GameManager.GetService<LightManager>();
        cellGridDrawable = GameManager.GetService<CellGridManager>();
        GridSize = cellGrid.GridSize;
        geneAmount = Settings.Instance.GeneAmount;

        plants = new Plant[GridSize.x, GridSize.y];

        EventManager.AddListener<Vector2Int>(Events.OnLeftMouseDown, OnLeftMouseDown);
    }

    public void OnPhysicsUpdate(){
        for (int y = 0; y < cellGrid.GridSize.y; y++){
            for(int x = 0; x < cellGrid.GridSize.x; x++){
                Vector2Int currentPos = new Vector2Int(x, y);
                ref Plant plant = ref plants[x, y];
                if (plant != null){
                    TryGrowInDirection(currentPos, Vector2Int.up);
                    TryGrowInDirection(currentPos, Vector2Int.left);
                    TryGrowInDirection(currentPos, Vector2Int.down);
                    TryGrowInDirection(currentPos, Vector2Int.right);
                }
            }
        }
        updatedPlants.Clear();
    }

    public void AddPlant(Vector2Int pos){
        Genome genome = new Genome(geneAmount);
        ref Cell currentCell = ref cellGrid.GetCell(pos);
        
        currentCell = Cell.leave;
        plants[pos.x, pos.y] = new Plant(genome.GetGene(0), genome);
        cellGridDrawable.AddChangedCell(pos);
    }

    public ref Plant GetCell(Vector2Int pos){
        return ref plants[pos.x, pos.y];
    }

    public bool IsInBounds(Vector2Int pos){
        if (GridUtility.IsInBounds(pos, Vector2Int.zero, GridSize)) return true;
        else return false;
    }

    //--------------------------------------------------

    private void OnLeftMouseDown(Vector2Int mousePos){
        AddPlant(mousePos);
    }

    private void TryGrowInDirection(Vector2Int parentPos, Vector2Int direction){
        
        if (updatedPlants.Contains(parentPos)) return;

        // Chromosome Parent
        ref Plant parentPlant = ref plants[parentPos.x, parentPos.y];
        int chromosome = parentPlant.Gene.GetChromosome(direction);
        if (chromosome == -1) return; // If is Inactive

        // Target Plant
        Vector2Int targetPos = parentPos + new Vector2Int(direction.x, -direction.y);
        if (!IsInBounds(targetPos)) return;
        ref Cell targetCell = ref cellGrid.GetCell(targetPos);
        if (targetCell != Cell.air) return;

        // Light
        ref byte lightLevel = ref lightGrid.GetCell(targetPos);
        if (lightLevel == 0) return;

        targetCell = Cell.leave;
        plants[targetPos.x, targetPos.y] = new Plant(parentPlant.Genome.GetGene(chromosome), parentPlant.Genome);
        cellGridDrawable.AddChangedCell(targetPos);
        updatedPlants.Add(targetPos);
    }
}