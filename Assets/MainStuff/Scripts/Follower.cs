using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : FractionIndexClass
{
    public float speed;
    public float jumpPower;
    public bool followOwner;
    public bool followOwnerCharger;
    public bool jumping;
    public bool moveToNextOwner;
    public GameObject pickUpEffect;
    public GameObject ownerToFollow;
    public GameObject chargerToFollow;
    public AudioSource audioPickEnemyCube;
    public AudioSource audioPickCubeToMe;
    public AudioSource audioPickShipCube;

    public override void Awake()
    {
        health = 50;
        maxHP = 50;
        gameObject.tag = "Follower";
        jumpPower = 25;
        speed = 34;
        followOwner = false;
        fractionId = 10;
        followOwnerCharger = false;
        jumping = true;
        isSimpleFollower = true;
        isVulnerable = false;
    }

    public void Update()
    {
        MoveTo();
    }

    public virtual void MoveTo()
    {
        if (followOwner == true) // Если двигаться к владельцу (определяется в Collision ниже)
        {
            Vector3 normalizeDirection; // Направление движения
            float xpos = gameObject.GetComponent<Transform>().transform.position.x;
            float zpos = gameObject.GetComponent<Transform>().transform.position.z;                       

            if (ownerToFollow != null)
            {
                if (ownerToFollow.GetComponent<PlayerClass>() != null) // Если владелец куба - игрок ...
                    normalizeDirection = (ownerToFollow.transform.TransformPoint(0, 0, -8000) - gameObject.GetComponent<Transform>().transform.position + new Vector3(0, 3, 0)).normalized; // ... то двигаться к точке позади игрока
                else // Если владелец куба - не игрок ...
                    normalizeDirection = (ownerToFollow.transform.TransformPoint(0, 10, -3) - gameObject.GetComponent<Transform>().transform.position + new Vector3(0, 3, 0)).normalized; // ... то двигаться к точке позади владельца

                if (ownerToFollow.gameObject.tag == "Seeker")
                    transform.position += normalizeDirection * Time.deltaTime * speed * 1.5f; // Сближать координату куба с Seeker или Player (перемещать куб)

                if (ownerToFollow.gameObject.tag == "Player")
                    transform.position += normalizeDirection * Time.deltaTime * speed * 2f; // Сближать координату куба с Seeker или Player (перемещать куб)

                if (ownerToFollow.gameObject.tag != "Player" && ownerToFollow.gameObject.tag != "Seeker")
                    transform.position += normalizeDirection * Time.deltaTime * speed * 1f; // Сближать координату куба с Seeker или Player (перемещать куб)
            }

            if (ownerToFollow != null)
                if (ownerToFollow.GetComponent<SeekerClass>() != null) // Если владелец - Seeker ...
                    if (ownerToFollow.GetComponent<SeekerClass>().totallyDead == true) // ... и он мертв ...
                    {
                            gameObject.GetComponent<Renderer>().material.color = Color.white; // ... то изменить цвет куба на белый
                    
                            if (ownerToFollow.GetComponent<SeekerClass>().whoIsDamaging!=null)
                            {
                                ownerToFollow = ownerToFollow.GetComponent<SeekerClass>().whoIsDamaging;
                                fractionId = 10; // Установить идентификатор FractionId
                                moveToNextOwner = true;
                                StartCoroutine(MoveToNextOwnerTrigger());
                                gameObject.tag = "Follower"; // вернуть тэг кубу Follower
                            }
                            else 
                            {
                                followOwner = false; // убрать владельца
                                gameObject.tag = "Follower"; // вернуть тэг кубу Follower
                                fractionId = 10;                                
                            }
                    }

            for (int i = 0; i < GameMaster.GM.platformObjectList.Count; i++) // Цикл по всем платформам
            {
                if (GameMaster.GM.platformObjectList[i] != null)
                    if ((xpos > GameMaster.GM.platformObjectList[i].transform.position.x - GameMaster.GM.platformObjectList[i].transform.lossyScale.x / 2) && (xpos < GameMaster.GM.platformObjectList[i].transform.position.x + GameMaster.GM.platformObjectList[i].transform.lossyScale.x / 2) && 
                        (zpos > GameMaster.GM.platformObjectList[i].transform.position.z - GameMaster.GM.platformObjectList[i].transform.lossyScale.z / 2) && (zpos < GameMaster.GM.platformObjectList[i].transform.position.z + GameMaster.GM.platformObjectList[i].transform.lossyScale.z / 2)) // Если куб над или под платформой i ...
                        {
                            chargerToFollow = GameMaster.GM.platformObjectList[i]; // ... то установить платформу к которой двигаться ChargerToFollow
                        
                            followOwnerCharger = true; // Если над или под платформой, то команда двигаться к платформе = true ...
                            followOwner = false; // ... и перестать следовать за владельцем
                        }
            }

            if (moveToNextOwner == true)
            {
                MoveAfterOwnerDeath();
            }

        }

        if (followOwnerCharger == true && chargerToFollow.gameObject != null) // Если двигаться к платформе
        {
            Vector3 normalizeDirection = (chargerToFollow.transform.position - gameObject.GetComponent<Transform>().transform.position + new Vector3(0, 8, 0)).normalized; // ... то определить направление и ...
            gameObject.GetComponent<Transform>().transform.position += normalizeDirection * Time.deltaTime * speed * 2; // ... сближаться с платформой
            gameObject.GetComponent<Rigidbody>().AddForce(0, 1000, 0); // Подталкивать куб вверх для столкновения с Ship
        }
    }

    public void JumpAnimation()
    {
        if (gameObject.GetComponent<Rigidbody>() != null && jumping == true)
        { 
            gameObject.GetComponent<Rigidbody>().GetComponent<Rigidbody>().AddForce(0, Random.Range(-jumpPower, jumpPower), 0, ForceMode.Impulse);
            gameObject.GetComponent<Rigidbody>().GetComponent<Rigidbody>().AddForce(Random.Range(-jumpPower, jumpPower / 10), 0, 0, ForceMode.Impulse);
            gameObject.GetComponent<Rigidbody>().GetComponent<Rigidbody>().AddForce(0, 0, Random.Range(-jumpPower, jumpPower), ForceMode.Impulse);
        }
    }

    public void MoveAfterOwnerDeath ()
    {
        speed = 60;

        gameObject.GetComponent<Rigidbody>().mass = 50;
        gameObject.GetComponent<Rigidbody>().angularDrag = 1;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        Vector3 normalizeDirection;

        if (ownerToFollow != null)
        { 
            normalizeDirection = (ownerToFollow.transform.position - gameObject.GetComponent<Transform>().transform.position).normalized; ; // движемся к тому, кто убил владельца
            gameObject.GetComponent<Transform>().transform.position += normalizeDirection * Time.deltaTime * speed; // ... сближаться с новым владельцем
        }
    }

    IEnumerator MoveToNextOwnerTrigger()
    {
        yield return new WaitForSeconds(0.8f);
        moveToNextOwner = false;
        speed = 50;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        gameObject.GetComponent<Rigidbody>().mass = 5;

        if (fractionId == 10)
            followOwner = false;
    }

    public virtual void OnCollisionEnter(Collision collisioninfo)
    {
        //if (collisioninfo.gameObject.tag == "Terrain" && jumping == true)
        //{
        //    gameObject.GetComponent<Rigidbody>().GetComponent<Rigidbody>().AddForce(0, Random.Range(4, jumpPower * 6), 0, ForceMode.Impulse);
        //    gameObject.GetComponent<Rigidbody>().GetComponent<Rigidbody>().AddForce(Random.Range(-jumpPower, jumpPower), 0, 0, ForceMode.Impulse);
        //    gameObject.GetComponent<Rigidbody>().GetComponent<Rigidbody>().AddForce(0, 0, Random.Range(-jumpPower, jumpPower), ForceMode.Impulse);
        //}

        if (collisioninfo.gameObject.tag == "Seeker") // Если куб столкнулся с Seeker или Player ...
        { 
            if (gameObject.GetComponent<FractionIndexClass>().fractionId == 10) // ... и его индекс равен 10 (свободный)
            {
                gameObject.GetComponent<Follower>().SetFractionId(collisioninfo.gameObject.GetComponent<FractionIndexClass>().fractionId); // установить  индекс равный Seeker или Player

                if (collisioninfo.gameObject.GetComponent<SeekerClass>() != null) // Если столкновение с Seeker, то ...
                {
                    SeekerClass Seeker = collisioninfo.gameObject.GetComponent<SeekerClass>();
                    Seeker.findNextObject = true; // Команда Seeker для поиска нового куба
                    Seeker.Heal(1); // Лечение Seeker
                    Seeker.countOfItemsCollected++; // Счетчик подобранных кубов++
                }

                if (collisioninfo.gameObject.GetComponent<PlayerClass>() != null) // Если столкновение с Player, то ... 
                    collisioninfo.gameObject.GetComponent<PlayerClass>().Heal(20); // Лечение Player

                ownerToFollow = collisioninfo.collider.gameObject; // Присвоить кубу владельца OwnerToFollow (Seeker или Player)

                gameObject.GetComponent<Renderer>().material.color = GameMaster.GM.fractionColors[collisioninfo.gameObject.GetComponent<FractionIndexClass>().fractionId];
                gameObject.GetComponent<Follower>().followOwner = true; // Двигаться к владельцу = true
                gameObject.GetComponent<Follower>().followOwnerCharger = false; // Двигаться к площадке = false
                gameObject.tag = "OwnedFollower"; // Изменить тег куба на OwnedFollower (имеет владельца)

                ContactPoint contact = collisioninfo.contacts[0];
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
                Vector3 pos = contact.point;
                gameObject.GetComponent<ParticleSystem>().Play();
                audioPickEnemyCube.Play();
            }
        }
        else if (collisioninfo.gameObject.tag == "Trooper" && ownerToFollow != null && 
        ownerToFollow.GetComponent<SeekerClass>() != null && 
        ownerToFollow.GetComponent<SeekerClass>().dead == false) 
        {
            gameObject.GetComponent<ParticleSystem>().Play();
            audioPickEnemyCube.Play();
            //collisioninfo.gameObject.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            collisioninfo.gameObject.GetComponent<FractionIndexClass>().health += 50;
            collisioninfo.gameObject.GetComponent<FractionIndexClass>().Level += 1;
            GameMaster.GM.RecursiveDestroy(transform, gameObject, 0.2f);
        }

        else if (collisioninfo.gameObject.tag == "Player")
        {
            gameObject.GetComponent<ParticleSystem>().Play();
            audioPickEnemyCube.Play();
            //collisioninfo.gameObject.transform.localScale += new Vector3(0.5f, 0.5f, 0.5f);
            collisioninfo.gameObject.GetComponent<FractionIndexClass>().health += 100;
            collisioninfo.gameObject.GetComponent<FractionIndexClass>().Level += 1;
            if (collisioninfo.gameObject.GetComponent<SeekerClass>() != null && collisioninfo.gameObject.GetComponent<SeekerClass>().currentWeapon != null)
                collisioninfo.gameObject.GetComponent<SeekerClass>().currentWeapon.ownerLevel = Level;
            GameMaster.GM.RecursiveDestroy(transform, gameObject, 0.2f);
        }


        if (collisioninfo.gameObject.tag == "Ship" && gameObject.tag == "OwnedFollower")
        {
            if (ownerToFollow != null)
            {
                if (ownerToFollow.GetComponent<SeekerClass>() != null) // Если владелец куба - Seeker ...
                {
                    ownerToFollow.GetComponent<SeekerClass>().countOfItemsCollected--; // ... то вычесть счетчик--
                    ownerToFollow.GetComponent<SeekerClass>().Heal(10);
                }
            }
            
            gameObject.transform.position = new Vector3(Random.Range(-700, 700), 200, Random.Range(-700, 700));
            fractionId = 10;
            gameObject.GetComponent<Renderer>().material.color = Color.white;
            gameObject.GetComponent<Follower>().followOwner = false;
            gameObject.GetComponent<Follower>().followOwnerCharger = false;
            gameObject.transform.localScale += new Vector3(0f, 0f, 0f);
            gameObject.tag = "Follower";
            
            if (collisioninfo.gameObject != null)
            {
                collisioninfo.gameObject.GetComponent<ShipClass>().EarnMoney(500);
                collisioninfo.gameObject.GetComponent<ShipClass>().Heal(1000);
            }
                

            audioPickShipCube.Play();
            GameMaster.GM.RecursiveDestroy(transform, gameObject, 1);
        }

        if (collisioninfo.gameObject.tag == "Ship" && gameObject.tag == "Follower")
            gameObject.transform.position = new Vector3(Random.Range(-900, 900), 50, Random.Range(-900, 900));
    }

}
