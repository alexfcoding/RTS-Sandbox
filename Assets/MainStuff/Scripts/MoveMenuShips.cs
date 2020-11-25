using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMenuShips : MonoBehaviour
{
    float timerY;
    float timerX;
    float timerZ;

    void Start()
    {
        timerX = Random.Range(0, 100);
        timerY = Random.Range(0, 100);
        timerZ = Random.Range(0, 100);
    }
      
    void FixedUpdate()
    {
        timerX += Time.deltaTime;
        timerY += Time.deltaTime;
        timerZ += Time.deltaTime;
        
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + Mathf.Sin(timerX) * 0.15f, gameObject.transform.position.y + Mathf.Sin(timerY) * 0.15f, gameObject.transform.position.z + Mathf.Sin(timerZ) * 0.15f);
        gameObject.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, 6 * Time.deltaTime);
    }
}
