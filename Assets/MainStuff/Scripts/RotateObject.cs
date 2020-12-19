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
        y = Random.Range(-speed, speed);
    }

    private void Start()
    {
        m_EulerAngleVelocity = new Vector3(0, 10, 0);
    }

    void FixedUpdate()
    {
        transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, 10 * Time.deltaTime);
    }
}
