using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watenk;

public class ListGrid<T> : IGrid<T>
{
    public Vector2Short GridSize { get; }

    private List<List<ListData<T>>> values;

    //-----------------------------------

    public ListGrid(Vector2Short size){
        GridSize = size;
        values = new List<List<ListData<T>>>();

        for (int x = 0; x < size.x; x++){
            values.Add(new List<ListData<T>>());
        }
    }

    public T Get(Vector2Short pos){

        ListData<T> listData = values[pos.x].Find((data) => pos == data.Pos);
        if (!listData.Equals(default(ListData<T>))){
            return listData.Data;
        }
        return default;
    }

    public void Set(Vector2Short pos, T newData){
        values[pos.x].Add(new ListData<T>(pos, newData));
    }

    public bool IsInBounds(Vector2Short pos){
        return GridUtility.IsInBounds(pos, Vector2Short.Zero, GridSize);
    }
}

public struct ListData<T>{
    public Vector2Short Pos;
    public T Data;

    //----------------------------

    public ListData(Vector2Short pos, T data){
        Pos = pos;
        Data = data;
    }
}
