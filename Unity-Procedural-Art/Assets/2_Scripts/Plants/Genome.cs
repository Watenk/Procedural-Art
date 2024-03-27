using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watenk;

public class Genome
{
    public byte DirectionChromosomeAmount { get; private set; }
    public byte InactiveDirectionChromosomeAmount { get; private set; }

    private DirectionChromosome[] directionChromosomes;

    // Cache
    private float chromosomeMutationChance;

    //--------------------------------------

    public Genome(byte directionChromosomeAmount){
        DirectionChromosomeAmount = directionChromosomeAmount;
        chromosomeMutationChance = Settings.Instance.ChromosomeMutationChance;
        InactiveDirectionChromosomeAmount = Settings.Instance.InactiveDirectionChromosomeAmount;

        directionChromosomes = new DirectionChromosome[DirectionChromosomeAmount];
        for (byte i = 0; i < DirectionChromosomeAmount; i++){
            directionChromosomes[i] = new DirectionChromosome(DirectionChromosomeAmount);
        }
    }

    public ref DirectionChromosome GetDirectionChromosome(byte index){
        return ref directionChromosomes[index];
    }

    public void Mutate(){
        for (byte i = 0; i < DirectionChromosomeAmount; i++){
            if (Random.Range(0.0f, 100.0f) <= chromosomeMutationChance){
                DirectionChromosome currentChromosome = directionChromosomes[i];
                currentChromosome.MutateGenes(chromosomeMutationChance, this);
            } 
        }
    }
}
