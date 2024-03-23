using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICellGrid 
{
    public Vector2Int GridSize { get; }

    //-------------------------------
    
    public ref Cell GetCell(Vector2Int pos);
    public bool IsInBounds(Vector2Int pos);
}
