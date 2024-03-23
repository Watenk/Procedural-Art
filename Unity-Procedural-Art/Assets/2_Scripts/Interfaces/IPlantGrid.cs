using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlantGrid 
{
    public Vector2Int GridSize { get; }

    //-------------------------------
    
    public ref Plant GetCell(Vector2Int pos);
    public bool IsInBounds(Vector2Int pos);
}
