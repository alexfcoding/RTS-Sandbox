using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowAnimation : MonoBehaviour
{
    public float tickBuilding;
    public float buildingHeightScale;    
    public float buildSpeed;    
        
    public void Start()
    {        
        tickBuilding = 0.05f;
        InvokeRepeating("ConstructBuilding", 0f, 0.01f); // 0.1f = Max TickBuilding (10)/(10sec/0.1)        
    }       

    public void ConstructBuilding ()
    {
        if (tickBuilding < buildingHeightScale)
        {
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, tickBuilding, gameObject.transform.localScale.z);
            tickBuilding += buildSpeed;
        }
        else if (tickBuilding > buildingHeightScale)
        {           
            CancelInvoke("ConstructBuilding");
        }
    }       
}
