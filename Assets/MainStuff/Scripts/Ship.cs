using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DeadDelegate();

public class Ship : Seeker
{
    public event DeadDelegate shipIsDeadEvent = null;

    float timer;
    public float money;
    public List<GameObject> fractionBarracsList;
    public List<GameObject> fractionFactoryList;
    public List<GameObject> fractionTowerList;
    public AudioSource insufficientFunds;

    public override void Awake()
    {
        money = 200000 * GameMaster.GM.startMoney / 100;

        health = 200000;
        maxHP = 200000;
        gameObject.tag = "Ship";
        countOfItemsCollected = 0;
        isVulnerable = true;
        textHP = Instantiate(GameMaster.GM.text3dDamage, transform.position + new Vector3(0, 7, 0), Quaternion.Euler(0, 0, 0));
        textHP.gameObject.GetComponent<TextMesh>().text = health.ToString();
        textHP.transform.parent = transform;
        
        healthBar = Instantiate(GameMaster.GM.healthBar, transform.position + new Vector3(0, 12, 0), Quaternion.Euler(0, 0, 0));
        healthBar.transform.SetParent(gameObject.transform);
        healthBar.transform.eulerAngles = transform.eulerAngles - new Vector3 (0,90,0);
        healthBarScaleMultiplier = 300;

        healthBar.transform.localScale = new Vector3(health / maxHP * healthBarScaleMultiplier, 10, 10);
        textHP.transform.localScale = new Vector3(10, 10, 10);
        
        textHP.transform.localPosition = new Vector3(0, 1000, 400);
        healthBar.transform.localPosition = new Vector3(0, 500, 400);

        if (gameObject.name == "Base")
            healthBar.transform.localScale = new Vector3(0, 0, 0);
    }

    public void Start()
    {
        textHP.GetComponent<TextMesh>().color = healthBar.GetComponent<SpriteRenderer>().color;
        textHP.GetComponent<TextMesh>().fontSize = 355;

        UpdateMoneyStats();
    }

    public void FixedUpdate()
    {
        MoveShip();
        EnemyAI();
    }
    
    public void CheckForDeadShip ()
    {
        if (shipIsDeadEvent != null)
            shipIsDeadEvent.Invoke();
    }

    public void EnemyAI()
    {
        int rndUnit;
        int rndBuilding;

        if (GameMaster.GM.aiModeOnly == false && factionId != 0 || GameMaster.GM.aiModeOnly == true)
        {            
            {
                if (fractionBarracsList.Count > 0 && CountFractionWarriors(factionId) <= fractionBarracsList.Count * 20 && money > 600)
                {
                    //SpendMoneyMethod spendOnTrooperDelegate = startCreatingTrooper;
                    //SpendMoneyMethod spendOnLightShipDelegate = startCreatingLightShip;
                    rndUnit = Random.Range(0, 100);

                    if (rndUnit < 70)
                        spendMoney(300, startCreatingTrooper);
                }

                if (fractionFactoryList.Count > 0 && CountFractionWarriors(factionId) <= fractionFactoryList.Count * 30 && money > 600)
                {
                    //SpendMoneyMethod spendOnTrooperDelegate = startCreatingTrooper;
                    //SpendMoneyMethod spendOnLightShipDelegate = startCreatingLightShip;
                    rndUnit = Random.Range(0, 100);

                    if (rndUnit < 70)
                        spendMoney(600, startCreatingLightShip);
                }

                if (fractionBarracsList.Count < 3 && money > 3000)
                {
                    //SpendMoneyMethod SpendOnBarracsDelegate = startBarracsConstruction;
                    //SpendMoneyMethod SpendOnFactoryDelegate = startFactoryConstruction;
                    rndBuilding = Random.Range(0, 100);

                    if (rndBuilding < 50)
                        spendMoney(2000, startBarracsConstruction);
                    else
                    {
                        spendMoney(3000, startFactoryConstruction);
                    }
                }

                if (fractionBarracsList.Count > 0 && money > 5000 && fractionBarracsList.Count < 3)
                {
                    SpendMoneyMethod SpendOnTowerDelegate = startCreatingTower;

                    rndBuilding = Random.Range(0, 100);

                    spendMoney(5000, SpendOnTowerDelegate);

                }
            }     
        }
    }

    public void startBarracsConstruction()
    {
        CreateBuilding(GameMaster.GM.trooperBase, "Barracs");
    }

    public void startFactoryConstruction()
    {
        CreateBuilding(GameMaster.GM.factoryPrefab, "Factory");
    }

