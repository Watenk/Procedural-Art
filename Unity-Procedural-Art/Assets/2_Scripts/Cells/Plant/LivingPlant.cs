using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingPlant
{
    public byte EatTime;
    public ushort LifeTime;
    public byte Energy;

    //---------------------------

    private LivingPlant() {}

    public LivingPlant(byte energy){
        LifeTime = 0;
        EatTime = 0;
        Energy = energy;
    }
}
