using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractionIndexClass : MonoBehaviour
{
    public int fractionId;

    public bool isSimpleFollower;
    public bool dead, totallyDead;
    public bool isVulnerable;
    public bool lootAfterDeath;

    public float health;
    public float maxHP;
    public float healthBarScaleMultiplier;
    public float level;
    public GameObject healthBar;
    public GameObject whoIsDamaging;
    public GameObject deathEffect;
    
    public AudioSource deathSound;

    public virtual void Awake()
    {
        level = 1;
        health = 1;
        maxHP = 1;
        healthBarScaleMultiplier = 0;
        healthBar = Instantiate(GameMaster.GM.healthBar, transform.position + new Vector3(0, 12, 0), Quaternion.Euler(0, 0, 0));
        healthBar.transform.SetParent(gameObject.transform);
        healthBar.transform.localScale = new Vector3(health / maxHP * healthBarScaleMultiplier, 0.05f, 1);
    }    

    public void SetFractionId(int fractionId)
    {
        this.fractionId = fractionId;

        if (gameObject.name == "Seeker" || gameObject.name == "GunTower" || gameObject.name == "Tower")
        {
            int lightsCount = 0;

            if (gameObject.GetComponentsInChildren<Light>() != null)
            {
                lightsCount = gameObject.GetComponentsInChildren<Light>().Length;

                for (int i = 0; i < lightsCount; i++)
                {
                    gameObject.GetComponentsInChildren<Light>()[i].color = GameMaster.GM.fractionColors[this.fractionId];
                }
            }
        }

        if (healthBar != null)
            healthBar.GetComponent<SpriteRenderer>().color = GameMaster.GM.fractionColors[this.fractionId];
    }

    public void SetHealth (float healthToSet)
    {
        health = healthToSet;
        maxHP = healthToSet;
    }

    public virtual void TakeDamage(float damage)
    {
        if (isVulnerable == true) 
        { 
            if (health > damage)
            {
                health -= damage;
                if (healthBar != null)
                    healthBar.transform.localScale = new Vector3(health / maxHP * healthBarScaleMultiplier, 0.05f, 1);
            }
            // Обездвижили
            else
            {
                if (dead == false)
                {
                    if (healthBar != null)
                        healthBar.transform.localScale = new Vector3(0, 0, 0);
                    //DeathEffect = Instantiate(GameMaster.GM.SmokeAfterDeath, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
                    //DeathEffect.transform.parent = gameObject.transform;
                    if (deathEffect != null)
                    {
                        GameObject deadBoom = Instantiate(deathEffect, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
                        Destroy(deadBoom, 2f);
                    }
                        

                    if (deathSound != null)
                        deathSound.Play();

                    if (lootAfterDeath == true)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            int RndNum = Random.Range(0, GameMaster.GM.detailsList.Count);
                            GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.detailsList[RndNum], transform.position + new Vector3(Random.Range(0, 4), Random.Range(0, 4), Random.Range(0, 4)), Quaternion.Euler(0, 0, 0), "Follower", GameMaster.GM.globalObjectList);
                            if (createdObject.GetComponent<Rigidbody>() == null)
                                createdObject.AddComponent<Rigidbody>();
                            createdObject.GetComponent<Rigidbody>().mass = 5;
                            if (createdObject.GetComponent<MeshCollider>() != null && createdObject.GetComponent<MeshCollider>().convex == false)
                                createdObject.GetComponent<MeshCollider>().convex = true;
                            //Destroy(createdObject, 10);
                        }
                    }
                }

                dead = true;
                health -= damage;
                GameMaster.GM.RecursiveDestroy(transform, gameObject, 3);
            }
        }
    }
}
