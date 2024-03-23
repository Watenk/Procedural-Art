using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILightGrid 
{
    public Vector2Int GridSize { get; }

    //-------------------------------
    
    public ref byte GetCell(Vector2Int pos);
    public bool IsInBounds(Vector2Int pos);
}
