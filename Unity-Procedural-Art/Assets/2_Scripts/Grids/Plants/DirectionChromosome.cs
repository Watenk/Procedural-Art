using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionChromosome
{
    private Dictionary<Vector2Short, byte> genes = new Dictionary<Vector2Short, byte>();

    //---------------------------

    public DirectionChromosome(int chromosomeAmount){
        genes.Add(Vector2Short.Up, (byte)Random.Range(0, chromosomeAmount));
        genes.Add(Vector2Short.Left, (byte)Random.Range(0, chromosomeAmount));
        genes.Add(Vector2Short.Down, (byte)Random.Range(0, chromosomeAmount));
        genes.Add(Vector2Short.Right, (byte)Random.Range(0, chromosomeAmount));
    }

    public byte GetGene(Vector2Short direction){
        if (!genes.TryGetValue(direction, out byte gene)) Debug.LogError("Couldn't get gene");
        return gene;
    }

    public void MutateGenes(float mutationChance, Genome genome){
        foreach (var current in genes){
            if (Random.Range(0.0f, 100.0f) <= mutationChance){
                genes[current.Key] = (byte)Random.Range(0, genome.ChromosomeAmount);
            }
        }
    }
}