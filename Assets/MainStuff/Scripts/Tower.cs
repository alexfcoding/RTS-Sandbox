using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tower : Seeker
{
    //public float setHealth;

    // Start is called before the first frame update
    public float healthBarTopPosition;
    public float levelTopPosition;

    public override void Awake()
    {
        level = 1;
       
        minDistance = 1000;
        minDistNum = 0;
        countOfItemsCollected = 0;
        alreadyHaveWeapon = true;
        //health = 5000;
        //maxHP = 5000;
        dead = false;
        foundObject = false;
        //isVulnerable = true;

        textHP = Instantiate(GameMaster.GM.text3dDamage, transform.position + new Vector3(0, levelTopPosition, 0), Quaternion.Euler(0, 0, 0));
        textHP.gameObject.GetComponent<TextMesh>().text = level.ToString();
        textHP.transform.parent = transform;

        healthBarScaleMultiplier = 0.3f;
        healthBar = Instantiate(GameMaster.GM.healthBar, transform.position + new Vector3(0, healthBarTopPosition, 0), Quaternion.Euler(0, 0, 0));
        healthBar.transform.SetParent(gameObject.transform);
        healthBar.transform.localScale = new Vector3(health / maxHP * healthBarScaleMultiplier, 0.02f, 1);
    }

    public void Start()
    {
        
    }

    public override void FindItemsAround(List<GameObject> objectList, List<GameObject> platformList)
    {

    }

    
}

