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
    private List<Vector2Short> growingPlantsGC = new List<Vector2Short>();
    private List<Vector2Short> grownPlants = new List<Vector2Short>();
    private List<Vector2Short> livingPlants = new List<Vector2Short>();
    private List<Vector2Short> livingPlantsGC = new List<Vector2Short>();
    private List<Vector2Short> livingPlantsAC = new List<Vector2Short>();
    private List<Vector2Short> deadPlants = new List<Vector2Short>();
    private List<Vector2Short> deadPlantsGC = new List<Vector2Short>();

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

        UpdateSeedPlants();
        UpdateGrowingPlants();
        UpdateGrownPlants();
        UpdateLivingPlants();
        UpdateDeadPlants();
    }

    public void AddSeed(Vector2Short pos){

        if (!cellGrid.IsInBounds(pos)) return;
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
            if (seedPlant.LifeTime >= Settings.Instance.SeedGrowAge){
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
            growingPlant.Energy -= 1;

            if (growingPlant.Energy <= 0){
                growingPlantsGC.Add(currentPos);
                growingPlantGrid.Set(currentPos, null);
                cell.CellType = CellTypes.air;
                cellGrid.Set(currentPos, cell);
                gridRenderer.Update(currentPos);
            }
            else{
                growingPlantGrid.Set(currentPos, growingPlant);
            }
        }
        foreach (Vector2Short current in growingPlantsGC) growingPlants.Remove(current);
        growingPlantsGC.Clear();
    }

    private void UpdateGrownPlants(){
        foreach (Vector2Short currentPos in grownPlants){

            growingPlants.Remove(currentPos);

            TryGrowInDirection(currentPos, Vector2Short.Up);
            TryGrowInDirection(currentPos, Vector2Short.Left);
            TryGrowInDirection(currentPos, Vector2Short.Down);
            TryGrowInDirection(currentPos, Vector2Short.Right);

            livingPlants.Add(currentPos);
            livingPlantGrid.Set(currentPos, new LivingPlant(growingPlantGrid.Get(currentPos).Energy));
            growingPlantGrid.Set(currentPos, null);
        }
        grownPlants.Clear();
    }

    private void UpdateLivingPlants(){
        foreach (Vector2Short currentPos in livingPlants){

            LivingPlant livingPlant = livingPlantGrid.Get(currentPos);
            livingPlant.Energy = UpdateEnergy(livingPlant.Energy, lightGrid.Get(currentPos));
            livingPlant.LifeTime += 1;
            livingPlant.EatTime += 1;

            if (livingPlant.EatTime >= Settings.Instance.EatingDelay){
                livingPlant.EatTime = 0;
                livingPlant.Energy -= 1;
            }

            TryTransferEnergyInDirection(currentPos, Vector2Short.Down);
            TryTransferEnergyInDirection(currentPos, Vector2Short.Down);
            TryTransferEnergyInDirection(currentPos, Vector2Short.Up);
            TryTransferEnergyInDirection(currentPos, Vector2Short.Left);
            TryTransferEnergyInDirection(currentPos, Vector2Short.Right);

            // if (IsFloating(currentPos)){
            //     Vector2Short downPos = new Vector2Short(currentPos.x, currentPos.y + 1);
            //     livingPlantGrid.Set(downPos, livingPlant);
            //     livingPlantGrid.Set(currentPos, null);
            //     livingPlantsAC.Add(new Vector2Short(currentPos.x, currentPos.y + 1));
            //     livingPlantsGC.Add(currentPos);
            //     Cell downCell = cellGrid.Get(downPos);
            //     Cell currentCell = cellGrid.Get(currentPos);
            //     downCell.CellType = CellTypes.leave;
            //     currentCell.CellType = CellTypes.air;
            //     cellGrid.Set(downPos, downCell);
            //     cellGrid.Set(currentPos, currentCell);
            //     gridRenderer.Update(downPos);
            //     gridRenderer.Update(currentPos);
            // }

            if (livingPlant.LifeTime >= Settings.Instance.PlantDieAge || livingPlant.Energy <= 0){ // Die
                Cell cell = cellGrid.Get(currentPos);
                cell.CellType = CellTypes.dead;
                cellGrid.Set(currentPos, cell);
                livingPlantGrid.Set(currentPos, null);
                deadPlants.Add(currentPos);
                deadPlantGrid.Set(currentPos, new DeadPlant());
                livingPlantsGC.Add(currentPos);
                gridRenderer.Update(currentPos);
            }
            else{
                livingPlantGrid.Set(currentPos, livingPlant);
            }
        }
        foreach (Vector2Short current in livingPlantsGC) livingPlants.Remove(current);
        livingPlantsGC.Clear();
    }

    private void UpdateDeadPlants(){
        foreach (Vector2Short currentPos in deadPlants){
            
            DeadPlant deadPlant = deadPlantGrid.Get(currentPos);
            deadPlant.LifeTime += 1;
            if (deadPlant.LifeTime >= Settings.Instance.RotAge){
                Cell cell = cellGrid.Get(currentPos);
                cell.CellType = CellTypes.air;
                cellGrid.Set(currentPos, cell);
                deadPlantGrid.Set(currentPos, null);
                deadPlantsGC.Add(currentPos);
                gridRenderer.Update(currentPos);
            }
            else{
                deadPlantGrid.Set(currentPos, deadPlant);
            }
        }
        foreach (Vector2Short current in deadPlantsGC) deadPlants.Remove(current);
        deadPlantsGC.Clear();
    }

    private bool IsFloating(Vector2Short pos){
        Vector2Short downPos = new Vector2Short(pos.x, pos.y + 1);

        if (cellGrid.Get(downPos).CellType != CellTypes.air) return false;

        for (int x = pos.x - 1; x < pos.x + 1; x++){
            Vector2Short checkPos = new Vector2Short(x, pos.y + 1);
            if (!cellGrid.IsInBounds(checkPos)) continue;
            Cell cell = cellGrid.Get(checkPos);
            if (cell.CellType != CellTypes.air) return false;
        }

        return true;
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
        GrowingPlant targetGrowingPlant = new GrowingPlant(directionGene, parentGrowingPlant.Genome, 10);
        growingPlantGrid.Set(targetPos, targetGrowingPlant);
        growingPlants.Add(targetPos);
        gridRenderer.Update(targetPos);
    }

    private void TryTransferEnergyInDirection(Vector2Short parentPos, Vector2Short direction){

        Vector2Short targetPos = parentPos + new Vector2Short(direction.x, -direction.y);
        if (!cellGrid.IsInBounds(targetPos)) return;
        LivingPlant directionPlant = livingPlantGrid.Get(targetPos);
        if (directionPlant == null) return;
        Cell directionCell = cellGrid.Get(targetPos);
        LivingPlant parentPlant = livingPlantGrid.Get(parentPos);
        byte transferAmount;
        if (directionCell.CellType == CellTypes.wood){
            transferAmount = Settings.Instance.WoodEnergyTransferAmount;
        }
        else{
            transferAmount = Settings.Instance.LeavesEnergyTransferAmount;
        }
        if (directionCell.CellType == CellTypes.wood || directionCell.CellType == CellTypes.leave && parentPlant.Energy >= 10){
            int newEnergy = directionPlant.Energy + transferAmount;
            if (newEnergy >= 255) return;
            directionPlant.Energy += transferAmount;
            livingPlantGrid.Set(targetPos, directionPlant);
            if ((int)parentPlant.Energy - (int)transferAmount < 0){
                parentPlant.Energy = 0;
            }
            else{
                parentPlant.Energy -= transferAmount;
            }
        }
    }
}