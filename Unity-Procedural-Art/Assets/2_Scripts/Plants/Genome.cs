using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watenk;

public class Genome
{
    public byte ChromosomeAmount { get; private set; }
    public byte InactiveChromosomeAmount { get; private set; }

    private DirectionChromosome[] chromosomes;

    // Cache
    private float chromosomeMutationChance;

    //--------------------------------------

    public Genome(byte chromosomeAmount){
        ChromosomeAmount = chromosomeAmount;
        chromosomeMutationChance = Settings.Instance.ChromosomeMutationChance;
        InactiveChromosomeAmount = Settings.Instance.InactiveChromosomeAmount;

        chromosomes = new DirectionChromosome[ChromosomeAmount];
        for (byte i = 0; i < ChromosomeAmount; i++){
            chromosomes[i] = new DirectionChromosome(ChromosomeAmount);
        }
    }

    public ref DirectionChromosome GetDirectionChromosome(byte index){
        return ref chromosomes[index];
    }

    public void Mutate(){
        for (byte i = 0; i < ChromosomeAmount; i++){
            if (Random.Range(0.0f, 100.0f) <= chromosomeMutationChance){
                DirectionChromosome currentChromosome = chromosomes[i];
                currentChromosome.MutateGenes(chromosomeMutationChance, this);
            } 
        }
    }
}
