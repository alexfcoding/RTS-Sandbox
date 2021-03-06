﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    public float rotationSpeed;
    float mouseX, mouseY;
    public Transform lookObject;
    public Transform player;
    
    void Start()
    {
        rotationSpeed = 0.8f;        
    }

    void LateUpdate()
    {
        Control();
    }

    void Control ()
    {
        transform.LookAt(lookObject);

        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY += Input.GetAxis("Mouse Y") * rotationSpeed * (-1);
        mouseY = Mathf.Clamp(mouseY, -60, 60);
                
        if (player.GetComponent<Player>().spectatorMode == false)
            if (player.GetComponent<Player>().stopCamControls == false)
                player.rotation = Quaternion.Euler(mouseY, mouseX - 70, 0);        
    }
}