    public void startCreatingTrooper()
    {
        CreateUnit(5, GameMaster.GM.trooperPrefab, "Trooper");
    }

    public void startCreatingLightShip()
    {
        CreateUnit(5, GameMaster.GM.lightShipPrefab, "LightShip");
    }

    public void startCreatingTower()
    {
        CreateBuilding(GameMaster.GM.TowerPrefab, "Tower");
    }

    public void startCreatingGunTower()
    {
        CreateBuilding(GameMaster.GM.TowerGunPrefab, "GunTower");
    }

    public void CreateBuilding(GameObject buildingPrefabObject, string buildingName)
    {
        GameObject newBuilding = null;
        if (gameObject != null)
        {
            if (GameMaster.GM.aiModeOnly == false)
            { 
                if (factionId != 0)
                {
                    newBuilding = GameMaster.GM.ConstructObject(buildingPrefabObject, GameMaster.GM.shipObjectList[factionId].transform.position
                + new Vector3(Random.Range(-500, 500), -GameMaster.GM.shipObjectList[factionId].transform.position.y, Random.Range(-500, 500)),
                 Quaternion.Euler(0, 0, 0), buildingName, GameMaster.GM.trooperBaseList);
                }       
                else
                {                    
                    newBuilding = GameMaster.GM.ConstructObject(buildingPrefabObject, GameMaster.GM.player.TransformPoint(0, 0, 25000),
                            Quaternion.Euler(0, 0, 0), buildingName, GameMaster.GM.trooperBaseList);
                    //newBuilding = GameMaster.GM.ConstructObject(buildingPrefabObject, GameMaster.GM.player.TransformPoint(0, 0, 50000),
                    //Quaternion.Euler(0, 0, 0), "Barracs", GameMaster.GM.trooperBaseList);
                }
            }
            else
            {
                newBuilding = GameMaster.GM.ConstructObject(buildingPrefabObject, GameMaster.GM.shipObjectList[factionId].transform.position
                + new Vector3(Random.Range(-500, 500), -GameMaster.GM.shipObjectList[factionId].transform.position.y, Random.Range(-500, 500)),
                 Quaternion.Euler(0, 0, 0), buildingName, GameMaster.GM.trooperBaseList);
            }

          

            newBuilding.transform.position = new Vector3(newBuilding.transform.position.x, 0, newBuilding.transform.position.z);

            newBuilding.GetComponent<FactionIndex>().SetFractionId(factionId);
            if (newBuilding.gameObject.tag == "Barracs")
                fractionBarracsList.Add(newBuilding);
            if (newBuilding.gameObject.tag == "Factory")
                fractionFactoryList.Add(newBuilding);
        }
    }

