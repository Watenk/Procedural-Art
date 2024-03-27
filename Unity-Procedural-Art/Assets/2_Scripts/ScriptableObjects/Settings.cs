using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Settings")]
public class Settings : ScriptableObject
{
    public static Settings Instance { 
        get{
            if (instance == null){
                instance = Resources.Load<Settings>("Settings");

                #if UNITY_EDITOR
                    if (instance == null) {Debug.LogError("Settings couldn't be loaded...");}
                #endif
            }
            return instance;
        }
    }
    private static Settings instance;

    //------------------------------------------------------------------

    [Header("Camera")]
    public float MinCamSize;
    public float MaxCamSize;
    public float ScrollSpeed;

    [Header("Physics")]
    public float UPSTarget;

    [Header("Grid")]
    public Vector2Short GridSize;

    [Header("GridRendering")]
    public Vector2Short DesiredChunkSize;
    [Tooltip("Amount of uv texture that is cut off to prevent graphical artifacts due to float precition")]
    public Vector2 UVFloatErrorMargin;

    [Header("CellTypeAtlas")]
    public Material CellTypeAtlas;
    [Tooltip("Size of atlas texture in pixels")]
    public Vector2Short CellTypeAtlasSize;
    [Tooltip("Size of 1 sprite in the atlas texture in pixels")]
    public Vector2Short CellTypeSpriteSize;

    [Header("UI")]
    [Tooltip("In Seconds")]
    public float ProfilerUpdateSpeed;

    [Header("Light")]
    public int MinLight;
    public int MaxLight;


    [Header("Plants")]
    [Tooltip("How many genes 1 plant has")]
    public byte GeneAmount;
    [Tooltip("How many inactive genes 1 plant has (Amount of genes that will not grow in a genome)")]
    public byte InactiveChromosomeAmount;
    [Tooltip("Chance a seed Chromosome mutates")]
    public float ChromosomeMutationChance;
    [Tooltip("")]
    public List<PlantGrowEnergyRequirement> plantGrowEnergyRequirements = new List<PlantGrowEnergyRequirement>();

    [Header("Prefabs")]
    public GameObject SingleValueGridPrefab;
    public GameObject GeneGridDebugPrefab;
}
