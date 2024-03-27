using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrid 
{
    public Vector2Short GridSize { get; }

    //-------------------------------
    
    public ref Cell GetCell(Vector2Short pos);
    public bool IsInBounds(Vector2Short pos);
}