    public void CreateUnit(int UnitCount, GameObject unitPrefab, string unitName)
    {
        if (GameMaster.GM.shipObjectList[factionId].gameObject != null)
        {
            for (int j = 0; j < UnitCount; j++)
            {
                GameObject createdObject = null;
                int rndBaseSpawnTrooper = Random.Range(0, fractionBarracsList.Count);
                int rndBaseSpawnLightShip = Random.Range(0, fractionFactoryList.Count);

                if (unitName == "Trooper")
                    if (fractionBarracsList.Count > 0)
                        if (fractionBarracsList[rndBaseSpawnTrooper].gameObject != null)
                        {
                            createdObject = GameMaster.GM.ConstructObject(unitPrefab, fractionBarracsList[rndBaseSpawnTrooper].transform.position + new Vector3(Random.Range(-120, 120), 0, Random.Range(-120, 120)), Quaternion.Euler(0, 0, 0), unitName, GameMaster.GM.unitList);
                            createdObject.GetComponent<FactionIndex>().SetFractionId(factionId);
                            Vector3 warriorPosition = createdObject.transform.position + new Vector3(0, 2, 0);
                            GameMaster.GM.GiveWeaponToObject(warriorPosition);
                            if (createdObject.GetComponent<Trooper>() != null)
                            {
                                createdObject.GetComponent<Trooper>().targetIsShip = false;
                                //createdObject.GetComponent<TrooperClass>().targetToChase = GameMaster.GM.shipObjectList[ChooseRandomTarget(GameMaster.GM.shipObjectList)];

                                if (factionId == 0 && GameMaster.GM.aiModeOnly == false)
                                    createdObject.GetComponent<Trooper>().targetToChase = GameMaster.GM.player.gameObject;

                                //createdObject.GetComponent<TrooperClass>().targetToChase = fractionBarracsList[rndBaseSpawnTrooper].gameObject;
                            }

                            //float randomScale = Random.Range(1f, 2f); // Рандомизируем размеры пехоты
                            //createdObject.transform.localScale = new Vector3(randomScale, randomScale, randomScale); // Рандомизируем размеры пехоты
                            createdObject.GetComponent<Seeker>().textHP.GetComponent<TextMesh>().color = GameMaster.GM.fractionColors[this.factionId];

                            if (createdObject.GetComponent<FactionIndex>().factionId != 0)
                                AttackPlayerWithProbability(0, createdObject);

                            if (createdObject.GetComponent<Trooper>() != null)
                                createdObject.GetComponent<Trooper>().targetToChaseByPlayerCommand = createdObject.GetComponent<Trooper>().targetToChase;  
                        }

                if (unitName == "LightShip")
                    if (fractionFactoryList.Count > 0)
                        if (fractionFactoryList[rndBaseSpawnLightShip].gameObject != null)
                        {
                            createdObject = GameMaster.GM.ConstructObject(unitPrefab, fractionFactoryList[rndBaseSpawnLightShip].transform.position + new Vector3(Random.Range(-80, 80), 0, Random.Range(-80, 80)), Quaternion.Euler(0, 0, 0), unitName, GameMaster.GM.unitList);

                            createdObject.GetComponent<FactionIndex>().SetFractionId(factionId);
                            Vector3 warriorPosition = createdObject.transform.position + new Vector3(0, 2, 0);
                            GameMaster.GM.GiveWeaponToObject(warriorPosition);
                            if (createdObject.GetComponent<Trooper>() != null)
                            {
                                createdObject.GetComponent<Trooper>().targetIsShip = false;
                                //createdObject.GetComponent<TrooperClass>().targetToChase = GameMaster.GM.shipObjectList[ChooseRandomTarget(GameMaster.GM.shipObjectList)];
                                if (factionId == 0 && GameMaster.GM.aiModeOnly == false)
                                    createdObject.GetComponent<Trooper>().targetToChase = GameMaster.GM.player.gameObject;
                                //createdObject.GetComponent<TrooperClass>().targetToChase = fractionBarracsList[rndBaseSpawnTrooper].gameObject;
                            }

                            //float randomScale = Random.Range(1f, 2f); // Рандомизируем размеры пехоты
                            //createdObject.transform.localScale = new Vector3(randomScale, randomScale, randomScale); // Рандомизируем размеры пехоты
                            createdObject.GetComponent<Seeker>().textHP.GetComponent<TextMesh>().color = GameMaster.GM.fractionColors[this.factionId];

                            if (createdObject.GetComponent<FactionIndex>().factionId != 0)
                                AttackPlayerWithProbability(0, createdObject);

                            if (createdObject.GetComponent<Trooper>() != null)
                                createdObject.GetComponent<Trooper>().targetToChaseByPlayerCommand = createdObject.GetComponent<Trooper>().targetToChase;
                        }
            }
        }
    }

    public int ChooseRandomTarget(List<GameObject> ListToSearchEnemy)
    {
        int rndTarget = factionId;

        while (rndTarget == factionId)
            rndTarget = Random.Range(0, GameMaster.GM.mainBaseCount);

        return rndTarget;
    }

    public void AttackPlayerWithProbability(int choosenProbability, GameObject attacker)
    {
        int rndPlayerAttack = Random.Range(0, 100);

        if (rndPlayerAttack < choosenProbability)
        {
            if (attacker.GetComponent<Trooper>() != null)
            attacker.GetComponent<Trooper>().targetToChase = GameMaster.GM.player.transform.gameObject;
            //attacker.GetComponent<TrooperClass>().lootAfterDeath = true;
        }
    }

    public int CountFractionWarriors(int fractionId)
    {
        int warriorsCount = 0;

        foreach (GameObject Warrior in GameMaster.GM.unitList)
        {
            if (Warrior != null && Warrior.GetComponent<FactionIndex>().factionId == fractionId)
                warriorsCount++;
        }

        return warriorsCount;
    }

    public int CountFractionBarracs(int fractionId, string barracsName)
    {
        int barracsCount = 0;

        if (GameMaster.GM.trooperBaseList != null)
            foreach (GameObject Barracs in GameMaster.GM.trooperBaseList)
            {
                if (Barracs.gameObject != null && Barracs.gameObject.name == barracsName && Barracs.GetComponent<FactionIndex>().factionId == fractionId)
                {
                    barracsCount++;
                    fractionBarracsList.Add(Barracs);
                }
            }

        return barracsCount;
    }

