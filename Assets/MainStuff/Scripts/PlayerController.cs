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

        Vector3 playerMovement = new Vector3(hor, 0f, ver) * 30 * Time.deltaTime;

         // if (gameObject.GetComponent<PlayerClass>().Dead == false)
         //     transform.Translate(playerMovement, Space.Self);
    }
}
