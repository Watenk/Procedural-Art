using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Watenk;

public class PlantManager : IPhysicsUpdateable
{
    // Settings: Gravity, Atmosphere, Light, wind, radiation, Temperature
    public Vector2Short GridSize { get; private set; }

    private List<Vector2Short> growingPlants = new List<Vector2Short>();
    private List<Vector2Short> grownPlants = new List<Vector2Short>();
    private List<Vector2Short> livingPlants = new List<Vector2Short>();
    private List<Vector2Short> deadPlants = new List<Vector2Short>();

    private byte geneAmount;
    private Dictionary<CellTypes, byte> plantGrowEnergyRequirements = new Dictionary<CellTypes, byte>();

    // Dependencies
    private IGrid grid;
    private IRenderableGrid renderableGrid;

    //---------------------------------------------------

    public PlantManager(){
        grid = GameManager.GetService<GridManager>();
        renderableGrid = GameManager.GetService<GridManager>();
        GridSize = grid.GridSize;
        geneAmount = Settings.Instance.GeneAmount;

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
        Cell cell = grid.GetCell(pos);
        
        cell.CellType = CellTypes.leave;
        cell.PlantCell = new PlantCell(0, genome, 0);
        growingPlants.Add(pos);
        renderableGrid.AddChangedCell(pos);
    }

    //--------------------------------------------------

    private void UpdateGrowingPlants(){
        foreach (Vector2Short currentPos in growingPlants){

            ref Cell cell = ref grid.GetCell(currentPos);
            if (cell.PlantCell.Energy >= plantGrowEnergyRequirements[cell.CellType]){ // If has enough energy to grow
                grownPlants.Add(currentPos); 
            }

            UpdateEnergy(ref cell);
        }
    }

    private void UpdateGrownPlants(){
        foreach (Vector2Short currentPos in grownPlants){
            ref Cell cell = ref grid.GetCell(currentPos);
            ref DirectionChromosome directionChromosome = ref cell.PlantCell.Genome.GetDirectionChromosome(cell.PlantCell.ThisGene);

            TryGrowInDirection(ref directionChromosome, Vector2Short.Up);
            TryGrowInDirection(ref directionChromosome, Vector2Short.Left);
            TryGrowInDirection(ref directionChromosome, Vector2Short.Down);
            TryGrowInDirection(ref directionChromosome, Vector2Short.Right);
        }
        grownPlants.Clear();
    }

    private void UpdateLivingPlants(){
        foreach (Vector2Short currentpos in livingPlants){

            ref Cell cell = ref grid.GetCell(currentpos);

            // Living time

            UpdateEnergy(ref cell);
        }
    }

    private void UpdateDeadPlants(){
        foreach (Vector2Short currentPos in deadPlants){

            // Decay Time

        }
    }

    private void UpdateEnergy(ref Cell cell){
        if (cell.PlantCell.Energy + cell.LightLevel <= 255){
            cell.PlantCell.Energy += cell.LightLevel;
        }
        else{
            cell.PlantCell.Energy = 255;
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