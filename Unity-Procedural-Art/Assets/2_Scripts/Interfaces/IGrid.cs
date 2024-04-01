using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrid<T>
{
    public Vector2Short GridSize { get; }

    //-------------------------------
    
    public T Get(Vector2Short pos);
    public void Set(Vector2Short pos, T newData);
    public bool IsInBounds(Vector2Short pos);
}