    public override void TakeDamage(float damage)
    {
        if (health > damage)
        {
            health -= damage;
               
            healthBar.transform.localScale = new Vector3(health / 200000 * healthBarScaleMultiplier, 10, 10);
            textHP.gameObject.GetComponent<TextMesh>().text = GetComponent<Seeker>().health.ToString();
        }
        else
        {
            if (dead == false)
            {
                CheckForDeadShip();
                healthBar.transform.localScale = new Vector3(0, 0, 0);
                smoke = Instantiate(GameMaster.GM.smokeAfterDeath, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
                smoke.transform.parent = gameObject.transform;
                GameObject Explode = Instantiate(GameMaster.GM.enemyDestroy, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
                Explode.transform.localScale += new Vector3(200, 200, 200);
                GameObject SmokeObj = Instantiate(smoke, gameObject.transform.position + new Vector3(0, -70, 0), Quaternion.Euler(0, 0, 0));
                SmokeObj.transform.localScale += new Vector3(100, 100, 100);
                SmokeObj.transform.parent = gameObject.transform;
            }

            dead = true;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
            totallyDead = true;
            GameMaster.GM.RecursiveDestroy(transform, gameObject, 3);

            for (int i = 0; i < GameMaster.GM.globalObjectList.Count; i++)
            {
                if (GameMaster.GM.globalObjectList[i] != null && GameMaster.GM.globalObjectList[i].gameObject.tag == "Seeker" && GameMaster.GM.globalObjectList[i].gameObject.GetComponent<FactionIndex>().factionId == factionId)
                {
                    GameObject Explode = Instantiate(GameMaster.GM.globalObjectList[i].gameObject.GetComponent<FactionIndex>().deathEffect, GameMaster.GM.globalObjectList[i].gameObject.transform.position, Quaternion.Euler(0, 0, 0));

                    Destroy(Explode, 2f);

                    Destroy(GameMaster.GM.globalObjectList[i].gameObject);

                    GameMaster.GM.globalObjectList.RemoveAt(i);
                }                    
            }
        }
    }

    public virtual void MoveShip()
    {
        timer += Time.deltaTime;

        if (dead == false)
        {
            transform.position += new Vector3(0, Mathf.Sin(timer * 1.5f) * 0.07f, 0);
        }

        healthBar.transform.LookAt(GameMaster.GM.player.transform.position);
        textHP.transform.LookAt(GameMaster.GM.player.transform.position);
        textHP.transform.Rotate(0, 180, 0);
    }
    
    public void EarnMoney (float coinsToEarn)
    {
        money += coinsToEarn;
        GameMaster.GM.enemyMoneyText.text = "";

        UpdateMoneyStats();
        for (int i = 1; i < GameMaster.GM.shipObjectList.Count; i++)
        {
            if (GameMaster.GM.shipObjectList[i] != null)
                GameMaster.GM.enemyMoneyText.text +=
                 "Enemy #" + i.ToString() + ": " + GameMaster.GM.shipObjectList[i].GetComponent<Ship>().money.ToString() + "\n";
        }
    }

    public void spendMoney(float howMuchToSpend, SpendMoneyMethod creatingMethod)
    {
        if (money >= howMuchToSpend)
        {
            money -= howMuchToSpend;
            creatingMethod();
        }
        else if (factionId == 0)
            insufficientFunds.Play();

        GameMaster.GM.enemyMoneyText.text = "";

        UpdateMoneyStats();

        for (int i = 1; i < GameMaster.GM.shipObjectList.Count; i++)
        {
            if (GameMaster.GM.shipObjectList[i] != null)
                GameMaster.GM.enemyMoneyText.text +=
                 "AI #" + i.ToString() + ": " + GameMaster.GM.shipObjectList[i].GetComponent<Ship>().money.ToString() + "\n";
        }
    }

    public void UpdateMoneyStats()
    {
        if (factionId == 0)
        {
            if (GameMaster.GM.aiModeOnly == false)
            {
                money = 0;
                GameMaster.GM.playerMoneyText.text = "Player: " + money.ToString();
            }
            else
            {
                GameMaster.GM.playerMoneyText.text = "AI #0: " + money.ToString();
            }
        }
    }

    public void OnCollisionEnter(Collision collisioninfo)
    {
        
    }

    public override void FindItemsAround(List<GameObject> _GlobalObjectList, List<GameObject> _PlatformList)
    {

    }
    
}
