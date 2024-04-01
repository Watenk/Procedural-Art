using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light
{
    public byte LightLevel { get; set; }

    //------------------------------
    
    private Light() {}

    public Light(byte lightLevel){
        LightLevel = lightLevel;
    }
}
