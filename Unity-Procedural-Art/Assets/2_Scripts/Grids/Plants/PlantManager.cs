using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Watenk;

public class PlantManager : IPhysicsUpdateable, IPlantGrid
{
    // Settings: Gravity, Atmosphere, Light, wind, radiation, Temperature
    public Vector2Short GridSize { get; private set; }

    private Dictionary<Vector2Short, PlantCell> growingPlants;
    private Dictionary<Vector2Short, PlantCell> livingPlants;
    private Dictionary<Vector2Short, PlantCell> deadPlants;

    private byte geneAmount;

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

        growingPlants = new Dictionary<Vector2Short, PlantCell>(GridSize.x * GridSize.y);
        livingPlants = new Dictionary<Vector2Short, PlantCell>(GridSize.x * GridSize.y);
        deadPlants = new Dictionary<Vector2Short, PlantCell>(GridSize.x * GridSize.y);

        EventManager.AddListener<Vector2Int>(Events.OnLeftMouseDown, OnLeftMouseDown);
    }

    public void OnPhysicsUpdate(){

        foreach (var current in growingPlants){
            // if (TryGrow(current.Value)){
            //     //livingPlants.Add(current.Value);
            // }
        }

        // // Garbage Collection
        // foreach (var current in GCGrowingPlants) growingPlants.Remove(current);
        // GCGrowingPlants.Clear();

        // for (int y = 0; y < cellGrid.GridSize.y; y++){
        //     for(int x = 0; x < cellGrid.GridSize.x; x++){
        //         Vector2Int currentPos = new Vector2Int(x, y);
        //         ref Plant currentGrowingPlant = ref growingPlants[x, y];
        //         if (currentGrowingPlant == null) continue;

        //         TryGrow(currentGrowingPlant, currentPos);
        //     }
        // }
    }

    public void AddPlant(Vector2Short pos){
        Genome genome = new Genome(geneAmount);
        ref Cell currentCell = ref cellGrid.GetCell(pos);
        
        currentCell = Cell.leave;
        growingPlants.Add(pos, new PlantCell(0, genome));
        cellGridDrawable.AddChangedCell(pos);
    }

    public PlantCell GetCell(Vector2Short pos){
        if (!growingPlants.ContainsKey(pos)) return null;

        return growingPlants[pos];
    }

    public bool IsInBounds(Vector2Short pos){
        if (GridUtility.IsInBounds(pos, Vector2Short.Zero, GridSize)) return true;
        else return false;
    }

    //--------------------------------------------------

    private void OnLeftMouseDown(Vector2Int mousePos){
        AddPlant(new Vector2Short(mousePos));
    }

    //private bool TryGrow(Plant plant){

    //}

    // private void TryGrowInDirection(Vector2Int parentPos, Vector2Int direction){
        
    //     if (updatedPlants.Contains(parentPos)) return;

    //     // Chromosome Parent
    //     ref Plant parentPlant = ref plants[parentPos.x, parentPos.y];
    //     byte chromosome = parentPlant.Gene.GetChromosome(direction);
    //     //if (chromosome == -1) return; // If is Inactive

    //     // Target Plant
    //     Vector2Int targetPos = parentPos + new Vector2Int(direction.x, -direction.y);
    //     if (!IsInBounds(targetPos)) return;
    //     ref Cell targetCell = ref cellGrid.GetCell(targetPos);
    //     if (targetCell != Cell.air) return;

    //     // Light
    //     ref byte lightLevel = ref lightGrid.GetCell(targetPos);
    //     if (lightLevel == 0) return;

    //     targetCell = Cell.leave;
    //     plants[targetPos.x, targetPos.y] = new Plant(parentPlant.Genome.GetChromosome(chromosome), parentPlant.Genome);
    //     cellGridDrawable.AddChangedCell(targetPos);
    //     updatedPlants.Add(targetPos);
    // }
}