using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speedPlayer;
    
    void Start()
    {
        speedPlayer = 20f;
    }

    void FixedUpdate()
    {
        playerMovement();
    }

    void playerMovement ()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
/*
        if (hor > 0 && ver == 0)
            gameObject.GetComponent<Animator>().Play("WalkRight_Shoot");
        if (hor < 0 && ver == 0)
            gameObject.GetComponent<Animator>().Play("WalkLeft_Shoot");
        if (hor > 0 && ver > 0) 
            gameObject.GetComponent<Animator>().Play("WalkRight_Shoot");
        if (hor > 0 && ver < 0)
            gameObject.GetComponent<Animator>().Play("WalkRight_Shoot");
        if (hor < 0 && ver > 0)
            gameObject.GetComponent<Animator>().Play("WalkLeft_Shoot");
        if (hor < 0 && ver < 0)
            gameObject.GetComponent<Animator>().Play("WalkLeft_Shoot");
        if ((ver == 0) && (hor ==0))
            gameObject.GetComponent<Animator>().Play("Idle");
        if ((ver > 0) && (hor == 0))
            gameObject.GetComponent<Animator>().Play("Run_Guard");
        if ((ver < 0) && (hor == 0))
            gameObject.GetComponent<Animator>().Play("WalkBack_Shoot");
*/

        Vector3 playerMovement = new Vector3(hor, 0f, ver) * 30 * Time.deltaTime;

     // if (gameObject.GetComponent<PlayerClass>().Dead == false)
     //     transform.Translate(playerMovement, Space.Self);
    }
}
