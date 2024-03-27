using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlantGrowEnergyRequirement
{
    public CellTypes cell;
    public byte energyAmount;
}
