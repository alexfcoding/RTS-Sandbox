﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public delegate void SpendMoneyMethod();

public class Player: FactionIndex
{
    public static Player mainPlayer;

    public Rigidbody RB;
    public Weapon currentWeapon;

    public bool alreadyHaveWeapon;
    public bool reload;
    public bool tacticMode;
    public bool deadPlayer;
    public bool soundAlreadyPlaying;
    public bool targetLockedIn;
    public bool stopCamControls;
    public float timer;
    public float tickWeapon;
    public float camX, camY, camZ, cameraX, cameraY, cameraZ;
    public float tickBuilding;
    public float selectorScale;
    public float reloadTime = 1;

    public int playerCubesCount;
    public int playerMoney;
    
    public ParticleSystem fire;
    public ParticleSystem fire2;
    public ParticleSystem fire3;
    public ParticleSystem fire4;
    public ParticleSystem fire5;
    public ParticleSystem fire6;
    public ParticleSystem fire7;
    public ParticleSystem fire8;
    public ParticleSystem fire9;
    public ParticleSystem fire10;

    public List<GameObject> teamMateList;

    public GameObject selector;    
    public GameObject currentShipTarget;
    public GameObject clickedObject;
    public GameObject targetUI, followUI, followUnitUI;
    public GameObject targetSpritePrefab;
    public GameObject targetToLock;
    public GameObject trooperBase;

    public AudioSource buildingInProgress;
    public AudioSource buildingComplete;
    public AudioSource insufficientFunds;
    public AudioSource unableBuildMore;
    public AudioSource missionFailed;
    public AudioSource playerDeath;
    public AudioSource damageSound;
    public AudioSource voice1, voice2, voice3;
    public AudioSource left, right, forward, backwards, up, down;
    public AudioSource pickup, changeWeapon;
    public Transform lookCube;
    public AudioSource buildTower;

    public float fx, fy, fz;
    public TextMesh playerHealth3dText;

    public bool spectatorMode;
    public bool isTimeToGiveCommand;

    public List<Weapon> playerWeaponList;
    int currentWeaponNumber;

    public Sprite targetSprite, followSprite, followUnitSprite;

    public void Start()
    {
        playerHealth3dText.text = $"HP: {health}";
        playerWeaponList = new List<Weapon>();
        currentWeaponNumber = 0;

        GameMaster.GM.myCamera.transform.localPosition = new Vector3(300000, 10000000, -150000);

        targetSprite = targetUI.GetComponent<Image>().sprite;
        followSprite = followUI.GetComponent<Image>().sprite;
        followUnitSprite = followUnitUI.GetComponent<Image>().sprite;
    }
    
