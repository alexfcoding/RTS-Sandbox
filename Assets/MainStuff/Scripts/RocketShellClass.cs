using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketShellClass : MonoBehaviour
{
    public GameObject weaponToStick;
    public GameObject explosionRocketPrefab;

    public AudioSource audio;
    public AudioSource audio2;

    public float speed;
    public float damage;
    public float dmgRadius;
    public float Timer;
    
    public bool isProjectile;
    public bool isHoming;
    public bool playersBullet;
    
    public void FixedUpdate()
    {
        MoveBullet();
    }

    public void LaunchSound()
    {
        audio.Play();
    }

    public virtual void MoveBullet ()
    {
        Timer += Time.deltaTime;

        if ((isProjectile==true) && (gameObject.GetComponent<Rigidbody>()!=null))
            gameObject.GetComponent<Rigidbody>().AddRelativeForce(0, 0, speed += 18, ForceMode.Acceleration);
        
        if (playersBullet == true && isHoming == true)
            if (Physics.BoxCast(GameMaster.GM.player.transform.TransformPoint(0, 0, 0), new Vector3(20, 20, 2), transform.forward, out RaycastHit hitInfo2, Quaternion.Euler(0, 0, 0), 400, 1 << 8))
                if ((hitInfo2.transform.tag == "Seeker" || hitInfo2.transform.tag == "Trooper")&&(hitInfo2.transform.GetComponent<FractionIndexClass>().fractionId != 0))
                {
                    gameObject.transform.LookAt(hitInfo2.transform.position + new Vector3(0, 1, 0));
                    Vector3 normalizeDirection = (hitInfo2.transform.position + new Vector3(0, 1, 0) - gameObject.transform.position).normalized;
                    gameObject.transform.position += normalizeDirection * Time.deltaTime * 30;
                    //GameObject Explode = Instantiate(GameMaster.GM.EnmemyDestroy, hitInfo2.transform.position, Quaternion.Euler(0, 0, 0));       
                }

        if (Timer > 5)
           Destroy(gameObject);
    }

    public virtual void Awake()
    {
        isProjectile = true;
        playersBullet = false;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "Rocket")
            if (playersBullet == false && collision.gameObject.tag=="Player" || playersBullet == true && collision.gameObject.tag != "Player" || playersBullet == false && collision.gameObject.tag != "Player")
            {
                ContactPoint contact = collision.contacts[0];
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
                Vector3 pos = contact.point;
                GameObject Explosions = Instantiate(explosionRocketPrefab, pos, Quaternion.Euler(0, 0, 0));
                Destroy(Explosions, 2f);

                Explode();
            }
    }

    public virtual void Explode()
    {
        audio2.Play();

        foreach (Transform child in gameObject.transform)
        {
            // if (child.name != "WFXMR_Nuke 1")
            GameObject.Destroy(child.gameObject);
        }

        Collider ammoCollider = gameObject.transform.GetComponent<Collider>();

        ammoCollider.enabled = false;
        gameObject.transform.GetComponent<MeshRenderer>().enabled = false;
                
        //ExplosionParticleSystem.Play();
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 35);

        foreach (Collider hit in colliders)
        {
            FractionIndexClass fractionHitObject = hit.GetComponent<FractionIndexClass>();

            if ((hit.GetComponent<Rigidbody>() != null && hit.GetComponent<SeekerClass>() == null && hit.GetComponent<TrooperClass>() == null) && (hit.name != "Rocket") && (hit.name != "Bomb"))
                hit.GetComponent<Rigidbody>().AddExplosionForce(500, gameObject.transform.position + new Vector3(0, 0, 0), 50, 1, ForceMode.Impulse);

            if (fractionHitObject != null && hit.gameObject != null)
            {
                if (weaponToStick != null && fractionHitObject.dead == false)
                    fractionHitObject.whoIsDamaging = weaponToStick.GetComponent<WeaponClass>().objectToStick.gameObject;

                if (weaponToStick != null && weaponToStick.GetComponent<WeaponClass>().objectToStick.gameObject.name == "Player")
                {
                    if (fractionHitObject.transform.tag != "Ship")
                        fractionHitObject.TakeDamage(damage * 2f);
                    else
                        fractionHitObject.TakeDamage(damage / 10);
                    //hit.transform.GetComponent<FractionIndexClass>().TakeDamage(damage * weaponToStick.GetComponent<WeaponClass>().ownerLevel);
                }
                else if (weaponToStick != null )
                {
                    //hit.transform.GetComponent<FractionIndexClass>().TakeDamage(damage * weaponToStick.GetComponent<WeaponClass>().ownerLevel);
                    if(fractionHitObject.transform.tag != "Ship")
                        fractionHitObject.TakeDamage(damage);
                    else
                        fractionHitObject.TakeDamage(damage / 10);
                }
            }

            if (hit.GetComponent<PlayerClass>() != null)
            {
                hit.GetComponent<PlayerClass>().TakeDamage(damage);
            }
        }

        if (gameObject.GetComponent<MeshRenderer>() != null)
            gameObject.GetComponent<MeshRenderer>().enabled = false;

        if (ammoCollider != null)
            ammoCollider.enabled = false;

        foreach (Transform child in transform)
        {
            if (child.GetComponent<MeshRenderer>() != null)
                child.GetComponent<MeshRenderer>().enabled = false;
            if (child.GetComponent<Collider>() != null)
                child.GetComponent<Collider>().enabled = false;
        }

        Destroy(gameObject, 3);
    }
}

