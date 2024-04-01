using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopwatch 
{
    public bool Runnning { get; private set; }
    public float Time { get; private set; }

    private float previousTime;

    //---------------------------------------

    public Stopwatch(){
        Runnning = true;
        previousTime = UnityEngine.Time.realtimeSinceStartup;
    }

    public void Update(){
        if (Runnning){
            float currentTime = UnityEngine.Time.realtimeSinceStartup;
            float timeDifference = currentTime - previousTime;
            Time += timeDifference;
            previousTime = currentTime;
        }
    }

    public void Start(){
        Runnning = true;
    }

    public float Stop(){
        Runnning = false;
        return Time;
    }

    public void Reset(){
        Time = 0;
        previousTime = UnityEngine.Time.realtimeSinceStartup;
        Runnning = true;
    }

    public void SetTime(float time){
        Time = time;
    }
}
