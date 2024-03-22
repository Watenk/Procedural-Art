using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Optimization
public static class EventManager
{
    private static Dictionary<Type, object> eventManagers = new Dictionary<Type, object>();

    //--------------------------------------------------

    public static void AddListener(Events eventName, Action listener){
        Get().AddListener(eventName, listener);
    }

    public static void AddListener<T>(Events eventName, Action<T> listener){
        Get<T>().AddListener(eventName, listener);
    }

    public static void RemoveListener(Events eventName, Action listener){
        Get().RemoveListener(eventName, listener);
    }

    public static void RemoveListener<T>(Events eventName, Action<T> listener){
        Get<T>().RemoveListener(eventName, listener);
    }

    public static void Invoke(Events eventName){
        Get().Invoke(eventName);
    }

    public static void Invoke<T>(Events eventName, T eventParam){
        Get<T>().Invoke(eventName, eventParam);
    }

    //---------------------------------------------------

    private static EventManagerNoParameter Get(){
        Type type = typeof(EventManagerNoParameter);

        if (eventManagers.ContainsKey(type)){
            return eventManagers[type] as EventManagerNoParameter;
        }
        else{
            EventManagerNoParameter newEventManager = new EventManagerNoParameter();
            eventManagers.Add(type, newEventManager);
            return newEventManager;
        }
    }

    private static EventManagerParameter<T> Get<T>(){
        Type type = typeof(T);

        if (eventManagers.ContainsKey(type)){
            return eventManagers[type] as EventManagerParameter<T>;
        }
        else{
            EventManagerParameter<T> newEventManager = new EventManagerParameter<T>();
            eventManagers.Add(type, newEventManager);
            return newEventManager;
        }
    }
}

public class EventManagerNoParameter
{
    private Dictionary<Events, Action> eventDictionary = new Dictionary<Events, Action>();

    //---------------------------------------------------

    public void AddListener(Events eventName, Action listener){
        if (eventDictionary.TryGetValue(eventName, out Action currentEvent)){
            currentEvent += listener;
            eventDictionary[eventName] = currentEvent;
        }
        else{
            currentEvent += listener;
            eventDictionary.Add(eventName, currentEvent);
        }
    }

    public void RemoveListener(Events eventName, Action listener){
        if (eventDictionary.TryGetValue(eventName, out Action currentEvent)){
            currentEvent -= listener;
            eventDictionary[eventName] = currentEvent;
        }
    }

    public void Invoke(Events eventName){
        if (eventDictionary.TryGetValue(eventName, out Action thisEvent)){
            thisEvent.Invoke();
        }
        #if UNITY_EDITOR
            else{
                Debug.LogWarning("Tried to invoke event " + eventName + " but there are no listeners");
            }
        #endif
    }
}

public class EventManagerParameter<T>
{
    private Dictionary<Events, Action<T>> eventDictionary = new Dictionary<Events, Action<T>>();

    //---------------------------------------------------

    public void AddListener(Events eventName, Action<T> listener){
        Action<T> currentEvent;
        if (eventDictionary.TryGetValue(eventName, out currentEvent)){
            currentEvent += listener;
            eventDictionary[eventName] = currentEvent;
        }
        else{
            currentEvent += listener;
            eventDictionary.Add(eventName, currentEvent);
        }
    }

    public void RemoveListener(Events eventName, Action<T> listener){
        Action<T> currentEvent;
        if (eventDictionary.TryGetValue(eventName, out currentEvent)){
            currentEvent -= listener;
            eventDictionary[eventName] = currentEvent;
        }
    }

    public void Invoke(Events eventName, T eventParam){
        Action<T> currentEvent = null;
        if (eventDictionary.TryGetValue(eventName, out currentEvent)){
            currentEvent.Invoke(eventParam);
        }
        #if UNITY_EDITOR
            else{
                Debug.LogWarning("Tried to invoke event " + eventName + " but there are no listeners");
            }
        #endif
    }
}