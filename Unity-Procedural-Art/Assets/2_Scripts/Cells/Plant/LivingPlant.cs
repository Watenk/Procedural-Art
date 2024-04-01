using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingPlant
{
    public ushort LifeTime;
    public byte Energy;

    //---------------------------

    private LivingPlant() {}

    public LivingPlant(byte energy){
        LifeTime = 0;
        Energy = energy;
    }
}
