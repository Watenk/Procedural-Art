using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Cell 
{
    public Vector2Int Pos { get; private set; }
    public Cells Cells {get{ return cells;} set{cells = value; drawable.AddChangedCell(Pos);}}
    private Cells cells;
    public Plant Plant;
    public int Lightlevel; 

    private IGridDrawable drawable;

    //-----------------------------------

    public Cell(Vector2Int pos, IGridDrawable drawable){
        Pos = pos;
        this.drawable = drawable;
    }
}
