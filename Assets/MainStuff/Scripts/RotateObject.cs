using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    float x, y, z;
    float speed;
    public Rigidbody RD;
    Vector3 m_EulerAngleVelocity;
    float time;
    private void Awake()
    {
        speed = 0.5f;
        //x = Random.Range(-speed, speed);
        y = Random.Range(-speed, speed);
        //z = Random.Range(-speed, speed);
    }
    private void Start()
    {
        m_EulerAngleVelocity = new Vector3(0, 10, 0);

        
    }
    void FixedUpdate()
    {
        //if (gameObject.name == "art_5")
        //    transform.Rotate(0, 0.2f, 0);
        //else
        //    transform.Rotate(x, y, z);
        time = Time.deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * time);
        RD.MoveRotation(RD.rotation * deltaRotation);

    }
}
