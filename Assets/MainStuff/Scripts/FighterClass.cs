using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterClass : TrooperClass
{
    float randomY;    
    
    public void Start()
    {
        //randomY = Random.Range(0, 10); 
        timer = Random.Range(0, 100);
    }

    public override void Update()
    {
        FindItemsAround(GameMaster.GM.globalObjectList, GameMaster.GM.platformObjectList);

        if (dead == false)
            FlyAnimation();
        else
            gameObject.GetComponent<Rigidbody>().useGravity = true;
    }

    public void FlyAnimation ()
    {
        timer += Time.deltaTime;
        //Vector3 MoveSin = new Vector3(transform.position.x + Mathf.Sin(Timer) * 0.1f,
        //  Terrain.activeTerrain.SampleHeight(transform.position) + Mathf.Sin(Timer * 4f + randomY) * 1f + 15f,
        
        Vector3 moveSin = new Vector3(transform.position.x + Mathf.Sin(timer) * 0.1f,
            transform.position.y + Mathf.Sin(timer * 2f) * 0.1f,
                transform.position.z + Mathf.Sin(timer * 2f) * 0.1f);
        
        if (transform.position.y < 5)
            gameObject.GetComponent<Rigidbody>().AddForce(0, 300, 0, ForceMode.Impulse);

        gameObject.GetComponent<Rigidbody>().MovePosition(moveSin);
    }

    public override void OnCollisionStay(Collision collisioninfo)
    {
        
    }
}
