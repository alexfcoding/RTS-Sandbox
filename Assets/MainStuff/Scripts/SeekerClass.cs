using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerClass : FractionIndexClass
{
    public float minDistance;
    public int minDistNum;
    public int countOfItemsCollected;

    public Vector3 currentTarget;
    
    public bool foundObject;
    public bool findNextObject = true;
    public bool goingToBase;
    public bool alreadyHaveWeapon;
    public bool isStationary;

    public GameObject textHP;
    public GameObject smoke;
    public GameObject currentTargetObject;
    public WeaponClass currentWeapon;

    public float timer2;

    public override void Awake()
    {
        timer2 = Random.Range(0, 100);
        level = 1;
        gameObject.tag = "Seeker";
        minDistance = 1000;
        minDistNum = 0;
        countOfItemsCollected = 0;
        alreadyHaveWeapon = false;
        health = 100000;
        maxHP = 100000;
        dead = false;
        foundObject = false;
        isVulnerable = true;

        textHP = Instantiate(GameMaster.GM.text3dDamage, transform.position + new Vector3(0, 18, 0), Quaternion.Euler(0, 0, 0));
        textHP.gameObject.GetComponent<TextMesh>().text = level.ToString();
        textHP.transform.parent = transform;

        healthBarScaleMultiplier = 10;
        healthBar = Instantiate(GameMaster.GM.healthBar, transform.position + new Vector3(0, 12, 0), Quaternion.Euler(0, 0, 0));
        healthBar.transform.SetParent(gameObject.transform);
        healthBar.transform.localScale = new Vector3(health/maxHP * healthBarScaleMultiplier, 0.05f, 1);
    }

    public virtual void Update()
    {
        FindItemsAround(GameMaster.GM.globalObjectList, GameMaster.GM.platformObjectList);
    }

    public override void TakeDamage(float damage)
    {
        if (isVulnerable == true)
        {
            if (health > damage)
            {
                if (whoIsDamaging != null && whoIsDamaging.GetComponent<FractionIndexClass>().fractionId != GetComponent<FractionIndexClass>().fractionId)
                    health -= damage;

                textHP.gameObject.GetComponent<TextMesh>().text = level.ToString();

                if (healthBar != null)
                    healthBar.transform.localScale = new Vector3(health / maxHP * healthBarScaleMultiplier, 0.02f, 1);
            }
            // Обездвижили
            else
            {
                if (dead == false)
                {
                    if (healthBar != null)
                        healthBar.transform.localScale = new Vector3(0, 0, 0);
                    //smoke = Instantiate(GameMaster.GM.smokeAfterDeath, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
                    //smoke.transform.parent = gameObject.transform;
                    //Destroy(smoke, 2f);

                    if (gameObject.GetComponent<TrooperClass>() == null && gameObject.GetComponent<Rigidbody>() != null)
                        gameObject.GetComponent<Rigidbody>().AddForce(0, 800, 0, ForceMode.Impulse);

                    StartCoroutine(Dying());
                }

                dead = true;
                health -= damage;
            }
        }
        
    }

    public void Heal(float healpoints)
    {
        health += healpoints;
        healthBar.transform.localScale = new Vector3(health / maxHP * healthBarScaleMultiplier, 0.05f, 1);

        //if (textHP != null)
        //{
        //    textHP.gameObject.GetComponent<TextMesh>().text = GetComponent<SeekerClass>().health.ToString();
        //}
    }

    public void SetHealth(float inputHealth)
    {
        health = inputHealth;
        //TextHP.gameObject.GetComponent<TextMesh>().text = Health.ToString();
    }

    public virtual void FindItemsAround(List<GameObject> objectList, List<GameObject> platformList)
    {
        RaycastHit GroundHit;
        Physics.Raycast(transform.position, -transform.up, out GroundHit, 10);
        
        transform.position = new Vector3(transform.position.x, 20, transform.position.z);

        //timer2 += Time.deltaTime;
        ////Vector3 MoveSin = new Vector3(transform.position.x + Mathf.Sin(Timer) * 0.1f,
        ////  Terrain.activeTerrain.SampleHeight(transform.position) + Mathf.Sin(Timer * 4f + randomY) * 1f + 15f,

        //Vector3 moveSin = new Vector3(transform.position.x + Mathf.Sin(timer2) * 0.1f,
        //    transform.position.y + Mathf.Sin(timer2 * 2f) * 0.1f,
        //        transform.position.z + Mathf.Sin(timer2 * 2f) * 0.1f);

        //gameObject.GetComponent<Rigidbody>().MovePosition(moveSin);
        //if (transform.position.y < 20)
        //{
        //    gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        //    gameObject.GetComponent<Rigidbody>().MovePosition(new Vector3(transform.position.x, 20, transform.position.z));
        //}

        healthBar.transform.LookAt(Camera.main.transform.position, -Vector3.up);

        if (objectList.Count > 0 && dead == false)
        {
            if (findNextObject == true)
            {
                minDistNum = Random.Range(0, objectList.Count);
            }

            if (objectList[minDistNum] == null || objectList[minDistNum].GetComponent<FractionIndexClass>().fractionId != 0 && objectList[minDistNum].GetComponent<FractionIndexClass>().fractionId != fractionId)
            {
                findNextObject = true;
            }

            if (objectList[minDistNum]!=null)
            {
                if ((objectList[minDistNum].gameObject.tag == "Follower" && objectList[minDistNum].GetComponent<Follower>().followOwner == false) || (goingToBase == false && objectList[minDistNum].gameObject.name == "Roller" && objectList[minDistNum].gameObject.GetComponent<RollerEnemyBase>() != null))
                    foundObject = true;
                else
                    foundObject = false;

                if (objectList[minDistNum].gameObject.tag == "Follower" && findNextObject == false && objectList[minDistNum].GetComponent<Follower>().followOwner == true)
                    findNextObject = true;

                if (foundObject == true && goingToBase == false)
                {
                    findNextObject = false;

                    if(objectList[minDistNum].gameObject != null)
                    {
                        currentTarget = objectList[minDistNum].transform.position;
                        currentTargetObject = objectList[minDistNum].gameObject;
                    }                       
                    else
                        minDistNum = Random.Range(0, objectList.Count);
                    
                   
                    Quaternion lookOnLook = Quaternion.LookRotation(new Vector3(currentTarget.x, transform.position.y, currentTarget.z) - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 3f);
                    Vector3 normalizeDirection = (objectList[minDistNum].gameObject.GetComponent<Transform>().transform.position - gameObject.transform.position).normalized;
                    gameObject.GetComponent<Transform>().transform.position += normalizeDirection * Time.deltaTime * 70;
                }
            }

            if (goingToBase == true && platformList[fractionId].gameObject != null)
            {
                currentTarget = new Vector3(platformList[fractionId].transform.position.x, transform.position.y, platformList[fractionId].transform.position.z);
                //transform.LookAt(currentTarget);

                Quaternion lookOnLook = Quaternion.LookRotation(new Vector3(currentTarget.x, transform.position.y, currentTarget.z) - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 3f);
                Vector3 normalizeDirection = (platformList[fractionId].gameObject.GetComponent<Transform>().transform.position - gameObject.transform.position).normalized;
                gameObject.GetComponent<Transform>().transform.position += normalizeDirection * Time.deltaTime * 70;
            }

            if (countOfItemsCollected >= 8)
                goingToBase = true;
            else if (countOfItemsCollected == 0)
                goingToBase = false;

            Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 50, 1 << 9);

            foreach (Collider hit in colliders)
                if (gameObject.tag == "Seeker" && hit.tag == "Follower" && dead == false)
                {
                    Vector3 normalizeDirection = (gameObject.transform.position - hit.transform.position).normalized;
                    hit.transform.position += normalizeDirection * Time.deltaTime * hit.GetComponent<Follower>().speed;
                }

            if (findNextObject == false && foundObject == false)
            {
                findNextObject = true;
            }
        }
    }

    IEnumerator Dying()
    {
        yield return new WaitForSeconds(0f);

        if (whoIsDamaging != null)
            if (whoIsDamaging.GetComponent<FractionIndexClass>().level < 5)
            {
                whoIsDamaging.GetComponent<FractionIndexClass>().level += 1;

                if (whoIsDamaging.GetComponent<SeekerClass>() != null)
                    whoIsDamaging.GetComponent<SeekerClass>().textHP.gameObject.GetComponent<TextMesh>().text = level.ToString();
            }
                
        if (whoIsDamaging != null && (whoIsDamaging.tag == "Trooper" || whoIsDamaging.tag == "Seeker"))
        {
            whoIsDamaging.GetComponent<FractionIndexClass>().health += 1000;
            maxHP += 1000;
        }
           

        totallyDead = true;

        if (GetComponent<AudioSource>() != null)
            GetComponent<AudioSource>().Play();

        GameObject Explode = Instantiate(deathEffect, gameObject.transform.position, Quaternion.Euler(0, 0, 0));

        Destroy(Explode, 2f);

        if (lootAfterDeath == true && whoIsDamaging.gameObject!= null && whoIsDamaging.GetComponent<PlayerClass>() != null)
        {
            int rndLootCount = Random.Range(1, 3 * (int) gameObject.GetComponent<FractionIndexClass>().level);

            for (int i = 0; i < rndLootCount; i++)
            {
                int rndNum = Random.Range(0, GameMaster.GM.detailsList.Count);

                GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.detailsList[rndNum], transform.position + new Vector3(Random.Range(0, 4), Random.Range(0, 4), Random.Range(0, 4)), Quaternion.Euler(0, 0, 0), "Follower", GameMaster.GM.globalObjectList);

                if (createdObject.GetComponent<Rigidbody>() == null)
                {
                    createdObject.AddComponent<Rigidbody>();
                }

                createdObject.GetComponent<Rigidbody>().mass = 5;
                //createdObject.GetComponent<Rigidbody>().useGravity = false;
                createdObject.GetComponent<Rigidbody>().angularDrag = 0.05f;
                createdObject.GetComponent<Rigidbody>().drag = 1f;
                if (createdObject.GetComponent<MeshCollider>() != null && createdObject.GetComponent<MeshCollider>().convex == false)
                    createdObject.GetComponent<MeshCollider>().convex = true;

                //createdObject.GetComponent<Follower>().ownerToFollow = gameObject;
                //createdObject.GetComponent<Follower>().followOwner = true;
                createdObject.GetComponent<Follower>().moveToNextOwner = true;
                //Destroy(createdObject, 15 + i);
            }
        }

        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 50);

        foreach (Collider hit in colliders)
        {
            if ((hit.GetComponent<Rigidbody>() != null) && (hit.tag != "Rocket"))
                hit.GetComponent<Rigidbody>().AddExplosionForce(1, gameObject.transform.position + new Vector3(0, 0, 0), 150, 1, ForceMode.Impulse);
        }
        
        GameMaster.GM.RecursiveDestroy(transform, gameObject, 1);

        for (int i = 0; i < 3; i++)
        {
            //Details = GameMaster.GM.ConstructObject(GameMaster.GM.EnemyDetailsPrefab, gameObject.transform.position + new Vector3(Random.Range(-7, 7), Random.Range(-7, 7), Random.Range(-7, 7)), Quaternion.Euler(0, 0, 0), "Parts", GameMaster.GM.GlobalObjectList);
            //Details.gameObject.tag = "Follower";
        }
    }
}
