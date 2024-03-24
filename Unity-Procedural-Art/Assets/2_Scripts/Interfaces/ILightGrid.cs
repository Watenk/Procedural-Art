using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILightGrid 
{
    public Vector2Short GridSize { get; }

    //-------------------------------
    
    public ref byte GetCell(Vector2Short pos);
    public bool IsInBounds(Vector2Short pos);
}
