using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : FactionIndex
{
    public float tickBuilding;
    public float buildingHeightScale;
    public AudioSource beginConstruction;
    public AudioSource constructionComplete;
    public float buildSpeed;
    public Vector3 healthBarPosition;

    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        health = 5000;
        maxHP = 5000;
        tickBuilding = 0.05f;

        if (beginConstruction != null && fractionId == 0)
            beginConstruction.Play();

        InvokeRepeating("ConstructBuilding", 0f, 0.05f); // 0.1f = Max TickBuilding (10)/(10sec/0.1)
        //isVulnerable = true;
        //lootAfterDeath = false;
        healthBarScaleMultiplier = 1;
        healthBar.transform.localScale = new Vector3(health / maxHP * healthBarScaleMultiplier, 0.05f, 1);
        healthBar.transform.localPosition = healthBarPosition;
    }

    public void FixedUpdate()
    {
        healthBar.transform.LookAt(Camera.main.transform.position);
    }

    public void ConstructBuilding()
    {
        if (tickBuilding < buildingHeightScale)
        {
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, tickBuilding, gameObject.transform.localScale.z);
            tickBuilding += buildSpeed;
        }
        else if (tickBuilding > buildingHeightScale)
        {
            healthBar.transform.localPosition += new Vector3(0, 4, 0);

            //if (fractionId == 0)
            //    constructionComplete.Play();

            CancelInvoke("ConstructBuilding");
        }
    }

    public void OnCollisionEnter(Collision collisioninfo)
    {
        if (collisioninfo.gameObject.tag == "Barracs")
        {
            gameObject.transform.position = GameMaster.GM.shipObjectList[fractionId].transform.position + new Vector3(Random.Range(-100, 100), -GameMaster.GM.shipObjectList[fractionId].transform.position.y, Random.Range(-100, 100));
        }
    }
}
