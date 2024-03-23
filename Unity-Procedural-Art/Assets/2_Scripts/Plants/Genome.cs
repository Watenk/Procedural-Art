using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watenk;

public class Genome
{
    public int GeneAmount { get; private set; }
    public int InactiveGenesAmount { get; private set; }

    private Gene[] genes;
    private float geneMutationChance;
    private float chromosomeMutationChance;

    //--------------------------------------

    public Genome(int geneAmount){
        GeneAmount = geneAmount;
        geneMutationChance = Settings.Instance.GeneMutationChance;
        chromosomeMutationChance = Settings.Instance.ChromosomeMutationChance;
        InactiveGenesAmount = Settings.Instance.InactiveGeneAmount;
        genes = new Gene[geneAmount];

        for (int i = 0; i < geneAmount; i++){
            Gene newGene = new Gene();
            newGene.AddChromosome(Vector2Int.up, Random.Range(0, geneAmount));
            newGene.AddChromosome(Vector2Int.left, Random.Range(0, geneAmount));
            newGene.AddChromosome(Vector2Int.down, Random.Range(0, geneAmount));
            newGene.AddChromosome(Vector2Int.right, Random.Range(0, geneAmount));
            genes[i] = newGene;
        }
    }

    public Gene GetGene(int geneIndex){
        if (!MathUtility.IsInBounds(geneIndex, 0, GeneAmount)) return null;

        return genes[geneIndex];
    }

    public void Mutate(){
        for (int i = 0; i < GeneAmount; i++){
            if (Random.Range(0.0f, 100.0f) <= geneMutationChance){
                Gene currentGene = genes[i];
                currentGene.MutateChromosomes(chromosomeMutationChance, GeneAmount);
            } 
        }
    }
}

public class Gene
{
    private Dictionary<Vector2Int, int> chromosomes = new Dictionary<Vector2Int, int>();

    //---------------------------

    public void AddChromosome(Vector2Int direction, int geneIndex){
        chromosomes.Add(direction, geneIndex);
    }

    public int GetChromosome(Vector2Int direction){
        chromosomes.TryGetValue(direction, out int geneIndex);
        return geneIndex;
    }

    public void MutateChromosomes(float mutationChance, int geneAmount){
        foreach (var current in chromosomes){
            if (Random.Range(0.0f, 100.0f) <= mutationChance){
                chromosomes[current.Key] = Random.Range(0, geneAmount);
            }
        }
    }
}
