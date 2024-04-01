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
    public byte DirectionChromosomeAmount;
    [Tooltip("How many inactive genes 1 plant has (Amount of genes that will not grow in a genome)")]
    public byte InactiveDirectionChromosomeAmount;
    [Tooltip("Chance a seed Chromosome mutates")]
    public float ChromosomeMutationChance;
    [Tooltip("Age at which a plant dies (in updates)")]
    public ushort PlantDieAge;
    [Tooltip("Age at which a seed grows (in updates)")]
    public ushort SeedGrowAge;
    [Tooltip("Age at which a plant rots away (in updates)")]
    public ushort RotAge;
    [Tooltip("Delay in updates that a plant eats (in updates)")]
    public ushort EatingDelay;
    public byte WoodEnergyTransferAmount;
    public byte LeavesEnergyTransferAmount;
    [Tooltip("Energy requirement to grow a certain type of plant")]
    public List<PlantGrowEnergyRequirement> plantGrowEnergyRequirements = new List<PlantGrowEnergyRequirement>();

    [Header("Prefabs")]
    public GameObject SingleValueGridPrefab;
    public GameObject GeneGridDebugPrefab;
}
