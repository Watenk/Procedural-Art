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
    public Vector2Int GridSize;

    [Header("GridRendering")]
    public Vector2Int DesiredChunkSize;
    public Material Atlas;
    [Tooltip("Size of atlas texture in pixels")]
    public Vector2Int AtlasSize;
    [Tooltip("Size of 1 texture in the atlas in pixels")]
    public Vector2Int SpriteTextureSize;
    [Tooltip("Amount of uv texture that is cut off to prevent graphical artifacts due to float precition")]
    public Vector2 UVFloatErrorMargin;

    [Header("UI")]
    [Tooltip("In Seconds")]
    public float ProfilerUpdateSpeed;
}
