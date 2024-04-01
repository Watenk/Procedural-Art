using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watenk;

public class ArrayGrid<T> : IGrid<T> where T : new()
{
    public Vector2Short GridSize { get; }

    private T[,] data;

    //----------------------------------------------

    public ArrayGrid(Vector2Short size){
        GridSize = size;

        data = new T[size.x, size.y];
        for (int y = 0; y < size.y; y++){
            for(int x = 0; x < size.x; x++){
                data[x, y] = new T();
            }
        }
    }

    public T Get(Vector2Short pos){
        return data[pos.x, pos.y];
    }

    public void Set(Vector2Short pos, T newData){
        data[pos.x, pos.y] = newData;
    }

    public bool IsInBounds(Vector2Short pos){
        return GridUtility.IsInBounds(pos, Vector2Short.Zero, GridSize);
    }
}
