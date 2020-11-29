using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponClass : MonoBehaviour
{
    public Transform objectToStick;
    
    public float damageBullet;
    
    public float timer;
    public float reloadTime, reloadTimeForEnemy, range;
    public float tick;
    public float camX;
    public float camY;
    public float camZ;
    float sprayShoot;
    public Vector3 weaponPositionOffset;
    public float ownerLevel;
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

    public SeekerClass currentSeeker;
    public PlayerClass currentPlayer;
    
    public virtual void Awake()
    {
        autoTarget = false;
        timer = 0;
        reloadNow = false;
        soundStop = false;
        rotatable = true;
    }

    public void Start()
    {
        InvokeRepeating("CheckForEnemyInSphere", 0, 1f);
        camX = GameMaster.GM.myCamera.transform.localPosition.x;
        camY = GameMaster.GM.myCamera.transform.localPosition.y;
        camZ = GameMaster.GM.myCamera.transform.localPosition.z;

        timer += (float) Random.Range(0, 1000) / 1000;
    }

    public void FixedUpdate()
    {
        if (objectToStick == null)
        {
            transform.Rotate(0, -5, 0);
        }

        
        //if (currentSeeker != null && currentSeeker.tag == "Tower")
        //{

        //    if (foundTargetToShoot == false)
        //    {
        //        gameObject.transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        //        transform.Rotate(0, -1, 0);
        //    }

        //}
    }

    public void Update()
    {
        WeaponAction();
        //if (gameObject.tag == "PlayerWeapon")
        //{
        //    GameObject LockedTarget = GameMaster.GM.player.GetComponent<PlayerClass>().targetToLock;

        //    if (GameMaster.GM.player.GetComponent<PlayerClass>().targetLockedIn == true)
        //    {
        //        if (LockedTarget.gameObject != null && LockedTarget.GetComponent<SeekerClass>() != null)
        //        {
        //            if (LockedTarget.gameObject.name == "Trooper")
        //                gameObject.transform.LookAt(LockedTarget.transform.position + new Vector3(0, 4, 0));
        //            if (LockedTarget.gameObject.name == "Seeker")
        //                gameObject.transform.LookAt(LockedTarget.transform.position + new Vector3(0, 2, 0));
        //            if (LockedTarget.gameObject.name == "LightShip")
        //                gameObject.transform.LookAt(LockedTarget.transform.position + new Vector3(0, 0, 0));
        //        }
        //    }
        //    else
        //        gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        //}

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
                if ((colliders[i].GetComponent<FractionIndexClass>() != null && objectToStick != null && colliders[i].gameObject != null
                && colliders[i].GetComponent<FractionIndexClass>().fractionId != objectToStick.GetComponent<FractionIndexClass>().fractionId
                && currentSeeker.GetComponent<FractionIndexClass>().dead == false
                && colliders[i].GetComponent<FractionIndexClass>().isSimpleFollower == false
                && colliders[i].GetComponent<FractionIndexClass>().dead == false) 
                 
                //(
                //colliders[i].GetComponent<FractionIndexClass>() != null && objectToStick != null && colliders[i].gameObject != null
                //&& colliders[i].GetComponent<FractionIndexClass>().fractionId != objectToStick.GetComponent<FractionIndexClass>().fractionId
                //&& currentSeeker.GetComponent<FractionIndexClass>().dead == false
                //&& colliders[i].GetComponent<FractionIndexClass>().isSimpleFollower == false
                //&& colliders[i].GetComponent<FractionIndexClass>().dead == false && objectToStick.transform.GetComponent<TrooperClass>() != null 
                //&& objectToStick.GetComponent<FractionIndexClass>().fractionId == 0)

                /*&& colliders[i].gameObject == objectToStick.transform.GetComponent<TrooperClass>().targetToChaseByPlayerCommand*/)

                {
                    foundTargetToShoot = true;
                    targetInSphere = colliders[i].gameObject;
                    if (objectToStick.transform.GetComponent<TrooperClass>() != null)
                        objectToStick.transform.GetComponent<TrooperClass>().enemyToLook = targetInSphere.gameObject;
                    break;
                }
            }

            if (foundTargetToShoot == false)
                if (objectToStick.transform.GetComponent<TrooperClass>() != null)
                { 
                    objectToStick.transform.GetComponent<TrooperClass>().enemyToLook = null;
                    
                }
        }

        return targetInSphere;
    }
  
    public virtual void ShootTarget ()
    {
        if (gameObject.tag == "PlayerWeapon")
        {
            if (Input.GetMouseButton(0) && isProjectile == true && Cursor.visible == false && objectToStick.GetComponent<PlayerClass>().tacticMode == false) 
            {
                if (reloadNow == false)
                {
                    if (isRocket == true && gameObject == GameMaster.GM.player.GetComponent<PlayerClass>().currentWeapon.gameObject)
                    {
                        GameMaster.GM.shakeCamera = true;
                        reloadNow = true;
                        GameObject CreatedObject = GameMaster.GM.ConstructObject(PlayerClass.mainPlayer.currentWeapon.rocketLauncherAmmoPrefab,
                        PlayerClass.mainPlayer.currentWeapon.transform.TransformPoint(0 + (float) Random.Range(-3,3) / 6, 0 + (float) Random.Range(-3, 3) / 6, 4), PlayerClass.mainPlayer.currentWeapon.transform.rotation,
                        "Rocket", GameMaster.GM.bulletObjectList, true);
                        CreatedObject.GetComponent<RocketShellClass>().playersBullet = true;
                        CreatedObject.GetComponent<RocketShellClass>().LaunchSound();
                        CreatedObject.GetComponent<RocketShellClass>().weaponToStick = gameObject;
                    }

                    if (isBombLauncher == true && gameObject == GameMaster.GM.player.GetComponent<PlayerClass>().currentWeapon.gameObject)
                    {
                        GameMaster.GM.shakeCamera = true;
                        reloadNow = true;
                        GameObject CreatedObject = GameMaster.GM.ConstructObject(PlayerClass.mainPlayer.currentWeapon.rocketLauncherAmmoPrefab,
                        PlayerClass.mainPlayer.currentWeapon.transform.TransformPoint(0, 0, 1), PlayerClass.mainPlayer.currentWeapon.transform.rotation,
                        "Bomb", GameMaster.GM.bulletObjectList);
                        CreatedObject.GetComponent<RocketShellClass>().playersBullet = true;
                        gameObject.GetComponent<AudioSource>().Play();
                        CreatedObject.GetComponent<RocketShellClass>().LaunchSound();
                        CreatedObject.GetComponent<RocketShellClass>().weaponToStick = gameObject;
                        GameMaster.GM.player.gameObject.GetComponent<Rigidbody>().AddRelativeForce(0, -500, -800, ForceMode.Impulse);                        
                    }
                }
            }

            if (Input.GetMouseButton(0) && isProjectile == false && Cursor.visible == false && objectToStick.GetComponent<PlayerClass>().tacticMode == false && gameObject == GameMaster.GM.player.GetComponent<PlayerClass>().currentWeapon.gameObject) // Если нажали мышь и оружие не стреляет объектами, а пулями 
            {
                GameMaster.GM.myCamera.transform.localPosition = new Vector3(Random.Range(-30, 30), Random.Range(camY - 30, camY + 30), Random.Range(camZ - 30, camZ + 30));

                if (reloadNow == false)
                {
                    GameMaster.GM.myCamera.transform.localPosition = new Vector3(Random.Range(-30, 30), Random.Range(camY - 30, camY + 30), Random.Range(camZ - 30, camZ + 30));
                    reloadNow = true;
                    gameObject.GetComponent<AudioSource>().Play();
                    gameObject.transform.GetComponent<ParticleSystem>().Play();
                    RaycastHit hit;

                    if (Physics.Raycast(transform.position, transform.forward, out hit, 600))
                        if (hit.transform.tag != "Player")
                        {
                            GameObject BulletPlayer = Instantiate(bulletEffectPlayer, hit.point, Quaternion.LookRotation(hit.normal));
                            Destroy(BulletPlayer, 2f);

                            if (hit.rigidbody != null && hit.transform.GetComponent<SeekerClass>() == null)
                                hit.rigidbody.AddForce(-hit.normal * 500, ForceMode.Impulse);

                            if (hit.transform.GetComponent<FractionIndexClass>() != null)
                            {
                                hit.transform.GetComponent<FractionIndexClass>().whoIsDamaging = objectToStick.gameObject;
                                //ownerLevel = currentPlayer.transform.GetComponent<FractionIndexClass>().level;
                                // hit.transform.GetComponent<FractionIndexClass>().TakeDamage(damageBullet * ownerLevel);
                                if (hit.transform.tag != "Ship")
                                    hit.transform.GetComponent<FractionIndexClass>().TakeDamage(damageBullet * 2f);
                                else
                                    hit.transform.GetComponent<FractionIndexClass>().TakeDamage(damageBullet / 10);
                            }

                            if (hit.transform.GetComponent<RocketShellClass>() != null)
                            {
                                //hit.transform.GetComponent<RocketShellClass>().damagedByBullet = true;
                                hit.transform.GetComponent<RocketShellClass>().Explode();
                            }

                            if (hit.transform.GetComponent<BombShellClass>() != null)
                            {                               
                                hit.transform.GetComponent<BombShellClass>().Explode();
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

                    //transform.LookAt(targetInSphere.transform.position + new Vector3(0, -10, 0));
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
                           

                        createdBullet.GetComponent<RocketShellClass>().LaunchSound();
                        createdBullet.GetComponent<RocketShellClass>().weaponToStick = gameObject;
                    }

                    if (isBombLauncher == true)
                    {
                        reloadNow = true;
                        if (currentSeeker.tag != "Tower")
                            createdBullet = GameMaster.GM.ConstructObject(rocketLauncherAmmoPrefab, transform.TransformPoint(Vector3.forward * 1), transform.rotation, "Bomb", GameMaster.GM.bulletObjectList);
                        else
                            createdBullet = GameMaster.GM.ConstructObject(rocketLauncherAmmoPrefab, transform.TransformPoint(new Vector3(0, 0.3f, 0.7f)) + new Vector3(sprayShoot, sprayShoot, sprayShoot), transform.rotation, "Bomb", GameMaster.GM.bulletObjectList);

                        createdBullet.GetComponent<RocketShellClass>().LaunchSound();
                        createdBullet.GetComponent<RocketShellClass>().weaponToStick = gameObject;
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
                transform.LookAt(targetInSphere.transform.position + new Vector3(0,0,0));

                if (objectToStick.transform.GetComponent<TrooperClass>() != null)
                    objectToStick.transform.GetComponent<TrooperClass>().enemyToLook = targetInSphere.gameObject;

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

                        if (hit2.transform.GetComponent<PlayerClass>() != null)
                        {
                            GameObject BulletAI = Instantiate(bulletEffectAI, hit2.point, Quaternion.LookRotation(hit2.normal));
                            Destroy(BulletAI, 2f);
                        }

                        if (gameObject != null)
                            if (targetInSphere.transform.GetComponent<FractionIndexClass>() != null && objectToStick != null && targetInSphere.transform.GetComponent<FractionIndexClass>().dead == false)
                            {
                                targetInSphere.transform.GetComponent<FractionIndexClass>().whoIsDamaging = objectToStick.gameObject;
                                //ownerLevel = currentSeeker.transform.GetComponent<FractionIndexClass>().level;
                                //targetInSphere.transform.GetComponent<FractionIndexClass>().TakeDamage(damageBullet * ownerLevel);
                                targetInSphere.transform.GetComponent<FractionIndexClass>().TakeDamage(damageBullet);
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
        if ((collision.gameObject.tag == "Player") && (collision.gameObject.GetComponent<PlayerClass>() != null) && canBePickedUp)
        {
           // collision.gameObject.GetComponent<PlayerClass>().alreadyHaveWeapon = true;
            collision.gameObject.GetComponent<PlayerClass>().pickup.Play();
            collision.gameObject.GetComponent<PlayerClass>().playerWeaponList.Add(this);
            collision.gameObject.GetComponent<PlayerClass>().currentWeapon = gameObject.GetComponent<WeaponClass>();

            Component[] renderer;

            for (int i = 0; i < collision.gameObject.GetComponent<PlayerClass>().playerWeaponList.Count; i++)
            {
                if (collision.gameObject.GetComponent<PlayerClass>().playerWeaponList[i].GetComponent<MeshRenderer>() != null)
                    collision.gameObject.GetComponent<PlayerClass>().playerWeaponList[i].GetComponent<MeshRenderer>().enabled = false;

                if (collision.gameObject.GetComponent<PlayerClass>().playerWeaponList[i].GetComponentsInChildren<Renderer>() != null)
                {
                    renderer = collision.gameObject.GetComponent<PlayerClass>().playerWeaponList[i].GetComponentsInChildren<Renderer>();

                    for (int j = 0; j < renderer.Length; j++)
                    {
                        if (renderer[j].gameObject.name == "Laser_Gun")
                            renderer[j].GetComponent<Renderer>().enabled = false;
                    }
                }
            }
                   
            if (collision.gameObject.GetComponent<PlayerClass>().currentWeapon.GetComponent<MeshRenderer>() != null)
                collision.gameObject.GetComponent<PlayerClass>().currentWeapon.GetComponent<MeshRenderer>().enabled = true;

            if (collision.gameObject.GetComponent<PlayerClass>().currentWeapon.GetComponentsInChildren<Renderer>() != null)
            {
                renderer = collision.gameObject.GetComponent<PlayerClass>().currentWeapon.GetComponentsInChildren<Renderer>();

                for (int j = 0; j < renderer.Length; j++)
                {
                    if (renderer[j].gameObject.name == "Laser_Gun")
                        renderer[j].GetComponent<Renderer>().enabled = true;
                }
            }

            gameObject.GetComponent<WeaponClass>().currentPlayer = collision.gameObject.GetComponent<PlayerClass>();

            if (gameObject.GetComponent<WeaponClass>().rocketLauncherAmmoPrefab != null)
                gameObject.GetComponent<WeaponClass>().rocketLauncherAmmoPrefab.GetComponent<RocketShellClass>().playersBullet = true;

            gameObject.tag = "PlayerWeapon";
            gameObject.transform.SetParent(collision.transform);
            gameObject.transform.localPosition = weaponPositionOffset;
            gameObject.transform.eulerAngles = collision.gameObject.transform.eulerAngles;

            if (gameObject.name == "BombLauncher")
            {
                gameObject.transform.localEulerAngles = new Vector3(-15, -5, 0);
            }            

            gameObject.GetComponent<Collider>().enabled = false;
            objectToStick = collision.gameObject.transform;
            //ownerLevel = objectToStick.gameObject.GetComponent<FractionIndexClass>().level;
        }

        if ((collision.gameObject.tag == "Seeker" || collision.gameObject.tag == "Trooper") && (collision.gameObject.GetComponent<SeekerClass>().alreadyHaveWeapon == false) && canBePickedUp)
        {
            collision.gameObject.GetComponent<SeekerClass>().alreadyHaveWeapon = true;
            collision.gameObject.GetComponent<SeekerClass>().currentWeapon = gameObject.GetComponent<WeaponClass>();
            gameObject.GetComponent<WeaponClass>().currentSeeker = collision.gameObject.GetComponent<SeekerClass>();
            gameObject.tag = "EnemyWeapon";
            gameObject.GetComponent<Collider>().enabled = false;
            objectToStick = collision.gameObject.transform;
            weaponPositionOffset.Set(0, 4, 0);
            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.transform.SetParent(collision.transform);

            if (collision.gameObject.name == "Seeker")
                transform.localPosition = new Vector3(0, -30f, 0);

            if (collision.gameObject.name == "Trooper")
            { 
                transform.localPosition = new Vector3(0.1f, 4f, 5f); ;

                if (transform.GetComponent<MeshRenderer>() != null)
                    transform.GetComponent<MeshRenderer>().enabled = false;

                if (transform.Find("Laser_Gun") != null && transform.Find("Laser_Gun").GetComponent<MeshRenderer>() != null)
                    transform.Find("Laser_Gun").GetComponent<MeshRenderer>().enabled = false;
            }

            if (collision.gameObject.name == "LightShip")
            {
                transform.localPosition = new Vector3(0f, 0f, 10f); ;
            }

            //ownerLevel = objectToStick.gameObject.GetComponent<FractionIndexClass>().level;
        }

        if ((collision.gameObject.tag == "EnemyShip") && (collision.gameObject.GetComponent<SeekerClass>().alreadyHaveWeapon == false))
        {
            collision.gameObject.GetComponent<ShipClass>().alreadyHaveWeapon = true;
            collision.gameObject.GetComponent<ShipClass>().currentWeapon = gameObject.GetComponent<WeaponClass>();
            gameObject.GetComponent<WeaponClass>().currentSeeker = collision.gameObject.GetComponent<SeekerClass>();
            gameObject.tag = "EnemyWeapon";
            gameObject.GetComponent<Collider>().enabled = false;
            objectToStick = collision.gameObject.transform;
            weaponPositionOffset.Set(0, 6, 0);
        }
    }
}
