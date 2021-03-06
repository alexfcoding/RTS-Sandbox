﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform objectToStick;
    
    public float damageBullet;
    
    public float timer;
    public float reloadTime, reloadTimeForEnemy, range;
    public float tick;
    public float camX;
    public float camY;
    public float camZ;
    public float ownerLevel;
    float sprayShoot;

    public Vector3 weaponPositionOffset;

    public GameObject rocketLauncherAmmoPrefab;
    public GameObject createdBullet;
    public GameObject bulletEffectAI, bulletEffectPlayer;
    GameObject targetInSphere;
    
    public bool autoTarget;
    public bool reloadNow;
    public bool soundStop;
    public bool isProjectile;
    public bool isBombLauncher;
    public bool isRocket;
    public bool rotatable;
    public bool foundTargetToShoot;
    public bool playerFollowingCommand;
    public bool canBePickedUp;

    public Seeker currentSeeker;
    public Player currentPlayer;

    Vector3 randomShootPosition;

    public virtual void Awake()
    {
        autoTarget = false;
        timer = 0;
        reloadNow = false;
        soundStop = false;
        rotatable = true;

        camX = GameMaster.GM.myCamera.transform.localPosition.x;
        camY = GameMaster.GM.myCamera.transform.localPosition.y;
        camZ = GameMaster.GM.myCamera.transform.localPosition.z;
    }

    public void Start()
    {
        InvokeRepeating("CheckForEnemyInSphere", 0, 1f);

        timer += (float) Random.Range(0, 1000) / 1000;
    }

    public void FixedUpdate()
    {
        if (objectToStick == null)
        {
            transform.Rotate(0, -5, 0);
        }
    }

    public void Update()
    {
        WeaponAction();
    }

    public void WeaponAction()
    {
       if (objectToStick != null)
        {
            ShootTarget();
        }
    }

    public GameObject CheckForEnemyInSphere()
    {
        targetInSphere = null;
        int layerMask = 1 << 8;
        if (gameObject.tag == "EnemyWeapon")
        {
            Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, range, layerMask);
            for (int i = 0; i < colliders.Length; i++)
            {
                if ((colliders[i].GetComponent<FactionIndex>() != null && objectToStick != null && colliders[i].gameObject != null
                && colliders[i].GetComponent<FactionIndex>().factionId != objectToStick.GetComponent<FactionIndex>().factionId
                && currentSeeker.GetComponent<FactionIndex>().dead == false
                && colliders[i].GetComponent<FactionIndex>().isSimpleFollower == false
                && colliders[i].GetComponent<FactionIndex>().dead == false) 
                )
                {
                    foundTargetToShoot = true;
                    targetInSphere = colliders[i].gameObject;
                    if (objectToStick.transform.GetComponent<Trooper>() != null)
                        objectToStick.transform.GetComponent<Trooper>().enemyToLook = targetInSphere.gameObject;
                    break;
                }
            }

            if (foundTargetToShoot == false)
                if (objectToStick.transform.GetComponent<Trooper>() != null)
                { 
                    objectToStick.transform.GetComponent<Trooper>().enemyToLook = null;                    
                }
        }

        return targetInSphere;
    }
  
    public virtual void ShootTarget ()
    {
        if (gameObject.tag == "PlayerWeapon")
        {
            if (Input.GetMouseButton(0) && isProjectile == true && Cursor.visible == false && objectToStick.GetComponent<Player>().tacticMode == false) 
            {
                if (reloadNow == false)
                {
                    if (isRocket == true && gameObject == GameMaster.GM.player.GetComponent<Player>().currentWeapon.gameObject)
                    {
                        GameMaster.GM.shakeCamera = true;
                        reloadNow = true;
                        GameObject CreatedObject = GameMaster.GM.ConstructObject(Player.mainPlayer.currentWeapon.rocketLauncherAmmoPrefab,                        
                            Player.mainPlayer.currentWeapon.transform.TransformPoint(0 + (float) Random.Range(-1, 1) / 6, 0 + (float) Random.Range(-1, 1) / 6, 6), Player.mainPlayer.currentWeapon.transform.rotation,
                        "Rocket", GameMaster.GM.bulletObjectList, true);
                        CreatedObject.GetComponent<RocketShell>().playersBullet = true;
                        CreatedObject.GetComponent<RocketShell>().LaunchSound();
                        CreatedObject.GetComponent<RocketShell>().weaponToStick = gameObject;
                        CreatedObject.gameObject.GetComponent<Rigidbody>().AddRelativeForce(Random.Range(-50, 50), Random.Range(-50, 50), 0, ForceMode.Impulse);
                    }

                    if (isBombLauncher == true && gameObject == GameMaster.GM.player.GetComponent<Player>().currentWeapon.gameObject)
                    {
                        GameMaster.GM.shakeCamera = true;
                        reloadNow = true;
                        GameObject CreatedObject = GameMaster.GM.ConstructObject(Player.mainPlayer.currentWeapon.rocketLauncherAmmoPrefab,
                        Player.mainPlayer.currentWeapon.transform.TransformPoint(0, 0, 1), Player.mainPlayer.currentWeapon.transform.rotation,
                        "Bomb", GameMaster.GM.bulletObjectList);
                        CreatedObject.GetComponent<RocketShell>().playersBullet = true;
                        gameObject.GetComponent<AudioSource>().Play();
                        CreatedObject.GetComponent<RocketShell>().LaunchSound();
                        CreatedObject.GetComponent<RocketShell>().weaponToStick = gameObject;
                        GameMaster.GM.player.gameObject.GetComponent<Rigidbody>().AddRelativeForce(0, -500, -800, ForceMode.Impulse);                        
                    }
                }
            }

            if (Input.GetMouseButton(0) && isProjectile == false && Cursor.visible == false && objectToStick.GetComponent<Player>().tacticMode == false && gameObject == GameMaster.GM.player.GetComponent<Player>().currentWeapon.gameObject) // Если нажали мышь и оружие не стреляет объектами, а пулями 
            {
                GameMaster.GM.myCamera.transform.localPosition = new Vector3(Random.Range(-30, 30), Random.Range(camY - 30, camY + 30), Random.Range(camZ - 30, camZ + 30));

                if (reloadNow == false)
                {
                    GameMaster.GM.myCamera.transform.localPosition = new Vector3(Random.Range(-30, 30), Random.Range(camY - 30, camY + 30), Random.Range(camZ - 30, camZ + 30));
                    reloadNow = true;
                    gameObject.GetComponent<AudioSource>().Play();
                                        
                    RaycastHit hit;

                    Vector3 randomizedSpray = new Vector3(Random.Range(-500, 500) / 120, Random.Range(-500, 500) / 120, Random.Range(-500, 500) / 120);
                                        
                    gameObject.transform.localPosition = randomShootPosition + randomizedSpray * 5;
                    gameObject.transform.GetComponent<ParticleSystem>().Play();
                    
                    if (Physics.Raycast(transform.position + randomizedSpray, transform.forward, out hit, 2000))
                        if (hit.transform.tag != "Player")
                        {
                            GameObject BulletPlayer = Instantiate(bulletEffectPlayer, hit.point + randomizedSpray, Quaternion.LookRotation(hit.normal));
                            BulletPlayer.transform.GetComponent<ParticleSystem>().Play();
                            Destroy(BulletPlayer, 2f);

                            if (hit.rigidbody != null && hit.transform.GetComponent<Seeker>() == null)
                                hit.rigidbody.AddForce(-hit.normal * 500, ForceMode.Impulse);

                            if (hit.transform.GetComponent<FactionIndex>() != null)
                            {
                                hit.transform.GetComponent<FactionIndex>().whoIsDamaging = objectToStick.gameObject;

                                if (hit.transform.tag != "Ship")
                                    hit.transform.GetComponent<FactionIndex>().TakeDamage(damageBullet * 1.5f);
                                else
                                    hit.transform.GetComponent<FactionIndex>().TakeDamage(damageBullet / 10);
                            }

                            if (hit.transform.GetComponent<RocketShell>() != null)
                            {                               
                                hit.transform.GetComponent<RocketShell>().Explode();
                            }

                            if (hit.transform.GetComponent<BombShell>() != null)
                            {                               
                                hit.transform.GetComponent<BombShell>().Explode();
                            }
                        }           
                }
            }

            timer += Time.deltaTime;

            if (timer > tick)
            {
                soundStop = false;
                reloadNow = false;
                tick += reloadTime;
            }

        }

        if (gameObject.tag == "EnemyWeapon" && isProjectile == true)
        {
            if (foundTargetToShoot == true && targetInSphere != null)
            {
                sprayShoot = Random.Range(-10, 10);
                
                if (currentSeeker.tag != "Tower")
                    transform.LookAt(targetInSphere.transform.position);
                else
                {
                    Quaternion lookOnLook = Quaternion.LookRotation(new Vector3(targetInSphere.transform.position.x, targetInSphere.transform.position.y - 10, targetInSphere.transform.position.z) - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 8);
                }
                   
                if (targetInSphere.tag == "Tower")
                {
                    transform.LookAt(targetInSphere.transform.position + new Vector3(0, 30, 0));
                }

                if (targetInSphere.tag == "TowerGun")
                {
                    transform.LookAt(targetInSphere.transform.position + new Vector3(0, 2, 0));
                }

                if (soundStop == false)
                {
                    soundStop = true;
                }

                if (reloadNow == false && isProjectile == true)
                {
                    if (isRocket == true) 
                    { 
                        reloadNow = true;
                        if (currentSeeker.tag != "Tower")
                            createdBullet = GameMaster.GM.ConstructObject(rocketLauncherAmmoPrefab, transform.TransformPoint(Vector3.forward * 1), transform.rotation, "Rocket", GameMaster.GM.bulletObjectList);
                        else
                        {
                            createdBullet = GameMaster.GM.ConstructObject(rocketLauncherAmmoPrefab, transform.TransformPoint(new Vector3(0, 0.3f, 0.7f)) + new Vector3(sprayShoot, sprayShoot, sprayShoot), transform.rotation, "Rocket", GameMaster.GM.bulletObjectList);
                            createdBullet.transform.localScale = new Vector3(20, 20, 20);
                        }
                           

                        createdBullet.GetComponent<RocketShell>().LaunchSound();
                        createdBullet.GetComponent<RocketShell>().weaponToStick = gameObject;
                    }

                    if (isBombLauncher == true)
                    {
                        reloadNow = true;
                        if (currentSeeker.tag != "Tower")
                            createdBullet = GameMaster.GM.ConstructObject(rocketLauncherAmmoPrefab, transform.TransformPoint(Vector3.forward * 1), transform.rotation, "Bomb", GameMaster.GM.bulletObjectList);
                        else
                            createdBullet = GameMaster.GM.ConstructObject(rocketLauncherAmmoPrefab, transform.TransformPoint(new Vector3(0, 0.3f, 0.7f)) + new Vector3(sprayShoot, sprayShoot, sprayShoot), transform.rotation, "Bomb", GameMaster.GM.bulletObjectList);

                        createdBullet.GetComponent<RocketShell>().LaunchSound();
                        createdBullet.GetComponent<RocketShell>().weaponToStick = gameObject;
                    }
                }

                timer += Time.deltaTime;

                if (timer > tick)
                {
                    soundStop = false;
                    reloadNow = false;
                    tick += reloadTimeForEnemy;
                }
            }
            else
                foundTargetToShoot = false;
        }

        if (gameObject.tag == "EnemyWeapon" && isProjectile == false)
        {
            if (foundTargetToShoot == true && targetInSphere != null)
            {
                if (targetInSphere.name == "Trooper")
                {
                    transform.LookAt(targetInSphere.transform.position + new Vector3(0, 12, 0));
                }
                else
                {
                    transform.LookAt(targetInSphere.transform.position + new Vector3(0, 0, 0));
                }
                
                if (objectToStick.transform.GetComponent<Trooper>() != null)
                    objectToStick.transform.GetComponent<Trooper>().enemyToLook = targetInSphere.gameObject;

                if (soundStop == false)
                {
                    soundStop = true;
                }

                if (reloadNow == false && isProjectile == false)
                {
                    reloadNow = true;
                    gameObject.GetComponent<AudioSource>().Play();
                    gameObject.transform.GetComponent<ParticleSystem>().Play();

                    RaycastHit hit2;

                    if (Physics.Raycast(transform.position, transform.forward, out hit2, range, 1 << 8))
                    {
                        if (hit2.transform.GetComponent<ParticleSystem>() != null)
                           hit2.transform.GetComponent<ParticleSystem>().Play();

                        if (hit2.transform.GetComponent<Player>() != null)
                        {
                            GameObject BulletAI = Instantiate(bulletEffectAI, hit2.point, Quaternion.LookRotation(hit2.normal));
                            Destroy(BulletAI, 2f);
                        }

                        if (gameObject != null)
                            if (targetInSphere.transform.GetComponent<FactionIndex>() != null && objectToStick != null && targetInSphere.transform.GetComponent<FactionIndex>().dead == false)
                            {
                                targetInSphere.transform.GetComponent<FactionIndex>().whoIsDamaging = objectToStick.gameObject;
                                targetInSphere.transform.GetComponent<FactionIndex>().TakeDamage(damageBullet);
                            }
                    }
                }

                timer += Time.deltaTime;

                if (timer > tick)
                {
                    soundStop = false;
                    reloadNow = false;
                    tick += reloadTimeForEnemy;
                }
            }
            else
                foundTargetToShoot = false;
        }

    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "Player") && (collision.gameObject.GetComponent<Player>() != null) && canBePickedUp)
        {
            collision.gameObject.GetComponent<Player>().pickup.Play();
            collision.gameObject.GetComponent<Player>().playerWeaponList.Add(this);
            collision.gameObject.GetComponent<Player>().currentWeapon = gameObject.GetComponent<Weapon>();

            Component[] renderer;

            for (int i = 0; i < collision.gameObject.GetComponent<Player>().playerWeaponList.Count; i++)
            {
                if (collision.gameObject.GetComponent<Player>().playerWeaponList[i].GetComponent<MeshRenderer>() != null)
                    collision.gameObject.GetComponent<Player>().playerWeaponList[i].GetComponent<MeshRenderer>().enabled = false;

                if (collision.gameObject.GetComponent<Player>().playerWeaponList[i].GetComponentsInChildren<Renderer>() != null)
                {
                    renderer = collision.gameObject.GetComponent<Player>().playerWeaponList[i].GetComponentsInChildren<Renderer>();

                    for (int j = 0; j < renderer.Length; j++)
                    {
                        if (renderer[j].gameObject.name == "Laser_Gun")
                            renderer[j].GetComponent<Renderer>().enabled = false;
                    }
                }
            }
                   
            if (collision.gameObject.GetComponent<Player>().currentWeapon.GetComponent<MeshRenderer>() != null)
                collision.gameObject.GetComponent<Player>().currentWeapon.GetComponent<MeshRenderer>().enabled = true;

            if (collision.gameObject.GetComponent<Player>().currentWeapon.GetComponentsInChildren<Renderer>() != null)
            {
                renderer = collision.gameObject.GetComponent<Player>().currentWeapon.GetComponentsInChildren<Renderer>();

                for (int j = 0; j < renderer.Length; j++)
                {
                    if (renderer[j].gameObject.name == "Laser_Gun")
                        renderer[j].GetComponent<Renderer>().enabled = true;
                }
            }

            gameObject.GetComponent<Weapon>().currentPlayer = collision.gameObject.GetComponent<Player>();

            if (gameObject.GetComponent<Weapon>().rocketLauncherAmmoPrefab != null)
                gameObject.GetComponent<Weapon>().rocketLauncherAmmoPrefab.GetComponent<RocketShell>().playersBullet = true;

            gameObject.tag = "PlayerWeapon";
            gameObject.transform.SetParent(collision.transform);
            gameObject.transform.localPosition = weaponPositionOffset;
            gameObject.transform.eulerAngles = collision.gameObject.transform.eulerAngles;

            randomShootPosition = gameObject.transform.localPosition;

            if (gameObject.name == "BombLauncher")
            {
                gameObject.transform.localEulerAngles = new Vector3(-15, -5, 0);
            }            

            gameObject.GetComponent<Collider>().enabled = false;
            objectToStick = collision.gameObject.transform;
        }

        if ((collision.gameObject.tag == "Seeker" || collision.gameObject.tag == "Trooper") && (collision.gameObject.GetComponent<Seeker>().alreadyHaveWeapon == false) && canBePickedUp)
        {
            collision.gameObject.GetComponent<Seeker>().alreadyHaveWeapon = true;
            collision.gameObject.GetComponent<Seeker>().currentWeapon = gameObject.GetComponent<Weapon>();
            gameObject.GetComponent<Weapon>().currentSeeker = collision.gameObject.GetComponent<Seeker>();
            gameObject.tag = "EnemyWeapon";
            gameObject.GetComponent<Collider>().enabled = false;
            objectToStick = collision.gameObject.transform;
            weaponPositionOffset.Set(0, 4, 0);
            Destroy(gameObject.GetComponent<Rigidbody>());
            
            if (collision.gameObject.tag == "Trooper")
                gameObject.transform.SetParent(collision.gameObject.GetComponent<Trooper>().body.transform);

            if (collision.gameObject.tag == "Seeker")
                gameObject.transform.SetParent(collision.transform);

            if (collision.gameObject.name == "Seeker")
                transform.localPosition = new Vector3(0, -30f, 0);

            if (collision.gameObject.name == "Trooper")
            { 
                transform.localPosition = new Vector3(-0.015f, 00.001f, 0.04f); ;

                if (transform.GetComponent<MeshRenderer>() != null)
                    transform.GetComponent<MeshRenderer>().enabled = false;

                if (transform.Find("Laser_Gun") != null && transform.Find("Laser_Gun").GetComponent<MeshRenderer>() != null)
                    transform.Find("Laser_Gun").GetComponent<MeshRenderer>().enabled = false;
            }

            if (collision.gameObject.name == "LightShip")
            {
                transform.localPosition = new Vector3(0f, 0f, 10f); ;
            }
        }

        if ((collision.gameObject.tag == "EnemyShip") && (collision.gameObject.GetComponent<Seeker>().alreadyHaveWeapon == false))
        {
            collision.gameObject.GetComponent<Ship>().alreadyHaveWeapon = true;
            collision.gameObject.GetComponent<Ship>().currentWeapon = gameObject.GetComponent<Weapon>();
            gameObject.GetComponent<Weapon>().currentSeeker = collision.gameObject.GetComponent<Seeker>();
            gameObject.tag = "EnemyWeapon";
            gameObject.GetComponent<Collider>().enabled = false;
            objectToStick = collision.gameObject.transform;
            weaponPositionOffset.Set(0, 6, 0);
        }
    }
}
