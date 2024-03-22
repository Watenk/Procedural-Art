using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer 
{
    public Action OnTimerIsDone;

    public float Lenght { get; private set; }
    public float Time { get; private set; }

    private bool timerIsDone = false;
    private float previousTime;


    //--------------------------------------------
    
    public Timer(float lenght){
        this.Lenght = lenght;
        Time = lenght;
        previousTime = UnityEngine.Time.realtimeSinceStartup;
    }
    
    public void Update(){

        if (timerIsDone) return;

        if (Time >= 0){
            float currentTime = UnityEngine.Time.realtimeSinceStartup;
            float TimeDifference = currentTime - previousTime;
            Time -= TimeDifference;
            previousTime = currentTime;
        }
        else{
            timerIsDone = true;
            if (OnTimerIsDone != null) OnTimerIsDone();
        }
    }

    public void ChangeLenght(float newLenght){
        Lenght = newLenght;
    }

    public void ChangeTime(float newTime){
        Time = newTime;
        timerIsDone = false;
    }

    public void Reset(){
        Time = Lenght;
        timerIsDone = false;
        previousTime = UnityEngine.Time.realtimeSinceStartup;
    }
}
