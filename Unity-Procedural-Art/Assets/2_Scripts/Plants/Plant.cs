using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant{
    public Gene Gene;
    public Genome Genome;

    //-----------------------------

    public Plant(Gene gene, Genome genome){
        Gene = gene;
        Genome = genome;
    }
}