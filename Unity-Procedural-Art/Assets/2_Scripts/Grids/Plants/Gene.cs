using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gene
{
    public int GeneIndex { get; private set; }
    private Dictionary<Vector2Int, int> chromosomes = new Dictionary<Vector2Int, int>();

    //---------------------------

    public Gene(int geneIndex){
        GeneIndex = geneIndex;
    }

    public void AddChromosome(Vector2Int direction, int gene){
        chromosomes.Add(direction, gene);
    }

    public int GetChromosome(Vector2Int direction){
        if (!chromosomes.TryGetValue(direction, out int chromosome)) Debug.LogError("Couldn't get chromosome");
        return chromosome;
    }

    public void MutateChromosomes(float mutationChance, int geneAmount){
        foreach (var current in chromosomes){
            if (Random.Range(0.0f, 100.0f) <= mutationChance){
                chromosomes[current.Key] = Random.Range(0, geneAmount);
            }
        }
    }
}