using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]

public class Settings : MonoBehaviour
{
    public static Settings SET;    

    /// <summary>
    /// Count of base ships (including player)
    /// </summary>
    public int mainBaseCount;
    public int startMoney;
    public int unitsCount;
    public int attackProbability;
    public bool aiPlayerBase;

    void Awake()
    {
        if (SET != null)
            GameObject.Destroy(SET);
        else
            SET = this;
                
        DontDestroyOnLoad(this);               
    }        
}