    public override void TakeDamage(float damage)
    {
        if (isVulnerable)
        {
            if (health >= damage)
            {
                health -= damage;
            }
            else
            {
                GameMaster.GM.playerHp.color = Color.red;
                GameMaster.GM.playerHp.text = "GameOver";

                if (deadPlayer == false)
                {
                    GameObject Explode = Instantiate(GameMaster.GM.enemyDestroy, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
                    GameObject Smoke = Instantiate(GameMaster.GM.smokeAfterDeath, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
                    Smoke.transform.SetParent(gameObject.transform);
                    RB.isKinematic = true;
                    Time.timeScale = 0.3f;

                    if (playerDeath != null)
                        playerDeath.Play();

                    if (missionFailed != null)
                        missionFailed.Play();

                    Invoke("GoToMainMenu", 1.0f);
                }

                deadPlayer = true;
            }
        }        

        GameMaster.GM.playerHp.text = "PlayerHP: " + Player.mainPlayer.health.ToString();
        playerHealth3dText.text = $"HP: {health}";
    }

    public void Heal (float healpoints)
    {
        health += healpoints;
        GameMaster.GM.playerHp.text = "PlayerHP: " + Player.mainPlayer.health.ToString();
        playerHealth3dText.text = $"HP: {health}";
    }

    public override void Awake()
    {
        if (mainPlayer != null)
            GameObject.Destroy(mainPlayer);
        else
            mainPlayer = this;

        //DontDestroyOnLoad(this);
        level = 1;
        reload = false;
        alreadyHaveWeapon = false;
        tickWeapon = 1;
        timer = 0;
        playerCubesCount = 0;
        playerMoney = 0;
        factionId = 0;
        targetSpritePrefab.transform.localScale = new Vector3(2f, 2f, 2f);
        selector = Instantiate(GameMaster.GM.teamSelect, transform.position + new Vector3(0, 5, 0), Quaternion.Euler(0, 0, 0));
        selector.transform.Rotate(90, 0, 0);
        selector.transform.position = new Vector3(transform.position.x, 2, transform.position.z);
        selectorScale = 0;
        selector.transform.localScale = new Vector3(0, 0, 0);
        tickBuilding = 0.1f;
        trooperBase = null;

        cameraX = GameMaster.GM.myCamera.transform.localPosition.x;
        cameraY = GameMaster.GM.myCamera.transform.localPosition.y;
        cameraZ = GameMaster.GM.myCamera.transform.localPosition.z;
    }

    public void Update()
    {
        // Правая кнопка мыши  
        if (Input.GetKey(KeyCode.Mouse1))
        {
            selectorScale += 4;
            selector.transform.localScale = new Vector3(selectorScale, selectorScale, selectorScale);
            tacticMode = true;
            lookCube.transform.localPosition = new Vector3(0, 4000, 3000);
            GameMaster.GM.myCamera.transform.localPosition = Vector3.Lerp(GameMaster.GM.myCamera.transform.localPosition, new Vector3(0, 14000, -40000),0.05f);
        }
        
        // Если включен тактический режим
        if (tacticMode == true)
        {
            RaycastHit groundDistance; ;
            Physics.Raycast(transform.position, transform.forward, out groundDistance, 2000);

            float playerHeightTerrain = Terrain.activeTerrain.SampleHeight(transform.position);
            float selectorDistance = Mathf.Sqrt(Mathf.Pow(groundDistance.distance, 2) - Mathf.Pow(playerHeightTerrain, 2));
            selector.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);

            foreach (GameObject teamMate in teamMateList)
            {
                if (teamMate != null && teamMate.GetComponent<Trooper>() != null && teamMate.GetComponent<Trooper>().teamSelectMark != null)
                    Destroy(teamMate.GetComponent<Trooper>().teamSelectMark);
            }

            teamMateList.Clear();

            Collider[] team = Physics.OverlapBox(selector.transform.position, new Vector3(selectorScale / 1.5f, 9999, selectorScale / 1.5f), Quaternion.identity, 1 << 8);

            foreach (Collider teamMate in team)
                if (teamMate.tag == "Trooper" && teamMate.gameObject.GetComponent<FactionIndex>().factionId == 0)
                {
                    teamMateList.Add(teamMate.gameObject);
                    teamMate.GetComponent<Trooper>().teamSelectMark = Instantiate(GameMaster.GM.teamMarker, teamMate.transform.position + new Vector3(0, 12, 0), Quaternion.Euler(0, 0, 0));
                    if (teamMate.name == "LightShip")
                    {
                        teamMate.GetComponent<Trooper>().teamSelectMark.transform.position = new Vector3(teamMate.GetComponent<Trooper>().transform.position.x, teamMate.GetComponent<Trooper>().transform.position.y - 3, teamMate.GetComponent<Trooper>().transform.position.z);
                    }

                    if (teamMate.name == "Trooper")
                    {
                        teamMate.GetComponent<Trooper>().teamSelectMark.transform.position = new Vector3(teamMate.GetComponent<Trooper>().transform.position.x, 0.4f, teamMate.GetComponent<Trooper>().transform.position.z);
                    }

                    teamMate.GetComponent<Trooper>().teamSelectMark.transform.Rotate(90, 0, 0);
                    teamMate.GetComponent<Trooper>().teamSelectMark.transform.localScale = new Vector3(4, 4, 4);
                    teamMate.GetComponent<Trooper>().teamSelectMark.transform.SetParent(teamMate.gameObject.transform);                    
                }

            CheckTacticMode();
        }
        
        // Наведение на цель для оружия
        //if (Physics.BoxCast(GameMaster.GM.player.transform.TransformPoint(0, 0, 0), new Vector3(0, 20, 20), transform.forward, out RaycastHit hitInfo2, Quaternion.Euler(0, 0, 0), 300, 1 << 8))
        //{
        //    if (hitInfo2.transform.gameObject != null && hitInfo2.transform.GetComponent<SeekerClass>() != null && hitInfo2.transform.GetComponent<ShipClass>() == null && hitInfo2.transform.GetComponent<SeekerClass>().dead == false)
        //    {
        //        if (hitInfo2.transform.GetComponent<FractionIndexClass>().fractionId != 0)
        //        {
        //            targetLockedIn = true;
        //            targetToLock = hitInfo2.transform.gameObject;

        //            if (hitInfo2.transform.name == "LightShip")
        //                targetSpritePrefab.transform.position = hitInfo2.transform.position + new Vector3(0, 0, 0);
        //            els
        //                targetSpritePrefab.transform.position = hitInfo2.transform.position + new Vector3(0, 3, 0);

        //            targetSpritePrefab.transform.LookAt(Camera.main.transform.position, -Vector3.up);
                        
        //            if (hitInfo2.transform.GetComponent<FractionIndexClass>().fractionId == 0)
        //                targetSpritePrefab.GetComponent<SpriteRenderer>().color = Color.green;
        //            else
        //                targetSpritePrefab.GetComponent<SpriteRenderer>().color = Color.red;
        //        }
        //    }
        //    else if (hitInfo2.transform.gameObject != null && hitInfo2.transform.GetComponent<SeekerClass>() != null && hitInfo2.transform.GetComponent<SeekerClass>().dead == true)
        //    {
        //        targetLockedIn = false;
        //        targetSpritePrefab.transform.position = new Vector3(0, 1000, 0);
        //    }
        //}
        //else
        //{
        //    targetLockedIn = false;
        //    targetSpritePrefab.transform.position = new Vector3(0, 1000, 0);
        //}

        if (spectatorMode == true)
        {
            transform.RotateAround(new Vector3(0, 0, 0), transform.up, 2f * Time.deltaTime);
            transform.LookAt(new Vector3(0, 0, 0));
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(-100, 0, 0)), Time.deltaTime);
        }

