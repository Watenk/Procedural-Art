using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell 
{
    public Cells Cells { get; private set; }

    //-------------------------------------

    public Cell(Cells cells){
        Cells = cells;
    }

    public void SetCells(Cells cells){
        Cells = cells;
    }
}
