using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerClass : SeekerClass
{
    // Start is called before the first frame update
    public override void Awake()
    {
        level = 1;
       
        minDistance = 1000;
        minDistNum = 0;
        countOfItemsCollected = 0;
        alreadyHaveWeapon = true;
        health = 60000;
        maxHP = 60000;
        dead = false;
        foundObject = false;
        isVulnerable = true;
        textHP = Instantiate(GameMaster.GM.text3dDamage, transform.position + new Vector3(0, 18, 0), Quaternion.Euler(0, 0, 0));
        textHP.gameObject.GetComponent<TextMesh>().text = level.ToString();
        textHP.transform.parent = transform;
        healthBarScaleMultiplier = 0.5f;
        healthBar = Instantiate(GameMaster.GM.healthBar, transform.position + new Vector3(0, 50, 0), Quaternion.Euler(0, 0, 0));
        healthBar.transform.SetParent(gameObject.transform);
        healthBar.transform.localScale = new Vector3(health / maxHP * healthBarScaleMultiplier, 0.05f, 1);
    }

    public override void FindItemsAround(List<GameObject> objectList, List<GameObject> platformList)
    {

    }

    
}

