﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trooper : Seeker
{
    public Vector3 pointFromShooting;
    public Vector3 pointFromShootingRandomize;
    public Vector3 pointToChase;

    public bool destinationPass;
    public bool wait;
    public bool foundTargetToAttack;
    public bool targetIsShip;
    public bool ignorePlayerCommandAndStay;
    public bool stopDoing;
    public bool rotateBody;

    public int attackTargetId;
    public int randomCollisionStuckDirection;

    public GameObject enemyToLook;
    public GameObject targetToChase;
    public GameObject targetToChaseByPlayerCommand;
    public GameObject teamSelectMark;
    public GameObject body;

    public float trooperSpeed;
    public float distToLerp;
    public float timer;
    float randomSpeedDeviation;

    public Rigidbody rbTrooper;        

    public override void Awake()
    {
        level = 1;
        attackTargetId = Random.Range(0, 6);
        targetIsShip = true;
        health = 1500;
        maxHP = 1500;
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

        textHP = Instantiate(GameMaster.GM.text3dDamage, transform.position + new Vector3(0, textHpHeight, 0), Quaternion.Euler(0, 0, 0));
        textHP.gameObject.GetComponent<TextMesh>().text = level.ToString();
        textHP.transform.parent = transform;

        healthBarScaleMultiplier = 0.8f;
        healthBar = Instantiate(GameMaster.GM.healthBar, transform.position + new Vector3(0, healthBarHeight, 0), Quaternion.Euler(0, 0, 0));
        healthBar.transform.localScale = new Vector3(health / maxHP * healthBarScaleMultiplier, 0.05f, 1);
        healthBar.transform.SetParent(gameObject.transform);
        pointFromShootingRandomize.Set(Random.Range(-40, 40), 0, Random.Range(-40, 40));
    }

    public void Start()
    {
        randomSpeedDeviation = (float) Random.Range(0, 1000) / 1000 / 8;
        trooperSpeed += randomSpeedDeviation;

        randomCollisionStuckDirection = Random.Range(0, 2);

        if (randomCollisionStuckDirection == 0)
            randomCollisionStuckDirection = -1;
        if (randomCollisionStuckDirection == 1)
            randomCollisionStuckDirection = 1;

        InvokeRepeating("KeepFormation", 0, 0.3f);
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
                    if (targetToChase.GetComponent<Player>() != null)
                    {
                        if (gameObject.name == "LightShip")
                            pointFromShooting = new Vector3(targetToChase.transform.position.x, targetToChase.transform.position.y, targetToChase.transform.position.z);
                        if (gameObject.name == "Trooper")
                            pointFromShooting = new Vector3(targetToChase.transform.position.x, transform.position.y, targetToChase.transform.position.z);
                    }

                    if (targetToChase.GetComponent<Seeker>() != null)
                    {
                        if (gameObject.name == "LightShip")
                            pointFromShooting = new Vector3(targetToChase.transform.position.x, targetToChase.transform.position.y, targetToChase.transform.position.z);
                        if (gameObject.name == "Trooper")
                            pointFromShooting = new Vector3(targetToChase.transform.position.x, transform.position.y, targetToChase.transform.position.z);
                    }  
                }
            }
            else
            {
                if (pointToChase != new Vector3(0, 0, 0))
                {
                    pointFromShooting = pointToChase + pointFromShootingRandomize * 4;
                }                    
                else
                {
                    if (factionId == 0 && GameMaster.GM.aiPlayerBase == false)
                    {
                        if (gameObject.name == "Trooper")
                            pointFromShooting = new Vector3(GameMaster.GM.player.transform.position.x, transform.position.y, GameMaster.GM.player.transform.position.z) + pointFromShootingRandomize * 4;
                        if (gameObject.name == "LightShip")
                            pointFromShooting = new Vector3(GameMaster.GM.player.transform.position.x, GameMaster.GM.player.transform.position.y, GameMaster.GM.player.transform.position.z) + pointFromShootingRandomize * 4;
                    }
                    else
                    {
                        int rnDShip = Random.Range(0, GameMaster.GM.mainBaseCount);

                        while (rnDShip == factionId)
                            rnDShip = Random.Range(0, GameMaster.GM.mainBaseCount);

                        if (GameMaster.GM.shipObjectList[rnDShip] != null)
                            targetToChase = GameMaster.GM.shipObjectList[rnDShip];

                        if (factionId != 0 && GameMaster.GM.aiPlayerBase == false && GameMaster.GM.shipObjectList[rnDShip] != null)
                            targetToChase = GameMaster.GM.shipObjectList[rnDShip];
                    }
                }
            }
           
            if (targetToChase != null && targetToChase.GetComponent<Ship>() != null)
                pointFromShooting = pointFromShooting + pointFromShootingRandomize * 4;

            if (targetToChase != null && targetToChase.GetComponent<Ship>() == null)
                pointFromShooting = pointFromShooting + pointFromShootingRandomize;

            if ((currentWeapon != null && enemyToLook != null) || (currentWeapon != null && enemyToLook != null) )
            {
                if (gameObject.name == "Trooper")
                {
                    body.transform.LookAt(enemyToLook.transform.position);

                    if (rotateBody)
                        body.transform.eulerAngles = body.transform.eulerAngles + new Vector3(0, 0, -90);

                    wait = true;
                }
                else
                {
                    Quaternion lookOnLook = Quaternion.LookRotation(enemyToLook.transform.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 4);

                    if (rotateBody)
                        body.transform.eulerAngles = body.transform.eulerAngles + new Vector3(0, 0, -90);
                   
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
                            
            if ( ((Vector3.Distance(transform.position, pointFromShooting)) > 80) && (wait == false) )
            {
                Quaternion lookOnLook = Quaternion.LookRotation(pointFromShooting - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 4);
                
                rbTrooper.AddRelativeForce(Vector3.forward * trooperSpeed * Time.deltaTime * 40, ForceMode.VelocityChange);

                if (gameObject.GetComponent<Animator>() != null)
                {
                    gameObject.GetComponent<Animator>().Play("Run_Guard");
                    gameObject.GetComponent<Animator>().speed = 1 + randomSpeedDeviation;
                } 
            }
            else
            {
                wait = true;

                if (enemyToLook == false)               
                {
                    Quaternion lookOnLook = Quaternion.LookRotation(Vector3.forward - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 4);
                }

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
                gameObject.GetComponent<Animator>().Play("Idle");

            stopDoing = true;
        }
    }

    public void KeepFormation()
    {
        if (wait == false)
        {
            Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 100);

            foreach (Collider hit in colliders)
            {
                if ((hit.GetComponent<Rigidbody>() != null) && (hit.name != "Rocket") && hit.GetComponent<Player>() == null && hit.GetComponent<Trooper>() != null)
                {
                    if (hit.name == "LightShip")
                        hit.GetComponent<Rigidbody>().AddExplosionForce(70, gameObject.transform.position + new Vector3(0, 0, 0), 0, 1, ForceMode.Force);
                    if (hit.name == "Trooper")
                        hit.GetComponent<Rigidbody>().AddExplosionForce(800, gameObject.transform.position + new Vector3(0, 0, 0), 0, 1, ForceMode.Force);
                }
            }
        }
    }

    public virtual void OnCollisionStay(Collision collisioninfo)
    {
        if (collisioninfo.gameObject.GetComponent<Building>() != null || collisioninfo.gameObject.GetComponent<Tower>() != null || collisioninfo.gameObject.GetComponent<Seeker>() != null)
        {
            rbTrooper.AddRelativeForce(Vector3.left * 300 * Random.Range(-1, 2), ForceMode.Impulse);     
        }
    }
}
