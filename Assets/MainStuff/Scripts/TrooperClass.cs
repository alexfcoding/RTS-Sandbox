﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrooperClass : SeekerClass
{
    public Vector3 pointFromShooting;
    public Vector3 pointFromShootingRandomize;
    
    public bool destinationPass;
    public bool wait;
    public bool foundTargetToAttack;
    public bool targetIsShip;
    public bool ignorePlayerCommandAndStay;

    public bool stopDoing;

    public int attackTargetId;

    public GameObject enemyToLook;
    public GameObject targetToChase;
    public GameObject targetToChaseByPlayerCommand;
    public GameObject teamSelectMark;
    
    public float trooperSpeed;
    public float distToLerp;
    public float timer;
    
    public Rigidbody rbTrooper;

    public override void Awake()
    {
        level = 1;
        attackTargetId = Random.Range(0, 6);
        targetIsShip = true;
        health = 1000;
        maxHP = 1000;
        destinationPass = false;
        foundTargetToAttack = false;
        wait = false;
        distToLerp = 170;
        gameObject.tag = "Trooper";
        wait = true;
        alreadyHaveWeapon = false;
        dead = false;
        foundObject = false;
        isVulnerable = true;

        textHP = Instantiate(GameMaster.GM.text3dDamage, transform.position + new Vector3(0, 10, 0), Quaternion.Euler(0, 0, 0));
        textHP.gameObject.GetComponent<TextMesh>().text = level.ToString();
        textHP.transform.parent = transform;

        healthBarScaleMultiplier = 0.8f;
        healthBar = Instantiate(GameMaster.GM.healthBar, transform.position + new Vector3(0, 5, 0), Quaternion.Euler(0, 0, 0));
        healthBar.transform.localScale = new Vector3(health / maxHP * healthBarScaleMultiplier, 0.05f, 1);
        healthBar.transform.SetParent(gameObject.transform);
        pointFromShootingRandomize.Set(Random.Range(-40, 40), 0, Random.Range(-40, 40));
    }
    
    public override void Update()
    {
        FindItemsAround(GameMaster.GM.globalObjectList, GameMaster.GM.platformObjectList);

        //Vector3 MoveSin = new Vector3(transform.position.x, 0.2f, transform.position.z);
        //    gameObject.GetComponent<Rigidbody>().MovePosition(MoveSin);

        //HealthBar.transform.LookAt(Camera.main.transform.position, -Vector3.up);
    }

    public override void FindItemsAround(List<GameObject> _GlobalObjectList, List<GameObject> _PlatformList)
    {
        if (dead == false && stopDoing == false)
        {
            if (targetToChase != null)
            {
                if (targetToChase.GetComponent<ShipClass>() != null)
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

                while (rnDShip == fractionId)
                    rnDShip = Random.Range(0, GameMaster.GM.mainBaseCount);

                if (fractionId != 0 && GameMaster.GM.shipObjectList[rnDShip] != null)
                    targetToChase = GameMaster.GM.shipObjectList[rnDShip];
                //else
                //    targetToChase = GameMaster.GM.player.gameObject;
            }
           
            if (targetToChase != null && targetToChase.GetComponent<ShipClass>() != null)
                pointFromShooting = pointFromShooting + pointFromShootingRandomize * 4;

            if (targetToChase != null && targetToChase.GetComponent<ShipClass>() == null)
                pointFromShooting = pointFromShooting + pointFromShootingRandomize;

            if ((currentWeapon != null && enemyToLook != null) || (currentWeapon != null && enemyToLook != null) )
            {
                if (enemyToLook.GetComponent<ShipClass>() != null)
                {
                    if (gameObject.name == "LightShip")
                    {
                        Quaternion lookOnLook = Quaternion.LookRotation(enemyToLook.transform.position - transform.position);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 4);
                    }
                        
                    wait = true;
                }
                else if (enemyToLook.GetComponent<ShipClass>() == null)
                {
                    Quaternion lookOnLook = Quaternion.LookRotation(enemyToLook.transform.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 4);
                    wait = true;
                }
            }
            else
            {
                wait = false;
            }

            // если враг мертв, то двигаться дальше
            if (enemyToLook != null && enemyToLook.GetComponent<SeekerClass>() != null && enemyToLook.GetComponent<SeekerClass>().dead == true && currentWeapon.GetComponent<WeaponClass>().playerFollowingCommand == true)
                wait = false;
                            
           if ( ((Vector3.Distance(transform.position, pointFromShooting)) > 80) && (wait == false) )
                {

                    Quaternion lookOnLook = Quaternion.LookRotation(pointFromShooting - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 4);

                    rbTrooper.AddRelativeForce(Vector3.forward * trooperSpeed * Time.deltaTime * 40, ForceMode.VelocityChange);//* Time.deltaTime * 30
                    if (gameObject.GetComponent<Animator>() != null)
                        gameObject.GetComponent<Animator>().Play("Run_Guard");
                }
            else
                {
                    wait = true;
                    if (gameObject.GetComponent<Animator>() != null)
                        gameObject.GetComponent<Animator>().Play("Idle");
                }

            if (((Vector3.Distance(transform.position, pointFromShooting)) > 120))
                {
                    wait = false;
                }
        }

        // Анимация смерти
        if (dead == true && stopDoing == false)
        {
            if (gameObject.GetComponent<Animator>() != null)
                gameObject.GetComponent<Animator>().Play("Die");

            stopDoing = true;
        }
    }

    public virtual void OnCollisionStay(Collision collisioninfo)
    {
        if (collisioninfo.gameObject.GetComponent<BuildingClass>() != null)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(0, 30, 0, ForceMode.Impulse);
        }
    }
}
