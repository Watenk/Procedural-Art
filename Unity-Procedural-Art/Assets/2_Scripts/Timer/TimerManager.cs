using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TimerManager : IUpdateable
{
    private List<Timer> timers = new List<Timer>();
    private List<Stopwatch> stopwatches = new List<Stopwatch>();

    //-----------------------------------------------------

    public void OnUpdate(){
        foreach (Timer current in timers){
            current.Update();
        }
        foreach (Stopwatch current in stopwatches){
            current.Update();
        }
    }

    public Timer AddTimer(float lenght){
        Timer newTimer = new Timer(lenght);
        timers.Add(newTimer);
        return newTimer;
    }

    public void RemoveTimer(Timer timer){
        timers.Remove(timer);
    }

    public Stopwatch AddStopwatch(){
        Stopwatch newStopwatch = new Stopwatch();
        stopwatches.Add(newStopwatch);
        return newStopwatch;
    }

    public void RemoveStopwatch(Stopwatch stopwatch){
        stopwatches.Remove(stopwatch);
    }
}