using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : IPhysicsUpdateable
{
    // Settings: Gravity, Atmosphere, Light, wind, radiation, Temperature
    private List<Plant> plants = new List<Plant>();
    private int geneAmount;

    //---------------------------------------------------

    public PlantManager(){
        geneAmount = Settings.Instance.GeneAmount;

        EventManager.AddListener<Vector2Int>(Events.OnLeftMouseDown, OnLeftMouseDown);
    }

    public void OnPhysicsUpdate(){
        foreach (Plant current in plants){
            current.OnPhysicsUpdate();
        }
    }

    public void AddPlant(Vector2Int pos){
        plants.Add(new Plant(pos, geneAmount));
    }

    //--------------------------------------------------

    private void OnLeftMouseDown(Vector2Int mousePos){
        AddPlant(mousePos);
    }
}
