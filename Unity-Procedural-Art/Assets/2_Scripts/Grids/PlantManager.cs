using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Watenk;

public class PlantManager : IPhysicsUpdateable
{
    // Settings: Atmosphere, Light, wind, radiation, Temperature
    public Vector2Short GridSize { get; private set; }

    private List<Vector2Short> seedPlants = new List<Vector2Short>();
    private List<Vector2Short> seedPlantsAC = new List<Vector2Short>();
    private List<Vector2Short> seedPlantsGC = new List<Vector2Short>();
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
    private IGridRenderer gridRenderer;

    //---------------------------------------------------

    public PlantManager(){
        GridSize = GameManager.GetService<GridManager>().GridSize;
        gridRenderer = GameManager.GetService<GridRenderer>();
        GridManager gridManager = GameManager.GetService<GridManager>();
        growingPlantGrid = gridManager.GetGrid<GrowingPlant>();
        livingPlantGrid = gridManager.GetGrid<LivingPlant>();
        deadPlantGrid = gridManager.GetGrid<DeadPlant>();
        seedPlantGrid = gridManager.GetGrid<SeedPlant>();
        lightGrid = gridManager.GetGrid<Light>();
        cellGrid = gridManager.GetGrid<Cell>();

        directionChromosomeAmount = Settings.Instance.DirectionChromosomeAmount;

        foreach (PlantGrowEnergyRequirement current in Settings.Instance.plantGrowEnergyRequirements){
            plantGrowEnergyRequirements.Add(current.cell, current.energyAmount);
        }

        EventManager.AddListener<Vector2Int>(Events.OnLeftMouseDown, (mousePos) => AddSeed(new Vector2Short(mousePos)));
    }

    public void OnPhysicsUpdate(){

        UpdateDeadPlants();
        UpdateLivingPlants();
        UpdateGrownPlants();
        UpdateGrowingPlants();
        UpdateSeedPlants();
    }

    public void AddSeed(Vector2Short pos){
        Cell cell = cellGrid.Get(pos);

        if (cell.CellType != CellTypes.air) return;

        seedPlantGrid.Set(pos, new SeedPlant(new Genome(directionChromosomeAmount), 255));
        seedPlantsAC.Add(pos);

        cell.CellType = CellTypes.seed;
        cellGrid.Set(pos, cell);
        gridRenderer.Update(pos);
    }

    //--------------------------------------------------

    private void UpdateSeedPlants(){
        seedPlants.AddRange(seedPlantsAC);
        seedPlantsAC.Clear();
        foreach (Vector2Short currentPos in seedPlants){
            Vector2Short downPos = new Vector2Short(currentPos.x, currentPos.y + 1);
            SeedPlant seedPlant = seedPlantGrid.Get(currentPos);
            Cell currentCell = cellGrid.Get(currentPos);
            Cell downCell = cellGrid.Get(downPos);

            if (seedPlant == null) Debug.Log("NEEEE");

            // Fall
            if (downCell.CellType == CellTypes.air){
                seedPlantGrid.Set(downPos, seedPlant);
                seedPlantGrid.Set(currentPos, null);
                seedPlantsAC.Add(new Vector2Short(currentPos.x, currentPos.y + 1));
                seedPlantsGC.Add(currentPos);
                downCell.CellType = CellTypes.seed;
                currentCell.CellType = CellTypes.air;
                cellGrid.Set(downPos, downCell);
                cellGrid.Set(currentPos, currentCell);
                gridRenderer.Update(downPos);
                gridRenderer.Update(currentPos);

                // Lifetime
                seedPlant = seedPlantGrid.Get(downPos);
                seedPlant.LifeTime += 1;
                seedPlantGrid.Set(downPos, seedPlant);
            }
            else{
                // Lifetime
                seedPlant.LifeTime += 1;
                seedPlantGrid.Set(currentPos, seedPlant);
            }

            // Germinate
            if (seedPlant.LifeTime >= 500){
                seedPlantGrid.Set(currentPos, null);
                seedPlantsGC.Add(currentPos);
                growingPlantGrid.Set(currentPos, new GrowingPlant(0, seedPlant.Genome, seedPlant.Energy));
                growingPlants.Add(currentPos);
                currentCell.CellType = CellTypes.leave;
                gridRenderer.Update(currentPos);
            }
        }
        foreach (Vector2Short current in seedPlantsGC) seedPlants.Remove(current);
        seedPlantsGC.Clear();
    }

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
        GrowingPlant targetGrowingPlant = new GrowingPlant(directionGene, parentGrowingPlant.Genome, 0);
        growingPlantGrid.Set(targetPos, targetGrowingPlant);
        growingPlants.Add(targetPos);
        gridRenderer.Update(targetPos);
    }
}