        Vector3 MoveSin = new Vector3(transform.position.x + Mathf.Sin(timer * 2f) * 0.03f, transform.position.y + Mathf.Sin(timer * 1f) * 0.03f,
            transform.position.z + Mathf.Sin(timer * 1f) * 0.03f);

        gameObject.GetComponent<Rigidbody>().MovePosition(MoveSin);

        camX = GameMaster.GM.myCamera.transform.localPosition.x;
        camY = GameMaster.GM.myCamera.transform.localPosition.y;
        camZ = GameMaster.GM.myCamera.transform.localPosition.z;

        if (tacticMode == false && isTimeToGiveCommand == false)
        {
            GameMaster.GM.myCamera.transform.localPosition = new Vector3(camX + Mathf.Sin(timer * fx) * 2f, camY + Mathf.Sin(timer * fy) * 1f,
            camZ + Mathf.Sin(timer * fz) * 2f);
            lookCube.transform.localPosition = new Vector3(0, 1000, 3000);

            float smoothTime = 0.3F;
            Vector3 velocity = Vector3.zero;

            //GameMaster.GM.myCamera.transform.localPosition = Vector3.SmoothDamp(GameMaster.GM.myCamera.transform.localPosition, new Vector3(cameraX, cameraY, cameraZ), ref velocity, smoothTime);
            GameMaster.GM.myCamera.transform.localPosition = Vector3.Lerp(GameMaster.GM.myCamera.transform.localPosition, new Vector3(cameraX, cameraY, cameraZ), 0.04f);
        }

        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 50, 1 << 9);

        foreach (Collider hit in colliders)
            if (hit.tag == "Follower" && dead == false)
            {
                Vector3 normalizeDirection = (gameObject.transform.position - hit.transform.position).normalized;
                hit.transform.position += normalizeDirection * Time.deltaTime * hit.GetComponent<Follower>().speed * 2;
            }

        timer += Time.deltaTime;

        waitForCommand();
                
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            stopCamControls = true;
            targetUI.GetComponent<Image>().enabled = true;
        }

        if (Input.GetKey("f"))
        {
            spectatorMode = true;
        }

        if (Input.GetKeyUp("f"))
        {
            spectatorMode = false;
        }

        if (Input.GetKey("j"))
        {
            currentWeapon.GetComponent<Transform>().position = gameObject.transform.TransformPoint(0, 0, 6000);
            currentWeapon.GetComponent<Weapon>().tag = "WithoutUser";
            currentWeapon.transform.parent = null;
            currentWeapon.objectToStick = null;
            currentWeapon.GetComponent<Weapon>().tick = 0f;
            currentWeapon.GetComponent<Weapon>().timer = 0;
            currentWeapon.GetComponent<Weapon>().reloadNow = true;
            currentWeapon.GetComponent<Weapon>().soundStop = false;
            currentWeapon.GetComponent<Collider>().enabled = true;
            alreadyHaveWeapon = false;
        }

        if (Input.GetKey("g"))
        {
            for (int i = 0; i < GameMaster.GM.globalObjectList.Count; i++)
                Destroy(GameMaster.GM.globalObjectList[i].gameObject);

            GameMaster.GM.globalObjectList.Clear();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            RB.AddRelativeForce(Vector3.forward * 20000);
        }

        if (Input.GetKeyUp("0"))
        {
            CallBarracsConstruction();
        }

        if (Input.GetKeyUp("9"))
        {
            CallFactoryConstruction();
        }

        if (Input.GetKeyUp("1"))
        {
            CallCreatingTrooper();
            buildTower.Play();
        }

        if (Input.GetKeyUp("2"))
        {
            CallCreatingLightShip();
            buildTower.Play();
        }

        if (Input.GetKeyDown("3"))
        {
            CallCreatingGunTower();
            buildTower.Play();
        }

        if (Input.GetKeyUp("4"))
        {
            CallCreatingTower();
            buildTower.Play();
        }

        if (Input.GetKey("w"))
        {
            if (forward.isPlaying == false)
            {
                forward.Play();
            }
        }       

        if (Input.GetKeyUp("w"))
        {
            if (forward.isPlaying == true)
            {
                IEnumerator fadeSound1 = FadeAudioSource.FadeOut(forward, 0.5f);
                StartCoroutine(fadeSound1);
            }
        }

        if (Input.GetKey("s"))
        {
            if (backwards.isPlaying == false)
            {
                backwards.Play();
            }
        }       

        if (Input.GetKeyUp("s"))
        {
            if (backwards.isPlaying == true)
            {
                IEnumerator fadeSound1 = FadeAudioSource.FadeOut(backwards, 0.5f);
                StartCoroutine(fadeSound1);
            } 
        }

        if (Input.GetKey("d"))
        {
            if (right.isPlaying == false)
            {
                right.Play();
            }
        }

        if (Input.GetKeyUp("d"))
        {
            if (right.isPlaying == true)
            {
                IEnumerator fadeSound1 = FadeAudioSource.FadeOut(right, 0.5f);
                StartCoroutine(fadeSound1);
            }
        }

        if (Input.GetKey("a"))
        {
            if (left.isPlaying == false)
            {
                left.Play();
            }      
        }

        if (Input.GetKeyUp("a"))
        {
            if (left.isPlaying == true)
            {
                IEnumerator fadeSound1 = FadeAudioSource.FadeOut(left, 0.5f);
                StartCoroutine(fadeSound1);
            }
        }

        if (Input.GetKey("q"))
        {
            if (down.isPlaying == false)
            {
                down.Play();
            }
        }
        
        if (Input.GetKeyUp("q"))
        {
            if (down.isPlaying == true)
            {
                IEnumerator fadeSound1 = FadeAudioSource.FadeOut(down, 0.5f);
                StartCoroutine(fadeSound1);
            }
        }

        if (Input.GetKey("e"))
        {
            if (up.isPlaying == false)
            {
                up.Play();
            }    
        }       

        if (Input.GetKeyUp("e"))
        {
            if (up.isPlaying == true)
            {
                IEnumerator fadeSound1 = FadeAudioSource.FadeOut(up, 0.5f);
                StartCoroutine(fadeSound1);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMainMenu();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            currentWeaponNumber += 1;

            if (currentWeaponNumber > playerWeaponList.Count - 1)
            {
                currentWeaponNumber = 0;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            currentWeaponNumber -= 1;

            if (currentWeaponNumber < 0)
            {
                currentWeaponNumber = playerWeaponList.Count - 1;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            changeWeapon.Play();
            
            if (playerWeaponList.Count > 0)
            {
                Component[] renderer;

                for (int i = 0; i < playerWeaponList.Count; i++)
                {
                    if (playerWeaponList[i].GetComponent<MeshRenderer>() != null)
                        playerWeaponList[i].GetComponent<MeshRenderer>().enabled = false;

                    if (playerWeaponList[i].GetComponentsInChildren<Renderer>() != null)
                    {
                        renderer = playerWeaponList[i].GetComponentsInChildren<Renderer>();

                        for (int j = 0; j < renderer.Length; j++)
                        {
                            if (renderer[j].gameObject.name == "Laser_Gun")
                                renderer[j].GetComponent<Renderer>().enabled = false;
                        }
                    }
                }

                currentWeapon = playerWeaponList[currentWeaponNumber];

                if (currentWeapon.GetComponent<MeshRenderer>() != null)
                    currentWeapon.GetComponent<MeshRenderer>().enabled = true;

                if (currentWeapon.GetComponentsInChildren<Renderer>() != null)
                {
                    renderer = currentWeapon.GetComponentsInChildren<Renderer>();

                    for (int j = 0; j < renderer.Length; j++)
                    {
                        if (renderer[j].gameObject.name == "Laser_Gun")
                            renderer[j].GetComponent<Renderer>().enabled = true;
                    }
                }
            }
        }
    }

    public void CheckTacticMode()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            tacticMode = false;
            isTimeToGiveCommand = true;
            selector.transform.localScale = new Vector3(0, 0, 0);
            selectorScale = 0;
        }
    }

    public void waitForCommand ()
    {
        if (isTimeToGiveCommand == true)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 5000))
            {              
                if (hit.transform.GetComponent<Seeker>() != null || hit.transform.GetComponent<Player>() != null)
                {
                    targetUI.GetComponent<Image>().enabled = true;                    

                    if (hit.transform.GetComponent<FactionIndex>().factionId != 0)
                    {
                        targetUI.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                        targetUI.GetComponent<Image>().sprite = targetSprite;
                        targetUI.GetComponent<Image>().transform.position = Input.mousePosition;
                        targetUI.GetComponent<Image>().color = GameMaster.GM.fractionColors[hit.transform.GetComponent<FactionIndex>().factionId];
                    }
                    else
                    {
                        targetUI.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        targetUI.GetComponent<Image>().sprite = followUnitSprite;
                        targetUI.GetComponent<Image>().transform.position = Input.mousePosition + new Vector3(0, 20, 0);
                        targetUI.GetComponent<Image>().color = GameMaster.GM.fractionColors[0];
                    }

                }                

                if (hit.transform.gameObject.GetComponent<Terrain>() != null)
                {
                    targetUI.transform.localScale = new Vector3(0.4f, 0.5f, 0.5f);
                    targetUI.GetComponent<Image>().sprite = followSprite;
                    targetUI.GetComponent<Image>().transform.position = Input.mousePosition + new Vector3(0, 25, 0);
                    targetUI.GetComponent<Image>().color = GameMaster.GM.fractionColors[0];
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && isTimeToGiveCommand == true) // Левая кнопка мыши
        {
            isTimeToGiveCommand = false;            
            targetUI.GetComponent<Image>().enabled = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            selector.transform.localScale = new Vector3(0, 0, 0);
            RaycastHit hit2;
            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);

            clickedObject = null;

            if (Physics.Raycast(ray2, out hit2, 5000))
            {
                if (hit2.transform.GetComponent<Seeker>() != null || hit2.transform.GetComponent<Player>() != null)
                    clickedObject = hit2.transform.gameObject;
                            
                foreach (GameObject teamMate in teamMateList)
                {
                    if (teamMate != null)
                    {
                        if (teamMate.GetComponent<Trooper>() != null)
                        {
                            if (teamMate.GetComponent<Trooper>().currentWeapon != null)
                            {
                                teamMate.GetComponent<Trooper>().currentWeapon.GetComponent<Weapon>().playerFollowingCommand = true;
                                StartCoroutine(cancelIgnoringEnemies(teamMate.GetComponent<Trooper>().currentWeapon.gameObject));
                            }

                            teamMate.GetComponent<Trooper>().targetToChase = clickedObject;

                            if (hit2.transform.GetComponent<Terrain>() != null)
                                teamMate.GetComponent<Trooper>().pointToChase = hit2.point;
                            else
                                teamMate.GetComponent<Trooper>().pointToChase = new Vector3(0, 0, 0);

                            teamMate.GetComponent<Trooper>().targetToChaseByPlayerCommand = clickedObject;
                            teamMate.GetComponent<Trooper>().wait = false;
                            teamMate.GetComponent<Trooper>().enemyToLook = null;
                        }
                    }
                }

                if (teamMateList.Count > 0)
                {
                    int Rnd = Random.Range(0, 2);

                    switch (Rnd)
                    {
                        case 0:
                            voice1.Play();
                            break;
                        case 1:
                            voice2.Play();
                            break;
                    }
                }
            }

            // Убираем метки с юнитов
            foreach (GameObject teamMate in teamMateList)
            {
                if (teamMate != null && teamMate.GetComponent<Trooper>() != null && teamMate.GetComponent<Trooper>().teamSelectMark != null)
                    Destroy(teamMate.GetComponent<Trooper>().teamSelectMark);
            }

            GameMaster.GM.myCamera.GetComponent<Camera>().fieldOfView = 45f;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            stopCamControls = false;            
        }
            
    }
    
    public void CallBarracsConstruction()
    {
        SpendMoneyMethod spendOnBarracsDelegate = GameMaster.GM.shipObjectList[0].GetComponent<Ship>().startBarracsConstruction;
        GameMaster.GM.shipObjectList[0].GetComponent<Ship>().spendMoney(2000, spendOnBarracsDelegate);
    }

    public void CallFactoryConstruction()
    {
        SpendMoneyMethod spendOnFactoryDelegate = GameMaster.GM.shipObjectList[0].GetComponent<Ship>().startFactoryConstruction;
        GameMaster.GM.shipObjectList[0].GetComponent<Ship>().spendMoney(3000, spendOnFactoryDelegate);
    }

    public void CallCreatingTrooper()
    {
        SpendMoneyMethod spendOnTrooperDelegate = GameMaster.GM.shipObjectList[0].GetComponent<Ship>().startCreatingTrooper;
        GameMaster.GM.shipObjectList[0].GetComponent<Ship>().spendMoney(300, spendOnTrooperDelegate);
    }

    public void CallCreatingLightShip()
    {
        SpendMoneyMethod spendOnLightShipDelegate = GameMaster.GM.shipObjectList[0].GetComponent<Ship>().startCreatingLightShip;
        GameMaster.GM.shipObjectList[0].GetComponent<Ship>().spendMoney(600, spendOnLightShipDelegate);
    }

    public void CallCreatingTower()
    {
        SpendMoneyMethod spendOnTowerDelegate = GameMaster.GM.shipObjectList[0].GetComponent<Ship>().startCreatingTower;
        GameMaster.GM.shipObjectList[0].GetComponent<Ship>().spendMoney(600, spendOnTowerDelegate);
    }

    public void CallCreatingGunTower()
    {
        SpendMoneyMethod spendOnGunTowerDelegate = GameMaster.GM.shipObjectList[0].GetComponent<Ship>().startCreatingGunTower;
        GameMaster.GM.shipObjectList[0].GetComponent<Ship>().spendMoney(600, spendOnGunTowerDelegate);
    }
    
    IEnumerator cancelIgnoringEnemies (GameObject weaponToControl)
    {
        yield return new WaitForSeconds(10);
        if (weaponToControl != null)
        weaponToControl.GetComponent<Weapon>().playerFollowingCommand = false;
    }

    public void FixedUpdate()
    {
        if (Input.GetKey("w"))
        {
            RB.AddRelativeForce(Vector3.forward * 2f, ForceMode.VelocityChange);
            fire.Play();
            fire2.Play();
        }
        else
        {
            fire.Stop();
            fire2.Stop();
        }   

        if (Input.GetKey("s"))
        {    
            RB.AddRelativeForce(-Vector3.forward * 1.5f, ForceMode.VelocityChange);
            fire5.Play();
            fire6.Play();
        }
        else
        {
            fire5.Stop();
            fire6.Stop();
        }

        if (Input.GetKeyUp("s"))
        {     
            fire5.Stop();
            fire6.Stop();
        }

        if (Input.GetKey("d"))
        {     
            RB.AddRelativeForce(Vector3.right * 1.5f, ForceMode.VelocityChange);
            fire3.Play();
        }
        else
        {
            fire3.Stop();
        }
            
        if (Input.GetKey("a"))
        {           
            RB.AddRelativeForce(-Vector3.right * 1.5f, ForceMode.VelocityChange);
            fire4.Play();
        }
        else
        {
            fire4.Stop();
        }
            
        if (Input.GetKey("q"))
        {     
            RB.AddRelativeForce(Vector3.up * (-1.5f), ForceMode.VelocityChange);
            fire7.Play();
            fire8.Play();
        }
        else
        {
            fire7.Stop();
            fire8.Stop();
        }
             
        if (Input.GetKey("e"))
        {      
            //if (transform.position.y < 20)
            // if (spectatorMode == true)
            RB.AddRelativeForce(Vector3.up * 1.5f, ForceMode.VelocityChange);

            fire9.Play();
            fire10.Play();
        }
        else
        {
            fire9.Stop();
            fire10.Stop();
        }
               
        if (Input.GetKey("x"))
        {
            GameMaster.GM.myCamera.GetComponent<Camera>().fieldOfView -= 0.8f;
        }

        if (Input.GetKey("c"))
        {
            GameMaster.GM.myCamera.GetComponent<Camera>().fieldOfView = 45f;
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
