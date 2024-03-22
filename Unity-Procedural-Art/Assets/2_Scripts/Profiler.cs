using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watenk;

// TODO: Implement ThreadUpdate times

public class Profiler
{
    public float FPS { get; private set; }
    public float UPS { get; private set; }

    // FrameTimes
    public float FrameTime { get; private set; } // Total frame time
    public float RemainingTime { get; private set; } // Unity's scripts
    public float UpdateTime { get; private set; } // Update loop
    public float PhysicsUpdateTime { get; private set; } // Physics update loop

    public Dictionary<Type, Stopwatch> UpdateStopwatches = new Dictionary<Type, Stopwatch>();
    public Dictionary<Type, Stopwatch> PhysicsUpdateStopwatches = new Dictionary<Type, Stopwatch>();

    private Timer upsTimer;
    private int upsCount;

    // Dependencies
    private TimerManager timerManager;

    //---------------------------------------

    public Profiler(){
        timerManager = GameManager.GetService<TimerManager>();
        upsTimer = timerManager.AddTimer(1.0f);
        upsTimer.OnTimerIsDone += OnUPSTimer;

        EventManager.AddListener<Dictionary<Type, object>>(Events.OnProfilerInit, Init);
        EventManager.AddListener<Type>(Events.OnProfilerStart, Start);
        EventManager.AddListener<Type>(Events.OnProfilerStop, Stop);
        EventManager.AddListener(Events.OnProfilerCalcTimes, CalcFrameTimes);
        EventManager.AddListener(Events.OnProfilerUPS, OnProfilerUPS);
    }

    public void CalcTimes(){
        CalcFrameTimes();
    }

    public void Init(Dictionary<Type, object> services){
        foreach (var current in services){
            if (current.Value is IUpdateable){
                UpdateStopwatches.Add(current.Key, timerManager.AddStopwatch());
            }
            else if (current.Value is IPhysicsUpdateable){
                PhysicsUpdateStopwatches.Add(current.Key, timerManager.AddStopwatch());
            }
        }
    }

    public void Start(Type service){
        Stopwatch stopwatch = GetStopwatch(service);
        if (stopwatch == null) return;

        stopwatch.Reset();
    }

    public void Stop(Type service){
        Stopwatch stopwatch = GetStopwatch(service);
        if (stopwatch == null) return;

        stopwatch.Stop();
    }

    //----------------------------------------

    private void CalcFrameTimes(){
        UpdateTime = 0;
        PhysicsUpdateTime = 0;
        foreach (KeyValuePair<System.Type, Stopwatch> kvp in UpdateStopwatches) { UpdateTime += kvp.Value.Time; }
        foreach (KeyValuePair<System.Type, Stopwatch> kvp in PhysicsUpdateStopwatches) { PhysicsUpdateTime += kvp.Value.Time; }
        UpdateTime *= 1000;
        PhysicsUpdateTime *= 1000;
        FrameTime = MathUtility.SetDecimals(Time.deltaTime * 1000, 1);
        FPS = MathUtility.SetDecimals(1000 / FrameTime, 1);
        RemainingTime = MathUtility.SetDecimals(FrameTime - (UpdateTime + PhysicsUpdateTime), 1);
    }

    private Stopwatch GetStopwatch(Type type){
        if (UpdateStopwatches.TryGetValue(type, out Stopwatch updateStopwatch)) return updateStopwatch;
        else if (UpdateStopwatches.TryGetValue(type, out Stopwatch physicsUpdateStopwatch)) return physicsUpdateStopwatch;
        return null;
    }

    private void OnProfilerUPS(){
        upsCount++;
    }

    private void OnUPSTimer(){
        UPS = upsCount;
        upsCount = 0;
        upsTimer.Reset();
    }
}
