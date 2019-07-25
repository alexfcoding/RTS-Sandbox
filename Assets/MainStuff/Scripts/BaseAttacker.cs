using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttacker : Follower
{
    public override void MoveTo()
    {
        Vector3 normalizeDirection = (GameMaster.GM.player.transform.position - gameObject.GetComponent<Transform>().transform.position).normalized;
        gameObject.GetComponent<Transform>().transform.position += normalizeDirection * Time.deltaTime * speed;
    }

    public override void Awake()
    {
        gameObject.tag = "BaseAttacker";
        jumpPower = 25;
        speed = 10;
        jumping = true;
    }

    public override void OnCollisionEnter(Collision collisioninfo)
    {
        if (collisioninfo.gameObject.tag == "Player")
        {
            ContactPoint contact = collisioninfo.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;
            GameObject Boom = Instantiate(GameMaster.GM.effectBoom, pos + new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0));
            Boom.transform.parent = collisioninfo.transform;
            GameMaster.GM.boomSound.Play();
            PlayerClass.mainPlayer.TakeDamage(9);
        }
    }
}

