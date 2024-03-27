using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRenderableGrid
{
    public ref List<Vector2Short> GetChangedCells();
    public void AddChangedCell(Vector2Short pos);
    public void ClearChangedCells();
}
