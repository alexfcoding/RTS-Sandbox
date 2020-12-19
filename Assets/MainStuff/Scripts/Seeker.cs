using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeker : FactionIndex
{
    public float minDistance;
    public int minDistNum;
    public int countOfItemsCollected;
    public int lootMinCount;

    public Vector3 currentTarget;
    
    public bool foundObject;
    public bool findNextObject = true;
    public bool goingToBase;
    public bool alreadyHaveWeapon;
    public bool isStationary;

    public GameObject textHP;
    public GameObject smoke;
    public GameObject currentTargetObject;
    public Weapon currentWeapon;

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

        textHP = Instantiate(GameMaster.GM.text3dDamage, transform.position + new Vector3(0, textHpHeight, 0), Quaternion.Euler(0, 0, 0));
        textHP.gameObject.GetComponent<TextMesh>().text = level.ToString();
        textHP.transform.parent = transform;

        healthBarScaleMultiplier = 10;
        healthBar = Instantiate(GameMaster.GM.healthBar, transform.position + new Vector3(0, healthBarHeight, 0), Quaternion.Euler(0, 0, 0));
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
                if (whoIsDamaging != null && whoIsDamaging.GetComponent<FactionIndex>().factionId != GetComponent<FactionIndex>().factionId)
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
                    
                    if (gameObject.GetComponent<Trooper>() == null && gameObject.GetComponent<Rigidbody>() != null)
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
    }

    public void SetHealth(float inputHealth)
    {
        health = inputHealth;
    }

    public virtual void FindItemsAround(List<GameObject> objectList, List<GameObject> platformList)
    {
        RaycastHit GroundHit;
        Physics.Raycast(transform.position, -transform.up, out GroundHit, 10);
        
        transform.position = new Vector3(transform.position.x, 20, transform.position.z);

        healthBar.transform.LookAt(Camera.main.transform.position, -Vector3.up);

        if (objectList.Count > 0 && dead == false)
        {
            if (findNextObject == true)
            {
                minDistNum = Random.Range(0, objectList.Count);
            }

            if (objectList[minDistNum] == null || objectList[minDistNum].GetComponent<FactionIndex>().factionId != 0 && objectList[minDistNum].GetComponent<FactionIndex>().factionId != factionId || currentTargetObject.gameObject == null)
            {
                findNextObject = true;
            }

            if (objectList[minDistNum] != null)
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

            if (goingToBase == true && platformList[factionId].gameObject != null)
            {
                currentTarget = new Vector3(platformList[factionId].transform.position.x, transform.position.y, platformList[factionId].transform.position.z);

                Quaternion lookOnLook = Quaternion.LookRotation(new Vector3(currentTarget.x, transform.position.y, currentTarget.z) - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 3f);
                Vector3 normalizeDirection = (platformList[factionId].gameObject.GetComponent<Transform>().transform.position - gameObject.transform.position).normalized;
                gameObject.GetComponent<Transform>().transform.position += normalizeDirection * Time.deltaTime * 70;
            }

            if (countOfItemsCollected >= 8)
                goingToBase = true;
            else if (countOfItemsCollected == 0)
                goingToBase = false;
            else if (countOfItemsCollected != 0 && currentTargetObject == null)
                goingToBase = false;

            Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 80, 1 << 9);

            foreach (Collider hit in colliders)
                if (gameObject.tag == "Seeker" && hit.tag == "Follower" && dead == false)
                {
                    Vector3 normalizeDirection = (gameObject.transform.position - hit.transform.position).normalized;
                    hit.transform.position += normalizeDirection * Time.deltaTime * hit.GetComponent<Follower>().speed * 2;
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
            if (whoIsDamaging.GetComponent<FactionIndex>().level < 5)
            {
                whoIsDamaging.GetComponent<FactionIndex>().level += 1;

                if (whoIsDamaging.GetComponent<Seeker>() != null)
                    whoIsDamaging.GetComponent<Seeker>().textHP.gameObject.GetComponent<TextMesh>().text = level.ToString();
            }
                
        if (whoIsDamaging != null && (whoIsDamaging.tag == "Trooper" || whoIsDamaging.tag == "Seeker"))
        {
            whoIsDamaging.GetComponent<FactionIndex>().health += 1000;
            whoIsDamaging.GetComponent<FactionIndex>().maxHP += 1000;
        }

        if (whoIsDamaging != null && whoIsDamaging.tag == "Player")
        {
            whoIsDamaging.GetComponent<FactionIndex>().health += 200;
            whoIsDamaging.GetComponent<FactionIndex>().maxHP += 1000;
            GameMaster.GM.player.GetComponent<Player>().playerHealth3dText.text = $"HP: {GameMaster.GM.player.GetComponent<Player>().health}"; 
        }

        totallyDead = true;

        if (GetComponent<AudioSource>() != null)
            GetComponent<AudioSource>().Play();

        GameObject Explode = Instantiate(deathEffect, gameObject.transform.position, Quaternion.Euler(0, 0, 0));

        Destroy(Explode, 2f);

        if (lootAfterDeath == true && whoIsDamaging.gameObject!= null && whoIsDamaging.GetComponent<Player>() != null)
        {
            int rndLootCount = Random.Range(lootMinCount, lootMinCount + (int) gameObject.GetComponent<FactionIndex>().level);

            for (int i = 0; i < rndLootCount; i++)
            {
                int rndNum = Random.Range(0, GameMaster.GM.detailsList.Count);

                GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.detailsList[rndNum], transform.position + new Vector3(Random.Range(-4, 4), Random.Range(0, 7), Random.Range(-4, 4)), Quaternion.Euler(0, 0, 0), "Follower", GameMaster.GM.globalObjectList);

                if (createdObject.GetComponent<Rigidbody>() == null)
                {
                    createdObject.AddComponent<Rigidbody>();
                }

                createdObject.GetComponent<Rigidbody>().mass = 5;
                createdObject.GetComponent<Rigidbody>().useGravity = true;
                createdObject.GetComponent<Rigidbody>().angularDrag = 0.05f;
                createdObject.GetComponent<Rigidbody>().drag = 1f;
                
                if (createdObject.GetComponent<MeshCollider>() != null && createdObject.GetComponent<MeshCollider>().convex == false)
                    createdObject.GetComponent<MeshCollider>().convex = true;

                createdObject.GetComponent<Follower>().moveToNextOwner = true;
            }
        }

        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 50);

        foreach (Collider hit in colliders)
        {
            if ((hit.GetComponent<Rigidbody>() != null) && (hit.tag != "Rocket"))
                hit.GetComponent<Rigidbody>().AddExplosionForce(300, gameObject.transform.position, 100, Random.Range(0, 1), ForceMode.Impulse);
        }
        
        GameMaster.GM.RecursiveDestroy(transform, gameObject, 4);
    }
}
