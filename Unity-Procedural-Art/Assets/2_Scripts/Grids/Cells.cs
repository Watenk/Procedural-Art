using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public CellTypes CellType;
    public byte LightLevel;
    public PlantCell PlantCell;
}

public class PlantCell{

    public byte ThisGene { get; private set; }
    public Genome Genome { get; private set; }
    public byte Energy;

    //-----------------------------

    public PlantCell(byte gene, Genome genome, byte startEnergy){
        ThisGene = gene;
        Genome = genome;
        Energy = startEnergy;
    }
}
