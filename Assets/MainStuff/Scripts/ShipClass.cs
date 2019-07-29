using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DeadDelegate();

public class ShipClass : SeekerClass
{
    public event DeadDelegate shipIsDeadEvent = null;

    float timer;
    public float money;
    public List<GameObject> fractionBarracsList;
    public AudioSource insufficientFunds;

    public override void Awake()
    {
        money = 5000;
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
        healthBarScaleMultiplier = 100;

        healthBar.transform.localScale = new Vector3(health / 30000 * healthBarScaleMultiplier, 10, 10);
       // TextHP.transform.localScale = new Vector3(200, 200, 200);
        
        textHP.transform.localPosition = new Vector3(0, 1000, 400);
        healthBar.transform.localPosition = new Vector3(0, 500, 400);

        if (gameObject.name == "Base")
            healthBar.transform.localScale = new Vector3(0, 0, 0);
    }

    public void Start()
    {
        textHP.GetComponent<TextMesh>().color = healthBar.GetComponent<SpriteRenderer>().color;
        textHP.GetComponent<TextMesh>().fontSize = 355;
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

        if (fractionId != 0)
        {
            if (fractionBarracsList.Count > 0 && CountFractionWarriors(fractionId) <= fractionBarracsList.Count * 20 && money > 600)
            {
                SpendMoneyMethod spendOnTrooperDelegate = startCreatingTrooper;
                SpendMoneyMethod spendOnLightShipDelegate = startCreatingLightShip;
                rndUnit = Random.Range(0, 100);

                if (rndUnit < 30)
                    spendMoney(600, startCreatingLightShip);
                else
                    spendMoney(300, startCreatingLightShip);
            }

            if (fractionBarracsList.Count < 4 && money > 3000)
            {
                SpendMoneyMethod SpendOnBarracsDelegate = startBarracsConstruction;
                SpendMoneyMethod SpendOnFactoryDelegate = startFactoryConstruction;
                rndBuilding = Random.Range(0, 100);
                
                if (rndBuilding < 50)
                    spendMoney(2000, SpendOnBarracsDelegate);
                else 
                {
                    spendMoney(3000, SpendOnFactoryDelegate);
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

    public void CreateBuilding(GameObject buildingPrefabObject, string buildingName)
    {
        GameObject newBuilding = null;
        if (gameObject != null)
        {
            if (fractionId != 0)
            {
                newBuilding = GameMaster.GM.ConstructObject(buildingPrefabObject, GameMaster.GM.shipObjectList[fractionId].transform.position
                 + new Vector3(Random.Range(-100, 100), -GameMaster.GM.shipObjectList[fractionId].transform.position.y, Random.Range(-100, 100)),
                  Quaternion.Euler(0, 0, 0), buildingName, GameMaster.GM.trooperBaseList);
            }

            if (fractionId == 0)
                newBuilding = GameMaster.GM.ConstructObject(buildingPrefabObject, GameMaster.GM.player.TransformPoint(0, 0, 50000),
                 Quaternion.Euler(0, 0, 0), "Barracs", GameMaster.GM.trooperBaseList);

            //newBuilding.transform.position = new Vector3(newBuilding.transform.position.x, 0, newBuilding.transform.position.z);

            newBuilding.GetComponent<FractionIndexClass>().SetFractionId(fractionId);
            fractionBarracsList.Add(newBuilding);
        }
    }

    public void CreateUnit(int UnitCount, GameObject unitPrefab, string UnitName)
    {
        if (GameMaster.GM.shipObjectList[fractionId].gameObject != null)
        {
            for (int j = 0; j < UnitCount; j++)
            {
                GameObject createdObject = null;
                int rndBaseSpawnTrooper = Random.Range(0, fractionBarracsList.Count);

                if (fractionBarracsList.Count > 0)
                    if (fractionBarracsList[rndBaseSpawnTrooper].gameObject != null)
                    {
                        createdObject = GameMaster.GM.ConstructObject(unitPrefab, fractionBarracsList[rndBaseSpawnTrooper].transform.position + new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50)), Quaternion.Euler(0, 0, 0), UnitName, GameMaster.GM.unitList);
                        createdObject.GetComponent<FractionIndexClass>().SetFractionId(fractionId);
                        Vector3 warriorPosition = createdObject.transform.position + new Vector3(0, 2, 0);
                        GameMaster.GM.GiveWeaponToObject(warriorPosition);

                        createdObject.GetComponent<TrooperClass>().targetIsShip = false;
                        createdObject.GetComponent<TrooperClass>().targetToChase = GameMaster.GM.shipObjectList[ChooseRandomTarget(GameMaster.GM.shipObjectList)];

                        if (createdObject.GetComponent<FractionIndexClass>().fractionId != 0)
                            AttackPlayerWithProbability(30, createdObject);
                        
                        createdObject.GetComponent<TrooperClass>().targetToChaseByPlayerCommand = createdObject.GetComponent<TrooperClass>().targetToChase;
                    }
            }
        }
    }

    public int ChooseRandomTarget(List<GameObject> ListToSearchEnemy)
    {
        int rndTarget = fractionId;

        while (rndTarget == fractionId)
            rndTarget = Random.Range(0, GameMaster.GM.mainBaseCount);

        return rndTarget;
    }

    public void AttackPlayerWithProbability(int choosenProbability, GameObject attacker)
    {
        int rndPlayerAttack = Random.Range(0, 100);

        if (rndPlayerAttack < choosenProbability)
        {
            attacker.GetComponent<TrooperClass>().targetToChase = GameMaster.GM.player.transform.gameObject;
            attacker.GetComponent<TrooperClass>().lootAfterDeath = true;
        }
            
    }

    public int CountFractionWarriors(int fractionId)
    {
        int warriorsCount = 0;

        foreach (GameObject Warrior in GameMaster.GM.unitList)
        {
            if (Warrior != null && Warrior.GetComponent<FractionIndexClass>().fractionId == fractionId)
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
                if (Barracs.gameObject != null && Barracs.gameObject.name == barracsName && Barracs.GetComponent<FractionIndexClass>().fractionId == fractionId)
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
               
            healthBar.transform.localScale = new Vector3(health / 100000 * healthBarScaleMultiplier, 10, 10);
            textHP.gameObject.GetComponent<TextMesh>().text = GetComponent<SeekerClass>().health.ToString();
        }
        else
        {
            if (dead == false)
            {
                CheckForDeadShip();
                healthBar.transform.localScale = new Vector3(0, 0, 0);
                //smoke = Instantiate(GameMaster.GM.smokeAfterDeath, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
                //smoke.transform.parent = gameObject.transform;
                //GameObject Explode = Instantiate(GameMaster.GM.enemyDestroy, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
                //Explode.transform.localScale += new Vector3(200, 200, 200);
                //GameObject SmokeObj = Instantiate(smoke, gameObject.transform.position + new Vector3(0, -70, 0), Quaternion.Euler(0, 0, 0));
                //SmokeObj.transform.localScale += new Vector3(100, 100, 100);
                //SmokeObj.transform.parent = gameObject.transform;
            }

            dead = true;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
            totallyDead = true;
            GameMaster.GM.RecursiveDestroy(transform, gameObject, 3);
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

        if (fractionId == 0)
            GameMaster.GM.playerMoneyText.text = "Player: " + money.ToString() + "$";

        for (int i = 1; i < GameMaster.GM.shipObjectList.Count; i++)
        {
            if (GameMaster.GM.shipObjectList[i] != null)
                GameMaster.GM.enemyMoneyText.text +=
                 "Enemy #" + i.ToString() + ": " + GameMaster.GM.shipObjectList[i].GetComponent<ShipClass>().money.ToString() + " $" + "\n";
        }
    }

    public void spendMoney(float howMuchToSpend, SpendMoneyMethod creatingMethod)
    {
        if (money >= howMuchToSpend)
        {
            money -= howMuchToSpend;
            creatingMethod();
        }
        else if (fractionId == 0)
            insufficientFunds.Play();

        GameMaster.GM.enemyMoneyText.text = "";

        if (fractionId == 0)
            GameMaster.GM.playerMoneyText.text = "Player: " + money.ToString() + "$";

        for (int i = 1; i < GameMaster.GM.shipObjectList.Count; i++)
        {
            if (GameMaster.GM.shipObjectList[i] != null)
                GameMaster.GM.enemyMoneyText.text +=
                 "Enemy #" + i.ToString() + ": " + GameMaster.GM.shipObjectList[i].GetComponent<ShipClass>().money.ToString() + " $" + "\n";
        }
    }

    public void OnCollisionEnter(Collision collisioninfo)
    {
        
    }

    public override void FindItemsAround(List<GameObject> _GlobalObjectList, List<GameObject> _PlatformList)
    {

    }
    
}
