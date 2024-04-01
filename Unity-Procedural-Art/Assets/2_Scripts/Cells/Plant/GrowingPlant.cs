using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingPlant
{
    public byte Gene;
    public Genome Genome;
    public byte Energy; 

    //------------------------------

    private GrowingPlant() {}

    public GrowingPlant(byte gene, Genome genome, byte startEnergy){
        Gene = gene;
        Genome = genome;
        Energy = startEnergy;
    }
}
