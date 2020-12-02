using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombShellClass : RocketShellClass
{
    bool exploded;
    public override void Awake()
    {
        isProjectile = true;
        playersBullet = false;

        if (gameObject.GetComponent<Rigidbody>() != null)
        {
            gameObject.GetComponent<Rigidbody>().AddRelativeForce(Random.Range(0, 200), Random.Range(0, 10), 1500, ForceMode.Impulse);
            gameObject.GetComponent<Rigidbody>().AddRelativeForce(0, 0, 0, ForceMode.Impulse);            
        }            
    }
    public override void MoveBullet()
    {
        Timer += Time.deltaTime;

        if (playersBullet == true && isHoming == true)
            if (Physics.BoxCast(GameMaster.GM.player.transform.TransformPoint(0, 0, 0), new Vector3(20, 20, 2), transform.forward, out RaycastHit hitInfo2, Quaternion.Euler(0, 0, 0), 400, 1 << 8))
                if ((hitInfo2.transform.tag == "Seeker" || hitInfo2.transform.tag == "Trooper") && (hitInfo2.transform.GetComponent<FractionIndexClass>().fractionId != 0))
                {
                    gameObject.transform.LookAt(hitInfo2.transform.position + new Vector3(0, 1, 0));
                    Vector3 normalizeDirection = (hitInfo2.transform.position + new Vector3(0, 1, 0) - gameObject.transform.position).normalized;
                    gameObject.transform.position += normalizeDirection * Time.deltaTime * 30;
                    //GameObject Explode = Instantiate(GameMaster.GM.EnmemyDestroy, hitInfo2.transform.position, Quaternion.Euler(0, 0, 0));       
                }

        if (Timer > 3 && exploded == false)
        {
            Explode();
        }            
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != null && (collision.gameObject.name != "Bomb" && collision.gameObject.name != "Terrain" && collision.gameObject.GetComponent<FractionIndexClass>() !=null && weaponToStick != null && collision.gameObject.GetComponent<FractionIndexClass>() != weaponToStick.GetComponent<WeaponClass>().objectToStick.GetComponent<FractionIndexClass>()))
            if (playersBullet == false && collision.gameObject.tag == "Player" || playersBullet == true && collision.gameObject.tag != "Player" || playersBullet == false && collision.gameObject.tag != "Player")
            {
                Explode();
            }
    }

    public override void Explode()
    {
        audio2.Play();

        foreach (Transform child in gameObject.transform)
        {
            // if (child.name != "WFXMR_Nuke 1")
            GameObject.Destroy(child.gameObject);
        }

        gameObject.transform.GetComponent<Collider>().enabled = false;
        gameObject.transform.GetComponent<MeshRenderer>().enabled = false;

        GameObject Explosions = Instantiate(explosionRocketPrefab, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(Explosions, 2f);
        //ExplosionParticleSystem.Play();
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 50);

        foreach (Collider hit in colliders)
        {
            if ((hit.GetComponent<Rigidbody>() != null && hit.GetComponent<SeekerClass>() == null && hit.GetComponent<TrooperClass>() == null) && (hit.name != "Rocket"))
                hit.GetComponent<Rigidbody>().AddExplosionForce(800, gameObject.transform.position + new Vector3(0, 0, 0), 30, 1, ForceMode.Impulse);

            if (hit.GetComponent<FractionIndexClass>() != null && hit.gameObject != null)
            {
                if (weaponToStick != null && hit.transform.GetComponent<FractionIndexClass>().dead == false)
                    hit.transform.GetComponent<FractionIndexClass>().whoIsDamaging = weaponToStick.GetComponent<WeaponClass>().objectToStick.gameObject;

                if (weaponToStick != null && weaponToStick.GetComponent<WeaponClass>().objectToStick.gameObject.name == "Player")
                {
                    if (hit.transform.GetComponent<FractionIndexClass>().transform.tag != "Ship")
                        hit.transform.GetComponent<FractionIndexClass>().TakeDamage(damage * 2f);
                    else
                        hit.transform.GetComponent<FractionIndexClass>().TakeDamage(damage / 10);
                  
                    //hit.transform.GetComponent<FractionIndexClass>().TakeDamage(damage * weaponToStick.GetComponent<WeaponClass>().ownerLevel);
                }
                else if (weaponToStick != null)
                {
                    //hit.transform.GetComponent<FractionIndexClass>().TakeDamage(damage * weaponToStick.GetComponent<WeaponClass>().ownerLevel);
                    if (hit.transform.GetComponent<FractionIndexClass>().transform.tag != "Ship")
                        hit.transform.GetComponent<FractionIndexClass>().TakeDamage(damage);
                    else
                        hit.transform.GetComponent<FractionIndexClass>().TakeDamage(damage / 10);
                }
            }

            if (hit.GetComponent<PlayerClass>() != null)
            {
                hit.GetComponent<PlayerClass>().TakeDamage(damage);
            }
        }

        if (gameObject.GetComponent<MeshRenderer>() != null)
            gameObject.GetComponent<MeshRenderer>().enabled = false;

        if (gameObject.GetComponent<Collider>() != null)
            gameObject.GetComponent<Collider>().enabled = false;

        foreach (Transform child in transform)
        {
            if (child.GetComponent<MeshRenderer>() != null)
                child.GetComponent<MeshRenderer>().enabled = false;
            if (child.GetComponent<Collider>() != null)
                child.GetComponent<Collider>().enabled = false;
        }

        Destroy(gameObject, 3);
        exploded = true;
    }
}

