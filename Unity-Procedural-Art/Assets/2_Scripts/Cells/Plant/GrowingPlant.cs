using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GrowingPlant
{
    public byte Gene;
    public Genome Genome;
    public byte Energy;

    //---------------------------------------------

    public GrowingPlant(byte gene, Genome genome, byte energy){
        Gene = gene;
        Genome = genome;
        Energy = energy;
    }   
}
