using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchColor : MonoBehaviour
{

    public GameObject light;
    public bool triggered = true;
    int id;    

    void Start()
    {
        id = gameObject.GetComponentInParent<FactionIndex>().factionId;
        InvokeRepeating("SwitchColor", 0f, 0.5f);
    }
    
    public void SwitchColor()
    {
        if (triggered)
        {
            light.gameObject.GetComponent<Light>().intensity = 10;
            light.gameObject.GetComponent<Light>().color = GameMaster.GM.fractionColors[id];
            triggered = false;
        }
        else
        {
            light.gameObject.GetComponent<Light>().intensity = 0;
            triggered = true;
        }            
    }
}
