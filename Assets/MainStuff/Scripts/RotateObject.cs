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
        if (gameObject.name == "art_5")
            transform.Rotate(0, 0.2f, 0);
        else
            transform.Rotate(x, y, z);
    }
}
