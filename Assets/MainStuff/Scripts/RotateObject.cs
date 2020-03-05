using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    float x, y, z;
    float speed;
    private void Awake()
    {
        speed = 5;
        x = Random.Range(-speed, speed);
        y = Random.Range(-speed, speed);
        z = Random.Range(-speed, speed);
    }
    private void Start()
    {
        
    }
    void FixedUpdate()
    {
        if (gameObject.name == "art_5")
            transform.Rotate(0, 0.2f, 0);
        else
            transform.Rotate(x, y, z);
    }
}
