using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Trooper
{
    float randomY;
    Rigidbody rb;

    public void Start()
    {
        //randomY = Random.Range(0, 10); 
        timer = Random.Range(0, 100);
        health = 800;
        maxHP = 800;
        rb = gameObject.GetComponent<Rigidbody>();

        randomCollisionStuckDirection = Random.Range(0, 2);

        if (randomCollisionStuckDirection == 0)
            randomCollisionStuckDirection = -1;
        if (randomCollisionStuckDirection == 1)
            randomCollisionStuckDirection = 1;
    }

    public override void Update()
    {
        FindItemsAround(GameMaster.GM.globalObjectList, GameMaster.GM.platformObjectList);

        //Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 50);

        //foreach (Collider hit in colliders)
        //{
        //    if ((hit.GetComponent<Rigidbody>() != null) && (hit.name != "Rocket") && hit.GetComponent<PlayerClass>() == null && hit.GetComponent<FighterClass>() != null)
        //        hit.GetComponent<Rigidbody>().AddExplosionForce(100, gameObject.transform.position + new Vector3(0, 0, 0), 0, 1, ForceMode.Force);
        //}

        if (dead == false)
            FlyAnimation();
        else
            rb.useGravity = true;
    }

    public void FlyAnimation ()
    {
        timer += Time.deltaTime;
        //Vector3 MoveSin = new Vector3(transform.position.x + Mathf.Sin(Timer) * 0.1f,
        //  Terrain.activeTerrain.SampleHeight(transform.position) + Mathf.Sin(Timer * 4f + randomY) * 1f + 15f,
        
        Vector3 moveSin = new Vector3(transform.position.x + Mathf.Sin(timer) * 0.1f,
            transform.position.y + Mathf.Sin(timer * 2f) * 0.1f,
                transform.position.z + Mathf.Sin(timer * 2f) * 0.1f);
        
        if (transform.position.y < 15)
            rb.AddForce(0, 300, 0, ForceMode.Impulse);

        rb.MovePosition(moveSin);
    }

    public override void OnCollisionStay(Collision collisioninfo)
    {
        if (collisioninfo.gameObject.GetComponent<Building>() != null || collisioninfo.gameObject.GetComponent<Tower>() != null || collisioninfo.gameObject.GetComponent<Seeker>() != null)
        {
            gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(100 * randomCollisionStuckDirection, 30, 0), ForceMode.Impulse);
        }
    }
}
