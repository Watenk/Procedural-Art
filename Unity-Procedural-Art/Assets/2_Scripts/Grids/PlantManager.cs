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
    private IGrid<GrowingPlant> growingPlantGrid;
    private IGrid<LivingPlant> livingPlantGrid;
    private IGrid<DeadPlant> deadPlantGrid;
    private IGrid<SeedPlant> seedPlantGrid;
    private IGrid<Light> lightGrid;
    private IGrid<Cell> cellGrid;

    //---------------------------------------------------

    public PlantManager(){
        GridSize = GameManager.GetService<GridManager>().GridSize;
        directionChromosomeAmount = Settings.Instance.DirectionChromosomeAmount;

        GridManager gridManager = GameManager.GetService<GridManager>();
        growingPlantGrid = gridManager.GetGrid<GrowingPlant>();
        livingPlantGrid = gridManager.GetGrid<LivingPlant>();
        deadPlantGrid = gridManager.GetGrid<DeadPlant>();
        seedPlantGrid = gridManager.GetGrid<SeedPlant>();
        lightGrid = gridManager.GetGrid<Light>();
        cellGrid = gridManager.GetGrid<Cell>();

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
        SeedPlant seedPlant = new SeedPlant{
            Genome = new Genome(directionChromosomeAmount),
            Energy = 255,
        };
        seedPlantGrid.Set(pos, seedPlant);

        Cell cell = cellGrid.Get(pos);
        cell.CellType = CellTypes.seed;
        cellGrid.Set(pos, cell);
    }

    //--------------------------------------------------

    private void UpdateGrowingPlants(){
        foreach (Vector2Short currentPos in growingPlants){

            GrowingPlant growingPlant = growingPlantGrid.Get(currentPos);
            Cell cell = cellGrid.Get(currentPos);

            if (growingPlant.Energy >= plantGrowEnergyRequirements[cell.CellType]){ // If has enough energy to grow
                grownPlants.Add(currentPos); 
            }

            growingPlant.Energy = UpdateEnergy(growingPlant.Energy, lightGrid.Get(currentPos));
            growingPlantGrid.Set(currentPos, growingPlant);
        }
    }

    private void UpdateGrownPlants(){
        foreach (Vector2Short currentPos in grownPlants){

            TryGrowInDirection(currentPos, Vector2Short.Up);
            TryGrowInDirection(currentPos, Vector2Short.Left);
            TryGrowInDirection(currentPos, Vector2Short.Down);
            TryGrowInDirection(currentPos, Vector2Short.Right);
        }
        grownPlants.Clear();
    }

    private void UpdateLivingPlants(){
        foreach (Vector2Short currentPos in livingPlants){

            LivingPlant livingPlant = livingPlantGrid.Get(currentPos);
            livingPlant.Energy = UpdateEnergy(livingPlant.Energy, lightGrid.Get(currentPos));
            livingPlantGrid.Set(currentPos, livingPlant);
        }
    }

    private void UpdateDeadPlants(){
        foreach (Vector2Short currentPos in deadPlants){

            // Decay Time

        }
    }

    private byte UpdateEnergy(byte currentEnergy, Light light){
        if (currentEnergy + light.LightLevel <= 255){
            return currentEnergy += light.LightLevel;
        }
        else{
            return 255;
        }
    }

    private void TryGrowInDirection(Vector2Short parentPos, Vector2Short direction){
        
        // check target Pos
        Vector2Short targetPos = parentPos + new Vector2Short(direction.x, -direction.y);
        if (!cellGrid.IsInBounds(targetPos)) return;
        Cell targetCell = cellGrid.Get(targetPos);
        if (targetCell.CellType != CellTypes.air) return;

        // Gene
        GrowingPlant parentGrowingPlant = growingPlantGrid.Get(parentPos);
        ref DirectionChromosome directionChromosome = ref parentGrowingPlant.Genome.GetDirectionChromosome(parentGrowingPlant.Gene);
        byte directionGene = directionChromosome.GetGene(direction);
        if (directionGene >= directionChromosomeAmount - parentGrowingPlant.Genome.InactiveDirectionChromosomeAmount) return; // If is Inactive

        targetCell.CellType = CellTypes.leave;
        GrowingPlant targetGrowingPlant = new GrowingPlant{
            Gene = directionGene,
            Genome = parentGrowingPlant.Genome,
            Energy = 0
        };
        growingPlantGrid.Set(targetPos, targetGrowingPlant);
        growingPlants.Add(targetPos);
    }
}