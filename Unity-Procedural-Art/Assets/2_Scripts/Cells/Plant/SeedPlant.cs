using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedPlant
{
    public ushort LifeTime;
    public Genome Genome;
    public byte Energy;

    //----------------------------------

    private SeedPlant() {}

    public SeedPlant(Genome genome, byte startEnergy){
        LifeTime = 0;
        Genome = genome;
        Energy = startEnergy;
    }
}
