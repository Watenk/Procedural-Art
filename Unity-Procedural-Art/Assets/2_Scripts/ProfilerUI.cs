using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Watenk;

public class ProfilerUI
{
    private Text profilerText;
    private Timer updateTimer;

    // Dependencies
    private Profiler profiler;

    //------------------------------------

    public ProfilerUI(){
        profilerText = References.Instance.ProfilerText;
        profiler = GameManager.GetService<Profiler>();
        updateTimer = GameManager.GetService<TimerManager>().AddTimer(Settings.Instance.ProfilerUpdateSpeed);

        updateTimer.OnTimerIsDone += UpdateUI;
    }

    //-----------------------------------

    private void UpdateUI(){

        EventManager.Invoke(Events.OnProfilerCalcTimes);

        string debugTextString = "";
        
        debugTextString +=  "FPS: " + profiler.FPS + "  /  " + profiler.FrameTime + "ms" + "\n";
        debugTextString += "UPS: " + profiler.UPS + "\n";
        debugTextString += "\n";

        // Updates
        debugTextString += "Update: " + MathUtility.SetDecimals(profiler.UpdateTime, 2) + "ms" + "\n";
        foreach (KeyValuePair<Type, Stopwatch> kvp in profiler.UpdateStopwatches){
            debugTextString += "   -" + kvp.Key + ": " + MathUtility.SetDecimals(kvp.Value.Time * 1000, 2) + "ms" + "\n";
        }
        debugTextString += "\n";

        // Physics Updates
        debugTextString += "PhysicsUpdate: " + MathUtility.SetDecimals(profiler.PhysicsUpdateTime, 2) + "ms" + "\n";
        foreach (KeyValuePair<Type, Stopwatch> kvp in profiler.PhysicsUpdateStopwatches){
            debugTextString += "   -" + kvp.Key + ": " + MathUtility.SetDecimals(kvp.Value.Time * 1000, 2) + "ms" + "\n";
        }
        debugTextString += "\n";

        // Remaining time
        debugTextString += "Remaining: " + MathUtility.SetDecimals(profiler.RemainingTime, 2) + "ms" + "\n";
        debugTextString += "\n";

        profilerText.text = debugTextString;

        updateTimer.Reset();
    }
}
