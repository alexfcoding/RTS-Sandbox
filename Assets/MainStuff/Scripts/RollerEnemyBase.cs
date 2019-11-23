using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerEnemyBase : TrooperClass
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        FindItemsAround(GameMaster.GM.globalObjectList, GameMaster.GM.platformObjectList);
    }

    public override void FindItemsAround(List<GameObject> _GlobalObjectList, List<GameObject> _PlatformList)
    {
        if (dead == false && stopDoing == false)
        {
            if (targetToChase != null)
            {
                if (targetToChase.GetComponent<ShipClass>() != null)
                {
                    pointFromShooting = targetToChase.transform.position - new Vector3(0, targetToChase.transform.position.y, 0);
                }
                else
                {
                    pointFromShooting = targetToChase.transform.position;
                }
            }
            else
            {
                int rnDShip = Random.Range(0, GameMaster.GM.mainBaseCount);

                while (rnDShip == fractionId)
                    rnDShip = Random.Range(0, GameMaster.GM.mainBaseCount);

                if (fractionId != 0 && GameMaster.GM.shipObjectList[rnDShip] != null)
                    targetToChase = GameMaster.GM.shipObjectList[rnDShip];
                else
                    targetToChase = GameMaster.GM.player.gameObject;
            }

            if (targetToChase != null && targetToChase.GetComponent<ShipClass>() != null)
                pointFromShooting = pointFromShooting + pointFromShootingRandomize * 4;

            if (targetToChase != null && targetToChase.GetComponent<ShipClass>() == null)
                pointFromShooting = pointFromShooting + pointFromShootingRandomize;

            if ((currentWeapon != null && enemyToLook != null) || (currentWeapon != null && enemyToLook != null))
            {
                if (enemyToLook.GetComponent<ShipClass>() != null)
                {
                   // if (gameObject.name == "LightShip")
                   //     gameObject.transform.LookAt(enemyToLook.transform.position);
                    wait = true;
                }
                else if (enemyToLook.GetComponent<ShipClass>() == null)
                {
                    //gameObject.transform.LookAt(enemyToLook.transform.position);
                    wait = true;
                }
            }
            else
            {
                //gameObject.transform.LookAt(PointFromShooting);
                wait = false;
            }

            // если враг мертв, то двигаться дальше
            if (enemyToLook != null && enemyToLook.GetComponent<SeekerClass>() != null && enemyToLook.GetComponent<SeekerClass>().dead == true && currentWeapon.GetComponent<WeaponClass>().playerFollowingCommand == true)
                wait = false;

            if (((Vector3.Distance(transform.position, pointFromShooting)) > 80) && (wait == false))
            {
                //gameObject.transform.LookAt(pointFromShooting);
                //rbTrooper.AddRelativeForce(Vector3.forward * trooperSpeed * Time.deltaTime * 40, ForceMode.VelocityChange);//* Time.deltaTime * 30
                Vector3 direction = (targetToChase.transform.position - transform.position).normalized;
                rbTrooper.AddForce(direction * 2, ForceMode.VelocityChange);
                //if (gameObject.GetComponent<Animator>() != null)
                //    gameObject.GetComponent<Animator>().Play("Run_Guard");
            }
            else
            {
                wait = true;
                //if (gameObject.GetComponent<Animator>() != null)
                //    gameObject.GetComponent<Animator>().Play("Idle");
            }

            if (((Vector3.Distance(transform.position, pointFromShooting)) > 120))
            {
                wait = false;
            }
        }

        // Анимация смерти
        if (dead == true && stopDoing == false)
        {
            //if (gameObject.GetComponent<Animator>() != null)
            //    gameObject.GetComponent<Animator>().Play("Die");

            stopDoing = true;
        }
    }
}
