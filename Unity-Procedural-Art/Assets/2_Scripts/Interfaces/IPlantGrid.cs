using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlantGrid 
{
    public Vector2Short GridSize { get; }

    //-------------------------------
    
    public PlantCell GetCell(Vector2Short pos);
    public bool IsInBounds(Vector2Short pos);
}
