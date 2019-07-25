using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    float x, y, z;

    private void Awake()
    {
        x = Random.Range(-0.2f, 0.2f);
        y = Random.Range(-0.2f, 0.2f);
        z = Random.Range(-0.2f, 0.2f);
    }
    private void Start()
    {
        
    }
    void FixedUpdate()
    {
        transform.Rotate(x, y, z);
    }
}
