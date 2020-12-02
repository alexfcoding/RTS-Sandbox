using System.Collections;
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
        //mouseX += Input.GetAxis("Mouse X") * RotationSpeed;
        //mouseY += Input.GetAxis("Mouse Y") * RotationSpeed*(-1);
        //mouseY = Mathf.Clamp(mouseY, -35, 60);
        transform.LookAt(lookObject);

        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY += Input.GetAxis("Mouse Y") * rotationSpeed * (-1);
        mouseY = Mathf.Clamp(mouseY, -60, 60);

        //transform.LookAt(TRG);
        if (player.GetComponent<PlayerClass>().spectatorMode == false)
            if (player.GetComponent<PlayerClass>().stopCamControls == false)
                player.rotation = Quaternion.Euler(mouseY, mouseX - 70, 0);
        
    }
}
