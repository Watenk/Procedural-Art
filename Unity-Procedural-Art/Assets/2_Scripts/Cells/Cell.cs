using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public CellTypes CellType;

    //---------------------------

    private Cell() {}

    public Cell(CellTypes cellType){
        CellType = cellType;
    }
}
