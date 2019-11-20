using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public delegate void SpendMoneyMethod();

public class PlayerClass: FractionIndexClass
{
    public static PlayerClass mainPlayer;

    public Rigidbody RB;
    public WeaponClass currentWeapon;

    public bool alreadyHaveWeapon;
    public bool reload;
    public bool tacticMode;
    public bool deadPlayer;
    public bool soundAlreadyPlaying;
    public bool targetLockedIn;

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
    public GameObject targetUI;
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
    public Transform lookCube;
    public float fx, fy, fz;
    public TextMesh playerHealth3dText;

    public bool spectatorMode;
    public bool isTimeToGiveCommand;

    public void Start()
    {
        playerHealth3dText.text = $"HP: {health}";
        //GameMaster.GM.MyCamera.transform.localPosition = new Vector3(CamX - 140, CamY - 140, CamZ - 140);
    }
    
    public override void TakeDamage(float damage)
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
                playerDeath.Play();
                missionFailed.Play();
            }

            deadPlayer = true;
        }

        GameMaster.GM.playerHp.text = "PlayerHP: " + PlayerClass.mainPlayer.health.ToString();
        playerHealth3dText.text = $"HP: {health}";
    }

    public void Heal (float healpoints)
    {
        health += healpoints;
        GameMaster.GM.playerHp.text = "PlayerHP: " + PlayerClass.mainPlayer.health.ToString();
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
        health = 30000;
        tickWeapon = 1;
        timer = 0;
        playerCubesCount = 0;
        playerMoney = 0;
        fractionId = 0;
        targetSpritePrefab.transform.localScale = new Vector3(2f, 2f, 2f);
        selector = Instantiate(GameMaster.GM.teamSelect, transform.position + new Vector3(0, 5, 0), Quaternion.Euler(0, 0, 0));
        // selector.transform.SetParent(gameObject.transform);
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
            selector.transform.localScale = new Vector3(selectorScale, selectorScale, 1);
            tacticMode = true;
            lookCube.transform.localPosition = new Vector3(0, 4000, 30000);
             GameMaster.GM.myCamera.transform.localPosition = Vector3.Lerp(GameMaster.GM.myCamera.transform.localPosition, new Vector3(0, 14000, -40000),0.05f);
        }
        
        // Если включен тактический режим
        if (tacticMode == true)
        {
            RaycastHit groundDistance; ;
            Physics.Raycast(transform.position, transform.forward, out groundDistance, 2000);

            float playerHeightTerrain = Terrain.activeTerrain.SampleHeight(transform.position);
            float selectorDistance = Mathf.Sqrt(Mathf.Pow(groundDistance.distance, 2) - Mathf.Pow(playerHeightTerrain, 2));
           // selector.transform.position = transform.position + transform.forward * selectorDistance;
           // selector.transform.position = new Vector3(selector.transform.position.x, 1, selector.transform.position.z);
            selector.transform.position = new Vector3(gameObject.transform.position.x, 1, gameObject.transform.position.z);
           // selector.transform.eulerAngles = new Vector3(90, 0, 0);

            foreach (GameObject teamMate in teamMateList)
            {
                if (teamMate != null && teamMate.GetComponent<TrooperClass>() != null && teamMate.GetComponent<TrooperClass>().teamSelectMark != null)
                    Destroy(teamMate.GetComponent<TrooperClass>().teamSelectMark);
            }

            teamMateList.Clear();

            Collider[] team = Physics.OverlapSphere(selector.transform.position, selectorScale / 1.5f, 1 << 8);
            //Collider[] team = Physics.OverlapBox(selector.transform.position, new Vector3(selectorScale / 1.5f, 9999, selectorScale / 1.5f), Quaternion.identity, 1 << 8);

            foreach (Collider teamMate in team)
                if (teamMate.tag == "Trooper" && teamMate.gameObject.GetComponent<FractionIndexClass>().fractionId == 0)
                {
                    teamMateList.Add(teamMate.gameObject);
                    teamMate.GetComponent<TrooperClass>().teamSelectMark = Instantiate(GameMaster.GM.teamMarker, teamMate.transform.position + new Vector3(0, 12, 0), Quaternion.Euler(0, 0, 0));
                    teamMate.GetComponent<TrooperClass>().teamSelectMark.transform.position = new Vector3(teamMate.GetComponent<TrooperClass>().transform.position.x, 0.2f, teamMate.GetComponent<TrooperClass>().transform.position.z);
                    teamMate.GetComponent<TrooperClass>().teamSelectMark.transform.Rotate(90, 0, 0);
                    teamMate.GetComponent<TrooperClass>().teamSelectMark.transform.localScale = new Vector3(4, 4, 4);
                    teamMate.GetComponent<TrooperClass>().teamSelectMark.transform.SetParent(teamMate.gameObject.transform);
                }

            CheckTacticMode();
        }
        
        // Наведение на цель для оружия
        if (Physics.BoxCast(GameMaster.GM.player.transform.TransformPoint(0, 0, 0), new Vector3(0, 20, 20), transform.forward, out RaycastHit hitInfo2, Quaternion.Euler(0, 0, 0), 300, 1 << 8))
        {
            if (hitInfo2.transform.gameObject != null && hitInfo2.transform.GetComponent<SeekerClass>() != null && hitInfo2.transform.GetComponent<ShipClass>() == null && hitInfo2.transform.GetComponent<SeekerClass>().dead == false)
            {
                if (hitInfo2.transform.GetComponent<FractionIndexClass>().fractionId != 0)
                {
                    targetLockedIn = true;
                    targetToLock = hitInfo2.transform.gameObject;

                    if (hitInfo2.transform.name == "LightShip")
                        targetSpritePrefab.transform.position = hitInfo2.transform.position + new Vector3(0, 0, 0);
                    else
                        targetSpritePrefab.transform.position = hitInfo2.transform.position + new Vector3(0, 3, 0);

                    targetSpritePrefab.transform.LookAt(Camera.main.transform.position, -Vector3.up);
                        
                    if (hitInfo2.transform.GetComponent<FractionIndexClass>().fractionId == 0)
                        targetSpritePrefab.GetComponent<SpriteRenderer>().color = Color.green;
                    else
                        targetSpritePrefab.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
            else if (hitInfo2.transform.gameObject != null && hitInfo2.transform.GetComponent<SeekerClass>() != null && hitInfo2.transform.GetComponent<SeekerClass>().dead == true)
            {
                targetLockedIn = false;
                targetSpritePrefab.transform.position = new Vector3(0, 1000, 0);
            }
        }
        else
        {
            targetLockedIn = false;
            targetSpritePrefab.transform.position = new Vector3(0, 1000, 0);
        }

        if (spectatorMode == true)
        {
            transform.RotateAround(new Vector3(0, 0, 0), transform.up, 2f * Time.deltaTime);
            transform.LookAt(new Vector3(0, 0, 0));
            // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(0, 0, 0)), Time.deltaTime);
        }
        
        //Vector3 MoveSin = new Vector3(transform.position.x + Mathf.Sin(timer * 1f) * 0.03f, transform.position.y + Mathf.Sin(timer * 2f) * 0.07f,
        //    transform.position.z + Mathf.Sin(timer * 3f) * 0.03f);

         Vector3 MoveSin = new Vector3(transform.position.x + Mathf.Sin(timer * 1f) * 0.03f, Terrain.activeTerrain.SampleHeight(transform.position) + Mathf.Sin(timer * 4f) * 0.2f + 8,
            transform.position.z + Mathf.Sin(timer * 1f) * 0.03f);
        
        gameObject.GetComponent<Rigidbody>().MovePosition(MoveSin);

        camX = GameMaster.GM.myCamera.transform.localPosition.x;
        camY = GameMaster.GM.myCamera.transform.localPosition.y;
        camZ = GameMaster.GM.myCamera.transform.localPosition.z;

        if (tacticMode == false)
        {
            GameMaster.GM.myCamera.transform.localPosition = new Vector3(camX + Mathf.Sin(timer * fx) * 2f, camY + Mathf.Sin(timer * fy) * 1f,
            camZ + Mathf.Sin(timer * fz) * 2f);
        }

        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 40, 1 << 9);

        foreach (Collider hit in colliders)
            if (hit.tag == "Follower" && dead == false)
            {
                Vector3 normalizeDirection = (gameObject.transform.position - hit.transform.position).normalized;
                hit.transform.position += normalizeDirection * Time.deltaTime * hit.GetComponent<Follower>().speed * 2;
            }

        timer += Time.deltaTime;

        waitForCommand();
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
                if (hit.transform.GetComponent<SeekerClass>() != null || hit.transform.GetComponent<PlayerClass>() != null)
                {
                    targetUI.GetComponent<Image>().enabled = true;
                    targetUI.GetComponent<Image>().transform.position = Input.mousePosition;

                    if (hit.transform.GetComponent<FractionIndexClass>().fractionId != 0)
                    {

                        targetUI.GetComponent<Image>().color = Color.red;
                    }
                    else
                        targetUI.GetComponent<Image>().color = Color.green;
                }
                else
                    targetUI.GetComponent<Image>().enabled = false;
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

            if (Physics.Raycast(ray2, out hit2, 5000))
            {
                if (hit2.transform.GetComponent<SeekerClass>() != null || hit2.transform.GetComponent<PlayerClass>() != null)
                    clickedObject = hit2.transform.gameObject;

                foreach (GameObject teamMate in teamMateList)
                {
                    if (teamMate != null)
                    {
                        if (teamMate.GetComponent<TrooperClass>() != null)
                        {
                            if (teamMate.GetComponent<TrooperClass>().currentWeapon != null)
                            {
                                teamMate.GetComponent<TrooperClass>().currentWeapon.GetComponent<WeaponClass>().playerFollowingCommand = true;
                                StartCoroutine(cancelIgnoringEnemies(teamMate.GetComponent<TrooperClass>().currentWeapon.gameObject));
                            }

                            teamMate.GetComponent<TrooperClass>().targetToChase = clickedObject;
                            teamMate.GetComponent<TrooperClass>().targetToChaseByPlayerCommand = clickedObject;
                            teamMate.GetComponent<TrooperClass>().wait = false;
                            teamMate.GetComponent<TrooperClass>().enemyToLook = null;
                        }

                    }

                }

                if (teamMateList.Count > 0)
                {
                    int Rnd = Random.Range(0, 3);

                    switch (Rnd)
                    {
                        case 0:
                            voice1.Play();
                            break;
                        case 1:
                            voice2.Play();
                            break;
                        case 2:
                            voice3.Play();
                            break;
                    }
                }
            }

            // Убираем метки с юнитов
            foreach (GameObject teamMate in teamMateList)
            {
                if (teamMate != null && teamMate.GetComponent<TrooperClass>() != null && teamMate.GetComponent<TrooperClass>().teamSelectMark != null)
                    Destroy(teamMate.GetComponent<TrooperClass>().teamSelectMark);

            }

            lookCube.transform.localPosition = new Vector3(0, 408, 1500);
            GameMaster.GM.myCamera.transform.localPosition = new Vector3(cameraX, cameraY, cameraZ);
        }
    }


    public void CallBarracsConstruction()
    {
        SpendMoneyMethod spendOnBarracsDelegate = GameMaster.GM.shipObjectList[0].GetComponent<ShipClass>().startBarracsConstruction;
        GameMaster.GM.shipObjectList[0].GetComponent<ShipClass>().spendMoney(2000, spendOnBarracsDelegate);
    }

    public void CallFactoryConstruction()
    {
        SpendMoneyMethod spendOnFactoryDelegate = GameMaster.GM.shipObjectList[0].GetComponent<ShipClass>().startFactoryConstruction;
        GameMaster.GM.shipObjectList[0].GetComponent<ShipClass>().spendMoney(3000, spendOnFactoryDelegate);
    }

    public void CallCreatingTrooper()
    {
        SpendMoneyMethod spendOnTrooperDelegate = GameMaster.GM.shipObjectList[0].GetComponent<ShipClass>().startCreatingTrooper;
        GameMaster.GM.shipObjectList[0].GetComponent<ShipClass>().spendMoney(300, spendOnTrooperDelegate);
    }

    public void CallCreatingLightShip()
    {
        SpendMoneyMethod spendOnLightShipDelegate = GameMaster.GM.shipObjectList[0].GetComponent<ShipClass>().startCreatingLightShip;
        GameMaster.GM.shipObjectList[0].GetComponent<ShipClass>().spendMoney(600, spendOnLightShipDelegate);
    }

    public void CallCreatingTower()
    {
        SpendMoneyMethod spendOnTowerDelegate = GameMaster.GM.shipObjectList[0].GetComponent<ShipClass>().startCreatingTower;
        GameMaster.GM.shipObjectList[0].GetComponent<ShipClass>().spendMoney(600, spendOnTowerDelegate);
    }

    public void CallCreatingGunTower()
    {
        SpendMoneyMethod spendOnGunTowerDelegate = GameMaster.GM.shipObjectList[0].GetComponent<ShipClass>().startCreatingGunTower;
        GameMaster.GM.shipObjectList[0].GetComponent<ShipClass>().spendMoney(600, spendOnGunTowerDelegate);
    }


    IEnumerator cancelIgnoringEnemies (GameObject weaponToControl)
    {
        yield return new WaitForSeconds(10);
        if (weaponToControl != null)
        weaponToControl.GetComponent<WeaponClass>().playerFollowingCommand = false;
    }

    public void FixedUpdate()
    {
        if (transform.position.y > 20)
            transform.position = new Vector3(transform.position.x, 20, transform.position.z);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyUp("0"))
        {
            foreach (GameObject teamMate in teamMateList)
            {
                if (teamMate != null)
                {
                    if (teamMate.GetComponent<TrooperClass>() != null)
                        teamMate.GetComponent<TrooperClass>().attackTargetId = 0;
                }
            }

            int Rnd = Random.Range(0, 3);
            switch (Rnd)
            {
                case 0:
                    voice1.Play();
                    break;
                case 1:
                    voice2.Play();
                    break;
                case 2:
                    voice3.Play();
                    break;
            }
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
            currentWeapon.GetComponent<Transform>().position = gameObject.transform.position + new Vector3(5, 0, 5);
            currentWeapon.GetComponent<WeaponClass>().tag = "WithoutUser";
            currentWeapon.transform.parent = null;
            currentWeapon.objectToStick = null;
            currentWeapon.GetComponent<WeaponClass>().tick = 0f;
            currentWeapon.GetComponent<WeaponClass>().timer = 0;
            currentWeapon.GetComponent<WeaponClass>().reloadNow = true;
            currentWeapon.GetComponent<WeaponClass>().soundStop = false;
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

        if (Input.GetKey("w"))
        {
            if (gameObject.GetComponent<AudioSource>().isPlaying == false)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }

            RB.AddRelativeForce(Vector3.forward * 1.5f, ForceMode.VelocityChange);
            fire.Play();
            fire2.Play();
        }
        else
        {
            fire.Stop();
            fire2.Stop();
        }

        if (Input.GetKeyUp("w"))
        {
            if (gameObject.GetComponent<AudioSource>().isPlaying == true)
                gameObject.GetComponent<AudioSource>().Stop();
        }

        if (Input.GetKey("s"))
        {
            if (gameObject.GetComponent<AudioSource>().isPlaying == false)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }

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
            if (gameObject.GetComponent<AudioSource>().isPlaying == true)
                gameObject.GetComponent<AudioSource>().Stop();

            fire5.Stop();
            fire6.Stop();
        }

        if (Input.GetKey("d"))
        {
            if (gameObject.GetComponent<AudioSource>().isPlaying == false)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }

            RB.AddRelativeForce(Vector3.right * 1.5f, ForceMode.VelocityChange);
            fire3.Play();
        }
        else
        {
            fire3.Stop();
        }

        if (Input.GetKeyUp("d"))
        {
            if (gameObject.GetComponent<AudioSource>().isPlaying == true)
                gameObject.GetComponent<AudioSource>().Stop();
        }

        if (Input.GetKey("a"))
        {
            if (gameObject.GetComponent<AudioSource>().isPlaying == false)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }

            RB.AddRelativeForce(-Vector3.right * 1.5f, ForceMode.VelocityChange);
            fire4.Play();
        }
        else
        {
            fire4.Stop();
        }

        if (Input.GetKeyUp("a"))
        {
            if (gameObject.GetComponent<AudioSource>().isPlaying == true)
                gameObject.GetComponent<AudioSource>().Stop();
        }

        if (Input.GetKey("q"))
        {
            if (gameObject.GetComponent<AudioSource>().isPlaying == false)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }

            
            RB.AddRelativeForce(Vector3.up * (-1.5f), ForceMode.VelocityChange);
            fire7.Play();
            fire8.Play();
        }
        else
        {
            fire7.Stop();
            fire8.Stop();
        }

        if (Input.GetKeyUp("q"))
        {
            if (gameObject.GetComponent<AudioSource>().isPlaying == true)
                gameObject.GetComponent<AudioSource>().Stop();
        }

        if (Input.GetKey("e"))
        {
            if (gameObject.GetComponent<AudioSource>().isPlaying == false)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }

            if (transform.position.y < 20)
                RB.AddRelativeForce(Vector3.up * 1.5f, ForceMode.VelocityChange);
            
            fire9.Play();
            fire10.Play();
        }
        else
        {
            fire9.Stop();
            fire10.Stop();
        }

        if (Input.GetKeyUp("e"))
        {
            if (gameObject.GetComponent<AudioSource>().isPlaying == true)
                gameObject.GetComponent<AudioSource>().Stop();
        }
    }
}
