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

    private Dictionary<CellTypes, byte> plantGrowEnergyRequirements = new Dictionary<CellTypes, byte>();
    private byte directionChromosomeAmount;

    // Dependencies
    private IGrid grid;
    private IRenderableGrid renderableGrid;

    //---------------------------------------------------

    public PlantManager(){
        grid = GameManager.GetService<GridManager>();
        renderableGrid = GameManager.GetService<GridManager>();
        GridSize = grid.GridSize;
        directionChromosomeAmount = Settings.Instance.DirectionChromosomeAmount;

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
        Genome genome = new Genome(directionChromosomeAmount);
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
            ref Cell parentCell = ref grid.GetCell(currentPos);

            TryGrowInDirection(currentPos, ref parentCell, Vector2Short.Up);
            TryGrowInDirection(currentPos, ref parentCell, Vector2Short.Left);
            TryGrowInDirection(currentPos, ref parentCell, Vector2Short.Down);
            TryGrowInDirection(currentPos, ref parentCell, Vector2Short.Right);
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

    private void TryGrowInDirection(Vector2Short parentPos, ref Cell parentCell, Vector2Short direction){
        
        // Gene
        ref DirectionChromosome directionChromosome = ref parentCell.PlantCell.Genome.GetDirectionChromosome(parentCell.PlantCell.ThisGene);
        byte directionGene = directionChromosome.GetGene(direction);
        if (directionGene >= directionChromosomeAmount - parentCell.PlantCell.Genome.InactiveDirectionChromosomeAmount) return; // If is Inactive
        
        // target Pos
        Vector2Short targetPos = parentPos + new Vector2Short(direction.x, -direction.y);
        if (!grid.IsInBounds(targetPos)) return;
        ref Cell targetCell = ref grid.GetCell(targetPos);
        if (targetCell.CellType != CellTypes.air) return;

        targetCell.CellType = CellTypes.leave;
        targetCell.PlantCell = new PlantCell(directionGene, parentCell.PlantCell.Genome, 0);
        growingPlants.Add(targetPos);
        renderableGrid.AddChangedCell(targetPos);
    }
}