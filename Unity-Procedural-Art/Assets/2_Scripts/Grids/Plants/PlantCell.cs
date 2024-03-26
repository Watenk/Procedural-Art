using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantCell{

    public byte ThisGene { get; private set; }
    public Genome Genome { get; private set; }
    public byte Energy;

    //-----------------------------

    public PlantCell(byte gene, Genome genome){
        ThisGene = gene;
        Genome = genome;
        Energy = 0;
    }
}