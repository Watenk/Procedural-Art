using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watenk;

public class Genome
{
    public int GeneAmount { get; private set; }

    private Gene[] genes;

    // Cache
    private float geneMutationChance;
    private float chromosomeMutationChance;
    private int inactiveGenesAmount;

    //--------------------------------------

    public Genome(int geneAmount){
        GeneAmount = geneAmount;
        geneMutationChance = Settings.Instance.GeneMutationChance;
        chromosomeMutationChance = Settings.Instance.ChromosomeMutationChance;
        inactiveGenesAmount = Settings.Instance.InactiveGeneAmount;
        genes = new Gene[geneAmount];

        for (int i = 0; i < geneAmount; i++){
            Gene newGene = new Gene(i);

            if (i >= inactiveGenesAmount){
                newGene.AddChromosome(Vector2Int.up, -1);
                newGene.AddChromosome(Vector2Int.left, -1);
                newGene.AddChromosome(Vector2Int.down, -1);
                newGene.AddChromosome(Vector2Int.right, -1);
                genes[i] = newGene;
            }
            else{
                newGene.AddChromosome(Vector2Int.up, Random.Range(0, geneAmount));
                newGene.AddChromosome(Vector2Int.left, Random.Range(0, geneAmount));
                newGene.AddChromosome(Vector2Int.down, Random.Range(0, geneAmount));
                newGene.AddChromosome(Vector2Int.right, Random.Range(0, geneAmount));
                genes[i] = newGene;
            }
        }
    }

    public Gene GetGene(int gene){
        if (!MathUtility.IsInBounds(gene, 0, GeneAmount)) return null;

        return genes[gene];
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
