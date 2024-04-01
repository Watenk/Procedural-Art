using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Light
{
    public byte LightLevel { get; set; }

    //------------------------------

    public Light(byte lightLevel){
        LightLevel = lightLevel;
    }
}
