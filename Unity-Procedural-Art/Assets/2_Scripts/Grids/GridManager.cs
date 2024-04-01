using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager
{
    public Vector2Short GridSize { get; private set; }

    private Dictionary<Type, object> arrayGrids = new Dictionary<Type, object>();
    private Dictionary<Type, object> listGrids = new Dictionary<Type, object>();

    //-----------------------------------

    public GridManager(){
        GridSize = Settings.Instance.GridSize;

        AddArrayGrid<Cell>();
        AddArrayGrid<Light>();

        AddListGrid<SeedPlant>();
        AddListGrid<GrowingPlant>();
        AddListGrid<LivingPlant>();
        AddListGrid<DeadPlant>();
    }

    public IGrid<T> GetGrid<T>() where T : new(){
        if (arrayGrids.TryGetValue(typeof(T), out object arrayGrid)){
            return (ArrayGrid<T>)arrayGrid;
        }
        else if (listGrids.TryGetValue(typeof(T), out object listGrid)){
            return (ListGrid<T>)listGrid;
        }
        else{
            Debug.LogError("Couldn't find grid of type " + typeof(T).ToString());
            return null;
        }
    }

    public IGrid<T> AddArrayGrid<T>() where T : new(){
        ArrayGrid<T> newGrid = new ArrayGrid<T>(GridSize);
        arrayGrids[typeof(T)] = newGrid;
        return newGrid;
    }

    public IGrid<T> AddListGrid<T>() where T : new(){
        ListGrid<T> newGrid = new ListGrid<T>(GridSize);
        listGrids[typeof(T)] = newGrid;
        return newGrid;
    }
}
