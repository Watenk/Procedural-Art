using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : IPhysicsUpdateable
{
    private Genome genome;
    private List<PlantChunk> activeChunks = new List<PlantChunk>();
    private List<PlantChunk> addedActiveChunks = new List<PlantChunk>();

    // Dependencies
    private GridManager gridManager;
    private LightManager lightManager;

    //--------------------------------

    public Plant(Vector2Int pos, int geneAmount){
        gridManager = GameManager.GetService<GridManager>();
        lightManager = GameManager.GetService<LightManager>();
        genome = new Genome(geneAmount);
        activeChunks.Add(new PlantChunk(pos, genome.GetGene(0)));
        gridManager.SetCell(pos, Cells.leave);
    }

    public void OnPhysicsUpdate(){
        foreach (PlantChunk current in activeChunks){
            TryGrowInDirection(current, Vector2Int.up);
            TryGrowInDirection(current, Vector2Int.left);
            TryGrowInDirection(current, Vector2Int.down);
            TryGrowInDirection(current, Vector2Int.right);
        }
        activeChunks.AddRange(addedActiveChunks);
        addedActiveChunks.Clear();
    }

    //-------------------------------------

    private void TryGrowInDirection(PlantChunk parent, Vector2Int direction){
        Vector2Int newPos = parent.Pos += new Vector2Int(direction.x, -direction.y);
        int chromosome = parent.Gene.GetChromosome(direction);
        
        if (!gridManager.isInBounds(newPos)) return;
        if (chromosome >= genome.InactiveGenesAmount) return;
        if (gridManager.GetCell(newPos).Cells != Cells.air) return;
        if (lightManager.GetLightLevel(newPos) == 0) return;
            
        gridManager.SetCell(newPos, Cells.leave);
        addedActiveChunks.Add(new PlantChunk(newPos, genome.GetGene(chromosome)));
    }
}

public class PlantChunk{
    public Vector2Int Pos;
    public Gene Gene;

    //-----------------------------

    public PlantChunk(Vector2Int pos, Gene gene){
        Pos = pos;
        Gene = gene;
    }
}
