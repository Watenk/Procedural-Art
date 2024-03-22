using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class References : MonoBehaviour
{
    public static References Instance { 
        get{
            if (instance == null){
                Debug.LogError("Couldn't find References Singleton");
            }
            return instance;
        }
    }
    private static References instance;

    public void Awake(){
        instance = this;
    }

    //--------------------------------------------

    public Text ProfilerText { get { return profilerText; } }
    [SerializeField] private Text profilerText;
}
