using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RollerEnemyBase : Trooper
{
    public override void Awake()
    {
        level = 1;
        attackTargetId = Random.Range(0, 6);
        targetIsShip = true;
        health = 2000;
        maxHP = 2000;
        destinationPass = false;
        foundTargetToAttack = false;
        wait = false;
        distToLerp = 170;
        gameObject.tag = "Trooper";
        gameObject.name = "Roller";
        wait = true;
        alreadyHaveWeapon = false;
        dead = false;
        foundObject = false;
        isVulnerable = true;
        textHP = Instantiate(GameMaster.GM.text3dDamage, transform.position + new Vector3(0, 10, 0), Quaternion.Euler(0, 0, 0));
        textHP.gameObject.GetComponent<TextMesh>().text = level.ToString();
        textHP.transform.parent = transform;
        healthBarScaleMultiplier = 0.8f;
        pointFromShootingRandomize.Set(Random.Range(-40, 40), 0, Random.Range(-40, 40));
    }
        
    public override void Update()
    {
        FindItemsAround(GameMaster.GM.globalObjectList, GameMaster.GM.platformObjectList);
    }

    public override void FindItemsAround(List<GameObject> _GlobalObjectList, List<GameObject> _PlatformList)
    {
        if (dead == false && stopDoing == false)
        {
            if (targetToChase != null)
            {
                if (targetToChase.GetComponent<Ship>() != null)
                {
                    pointFromShooting = targetToChase.transform.position - new Vector3(0, targetToChase.transform.position.y, 0);
                }
                else
                {
                    pointFromShooting = targetToChase.transform.position;
                }
            }
            else
            {
                int rnDShip = Random.Range(0, GameMaster.GM.mainBaseCount);

                while (rnDShip == factionId)
                    rnDShip = Random.Range(0, GameMaster.GM.mainBaseCount);

                if (factionId != 0 && GameMaster.GM.shipObjectList[rnDShip] != null)
                    targetToChase = GameMaster.GM.shipObjectList[rnDShip];
                else
                    targetToChase = GameMaster.GM.player.gameObject;
            }

            if (targetToChase != null && targetToChase.GetComponent<Ship>() != null)
                pointFromShooting = pointFromShooting + pointFromShootingRandomize * 4;

            if (targetToChase != null && targetToChase.GetComponent<Ship>() == null)
                pointFromShooting = pointFromShooting + pointFromShootingRandomize;

            if ((currentWeapon != null && enemyToLook != null) || (currentWeapon != null && enemyToLook != null))
            {
                if (enemyToLook.GetComponent<Ship>() != null)
                {
                    wait = true;
                }
                else if (enemyToLook.GetComponent<Ship>() == null)
                {
                    wait = true;
                }
            }
            else
            {
                wait = false;
            }

            // если враг мертв, то двигаться дальше
            if (enemyToLook != null && enemyToLook.GetComponent<Seeker>() != null && enemyToLook.GetComponent<Seeker>().dead == true && currentWeapon.GetComponent<Weapon>().playerFollowingCommand == true)
                wait = false;

            if (((Vector3.Distance(transform.position, pointFromShooting)) > 80) && (wait == false))
            {
                Vector3 direction = (targetToChase.transform.position - transform.position).normalized;
                rbTrooper.AddForce(direction * 1, ForceMode.VelocityChange);
            }
            else
            {
                wait = true;
            }

            if (((Vector3.Distance(transform.position, pointFromShooting)) > 120))
            {
                wait = false;
            }
        }

        // Анимация смерти
        if (dead == true && stopDoing == false)
        {
            stopDoing = true;
        }
    }
}
