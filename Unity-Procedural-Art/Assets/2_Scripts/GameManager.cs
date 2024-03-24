using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { 
        get{
            if (instance == null){
                Debug.LogError("Couldn't find GameManager Singleton");
            }
            return instance;
        }
    }
    private static GameManager instance;

    //---------------------------------------------------------

    private static Dictionary<System.Type, object> services = new Dictionary<System.Type, object>();
    private static List<IUpdateable> updateables = new List<IUpdateable>();
    private static List<IPhysicsUpdateable> physicsUpdateables = new List<IPhysicsUpdateable>();
    private TimerManager timerManager;
    private Timer physicsUpdateTimer;

    //----------------------------------------

    public void Start(){
        instance = this;

        timerManager = AddService(new TimerManager());
        AddService(new Profiler());

        AddService(new CellGridManager());
        MapGenerator mapGenerator = new MapGenerator();
        AddService(new CellGridRenderer());
        AddService(new LightManager());
        //AddService(new LightGridDebugger());
        AddService(new PlantManager());
        AddService(new PlantGridDebugger());

        AddService(new InputHandler());
        AddService(new CameraController());

        AddService(new UIManager());

        EventManager.Invoke(Events.OnProfilerInit, services);

        physicsUpdateTimer = timerManager.AddTimer(1 / Settings.Instance.UPSTarget);
        physicsUpdateTimer.OnTimerIsDone += PhysicsUpdate;
    }

    public void Update(){
        foreach (IUpdateable current in updateables){ 
            EventManager.Invoke(Events.OnProfilerStart, current.GetType());
            timerManager.OnUpdate();
            current.OnUpdate(); 
            timerManager.OnUpdate();
            EventManager.Invoke(Events.OnProfilerStop, current.GetType());
        }
    }

    public void PhysicsUpdate(){
        foreach (IPhysicsUpdateable current in physicsUpdateables){ 
            EventManager.Invoke(Events.OnProfilerStart, current.GetType());
            timerManager.OnUpdate();
            current.OnPhysicsUpdate(); 
            timerManager.OnUpdate();
            EventManager.Invoke(Events.OnProfilerStop, current.GetType());
        }
        physicsUpdateTimer.Reset();
        EventManager.Invoke(Events.OnProfilerUPS);
    }

    public static T GetService<T>(){
        services.TryGetValue(typeof(T), out object service);

        #if UNITY_EDITOR
            if (service == null) { Debug.LogError(typeof(T).Name + " Sevice not found"); }
        #endif

        return (T)service;
    }

    //---------------------------------------

    private T AddService<T>(T service){
        services.Add(typeof(T), service);

        if (service is IUpdateable) { updateables.Add((IUpdateable)service); }
        if (service is IPhysicsUpdateable) { physicsUpdateables.Add((IPhysicsUpdateable)service); }
        return service;
    }
}
