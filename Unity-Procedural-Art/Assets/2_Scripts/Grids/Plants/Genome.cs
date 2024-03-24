using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watenk;

public class Genome
{
    public byte ChromosomeAmount { get; private set; }
    public byte InactiveChromosomeAmount { get; private set; }

    private Chromosome[] chromosomes;

    // Cache
    private float chromosomeMutationChance;

    //--------------------------------------

    public Genome(byte chromosomeAmount){
        ChromosomeAmount = chromosomeAmount;
        chromosomeMutationChance = Settings.Instance.ChromosomeMutationChance;
        InactiveChromosomeAmount = Settings.Instance.InactiveChromosomeAmount;

        chromosomes = new Chromosome[ChromosomeAmount];
        for (byte i = 0; i < ChromosomeAmount; i++){
            chromosomes[i] = new Chromosome(ChromosomeAmount);
        }
    }

    public Chromosome GetChromosome(byte index){
        return chromosomes[index];
    }

    public void Mutate(){
        for (byte i = 0; i < ChromosomeAmount; i++){
            if (Random.Range(0.0f, 100.0f) <= chromosomeMutationChance){
                Chromosome currentChromosome = chromosomes[i];
                currentChromosome.MutateGenes(chromosomeMutationChance, this);
            } 
        }
    }
}
