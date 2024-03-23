using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class PlantManager : IPhysicsUpdateable
{
    // Settings: Gravity, Atmosphere, Light, wind, radiation, Temperature

    // Cache
    private int geneAmount;

    // Dependencies
    private IGrid grid;

    //---------------------------------------------------

    public PlantManager(){
        grid = GameManager.GetService<GridManager>();
        geneAmount = Settings.Instance.GeneAmount;
        EventManager.AddListener<Vector2Int>(Events.OnLeftMouseDown, OnLeftMouseDown);
    }

    public void OnPhysicsUpdate(){
        for (int y = 0; y < grid.GridSize.y; y++){
            for(int x = 0; x < grid.GridSize.x; x++){
                Cell parent = grid.GetCell(new Vector2Int(x, y));
                if (parent.Plant != null) {
                    TryGrowInDirection(parent, Vector2Int.up);
                    TryGrowInDirection(parent, Vector2Int.left);
                    TryGrowInDirection(parent, Vector2Int.down);
                    TryGrowInDirection(parent, Vector2Int.right);
                }
            }
        }
    }

    public void AddPlant(Vector2Int pos){
        Genome genome = new Genome(geneAmount);
        Cell currentCell = grid.GetCell(pos);
        currentCell.Plant = new Plant(genome.GetGene(0), genome);
        currentCell.Cells = Cells.leave;
    }

    //--------------------------------------------------

    private void OnLeftMouseDown(Vector2Int mousePos){
        AddPlant(mousePos);
    }

    private void TryGrowInDirection(Cell parent, Vector2Int direction){
        Cell targetCell = grid.GetCell(parent.Pos + new Vector2Int(direction.x, -direction.y));
        if (targetCell == null) return;
        Debug.Log("parent: " + parent + ", direction: " + direction + ", Plant: " + parent.Plant.ToString());
        int chromosome = parent.Plant.Gene.GetChromosome(direction);
        if (chromosome == -1) return; // Is Inactive
        if (targetCell.Cells != Cells.air) return;
        if (targetCell.Lightlevel == 0) return;

        targetCell.Cells = Cells.leave;
        targetCell.Plant = new Plant(parent.Plant.Genome.GetGene(chromosome), parent.Plant.Genome);
    }
}