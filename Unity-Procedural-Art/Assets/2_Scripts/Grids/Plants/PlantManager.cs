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
    private Dictionary<Vector2Short, PlantCell> grownPlants;
    private Dictionary<Vector2Short, PlantCell> livingPlants;
    private Dictionary<Vector2Short, PlantCell> deadPlants;

    private byte geneAmount;
    private Dictionary<Cell, byte> plantGrowEnergyRequirements = new Dictionary<Cell, byte>();

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
        grownPlants = new Dictionary<Vector2Short, PlantCell>();
        livingPlants = new Dictionary<Vector2Short, PlantCell>(GridSize.x * GridSize.y);
        deadPlants = new Dictionary<Vector2Short, PlantCell>(GridSize.x * GridSize.y);

        foreach (PlantGrowEnergyRequirement current in Settings.Instance.plantGrowEnergyRequirements){
            plantGrowEnergyRequirements.Add(current.cell, current.energyAmount);
        }

        EventManager.AddListener<Vector2Int>(Events.OnLeftMouseDown, (mousePos) => AddPlant(new Vector2Short(mousePos)));
    }

    public void OnPhysicsUpdate(){

        UpdateGrowingPlants();
        UpdateGrownPlants();
        UpdateLivingPlants();
        UpdateDeadPlants();
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

    private void UpdateGrowingPlants(){
        foreach (var current in growingPlants){

            if (current.Value.Energy >= plantGrowEnergyRequirements[cellGrid.GetCell(current.Key)]){ // If has enough energy to grow
                grownPlants.Add(current.Key, current.Value); 
            }

            UpdateEnergy(current);
        }
    }

    private void UpdateGrownPlants(){
        foreach (var current in grownPlants){
            ref DirectionChromosome directionChromosome = ref current.Value.Genome.GetDirectionChromosome(current.Value.ThisGene);
            TryGrowInDirection(ref directionChromosome, Vector2Short.Up);
            TryGrowInDirection(ref directionChromosome, Vector2Short.Left);
            TryGrowInDirection(ref directionChromosome, Vector2Short.Down);
            TryGrowInDirection(ref directionChromosome, Vector2Short.Right);
        }
        grownPlants.Clear();
    }

    private void UpdateLivingPlants(){
        foreach (var current in livingPlants){

            // Living time

            UpdateEnergy(current);
        }
    }

    private void UpdateDeadPlants(){
        foreach (var current in deadPlants){

            // Decay Time

        }
    }

    private void UpdateEnergy(KeyValuePair<Vector2Short, PlantCell> kvp){
        ref byte lightLevel = ref lightGrid.GetCell(kvp.Key);
        if (kvp.Value.Energy + lightLevel <= 255){
            kvp.Value.Energy += lightLevel;
        }
        else{
            kvp.Value.Energy = 255;
        }
    }

    private void TryGrowInDirection(ref DirectionChromosome directionChromosome, Vector2Short direction){
        
        directionChromosome.GetGene(direction);
        
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
    }
}