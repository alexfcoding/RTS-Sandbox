using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public int numCubes;     
    public float spawnCircleAngle;
    public float spawnCircleAngle2;
    public Vector3 warriorPosition;
    public Vector3 randomSpawnPosition;
    public GameObject spawnPlatform;
    public Text statsText;
    void Start()
    {
        spawnPlatform = GameMaster.GM.ConstructObject(GameMaster.GM.spawnPlatform, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), "SpawnPlatform", GameMaster.GM.globalObjectList);
        InvokeRepeating("GenerateCubes", 0f, 0.3f);
        //InvokeRepeating("GenerateRollerBalls", 0f, 10f);
        GameMaster.GM.SetFractionColors();

        for (int i = 0; i < GameMaster.GM.mainBaseCount; i++)
        {
            GameObject newShipObject = GameMaster.GM.ConstructObject(GameMaster.GM.shipPrefab, new Vector3(2600 * Mathf.Cos(spawnCircleAngle * 3.14f / 180), 110, 2600 * Mathf.Sin(spawnCircleAngle * 3.14f / 180)), Quaternion.Euler(0, Random.Range(-180, 180), 0), "Seeker", GameMaster.GM.shipObjectList);
            newShipObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            newShipObject.GetComponent<ShipClass>().SetFractionId(i);
            
            if (i == 0)
            {
                GameMaster.GM.shipObjectList[0].GetComponent<ShipClass>().shipIsDeadEvent += printDeadHandler;
                //GameMaster.GM.player.transform.position = GameMaster.GM.shipObjectList[0].transform.position + new Vector3(600, -110 + 5, -100);
                GameMaster.GM.player.transform.position = GameMaster.GM.shipObjectList[0].transform.position + new Vector3(300, -110 + 5, 0);
                GameMaster.GM.player.GetComponent<PlayerClass>().playerHealth3dText.color = GameMaster.GM.fractionColors[0];                
                GameMaster.GM.ConstructObject(GameMaster.GM.machineGunPrefab, GameMaster.GM.player.transform.TransformPoint(10000, 0, -6000), Quaternion.Euler(0, 0, 0), "MachineGun", GameMaster.GM.weaponObjectList);
                GameMaster.GM.ConstructObject(GameMaster.GM.rocketLauncherPrefab, GameMaster.GM.player.transform.TransformPoint(10000, 0, -3000), Quaternion.Euler(0, 0, 0), "RocketLauncher", GameMaster.GM.weaponObjectList);
                GameMaster.GM.ConstructObject(GameMaster.GM.rocketLauncherMiniPrefab, GameMaster.GM.player.transform.TransformPoint(10000, 0, 0), Quaternion.Euler(0, 0, 0), "RocketLauncher", GameMaster.GM.weaponObjectList);
                GameMaster.GM.ConstructObject(GameMaster.GM.bombLauncherPrefab, GameMaster.GM.player.transform.TransformPoint(10000, 0, 3000), Quaternion.Euler(0, 0, 0), "BombLauncher", GameMaster.GM.weaponObjectList);
                
            }
            if (i != 0)
            {
                for (int j = 0; j < 20; j++)
                {
                    GameObject newTower = GameMaster.GM.ConstructObject(GameMaster.GM.TowerGunPrefab, new Vector3(newShipObject.transform.position.x - 700 * Mathf.Cos(spawnCircleAngle2 * 3.14f / 180), 0, newShipObject.transform.position.z - 700 * Mathf.Sin(spawnCircleAngle2 * 3.14f / 180)), Quaternion.Euler(0, Random.Range(-180, 180), 0), "GunTower", GameMaster.GM.globalObjectList);
                    newTower.GetComponent<TowerClass>().SetFractionId(newShipObject.GetComponent<ShipClass>().fractionId);
                    newTower.GetComponent<TowerClass>().isVulnerable = true;
                    newTower.GetComponent<TowerClass>().health = 5000;
                    newTower.GetComponent<TowerClass>().maxHP = 5000;
                    spawnCircleAngle2 += 360 / 20;

                }
            }                

            spawnCircleAngle += 360 / GameMaster.GM.mainBaseCount;
        }
        
        Time.timeScale = 1; 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Создаем платформу для каждого корабля
        for (int i = 0; i < GameMaster.GM.shipObjectList.Count; i++)
        {
            GameObject newPlatformObject = GameMaster.GM.ConstructObject(GameMaster.GM.platformPrefab, GameMaster.GM.shipObjectList[i].transform.position - new Vector3(0, GameMaster.GM.shipObjectList[i].transform.position.y - 20, 0), Quaternion.Euler(0, 0, 0), "Platform", GameMaster.GM.platformObjectList);
            newPlatformObject.transform.SetParent(GameMaster.GM.shipObjectList[i].transform);
            newPlatformObject.transform.localPosition = new Vector3(0, -275, -81);
            GameMaster.GM.shipObjectList[i].GetComponent<ShipClass>().EarnMoney(0);
            newPlatformObject.gameObject.GetComponent<FractionIndexClass>().healthBar.transform.localScale = new Vector3(0,0,0);
        }

        // Создаем SeekerPrefab под кораблями из GameMaster.GM.ShipObjectList с рандомной позицией
        // X,Z +- 90 от корабля с использованием функции ConstructObject и задаем имя Seeker, потом помещаем в общий список GlobalObjectList
        for (int i = 0; i < GameMaster.GM.mainBaseCount; i++)
            for (int j = 0; j < 2; j++)
            {
                GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.seekerPrefab, GameMaster.GM.shipObjectList[i].transform.position - new Vector3(0, GameMaster.GM.shipObjectList[i].transform.position.y, 0) +
                    new Vector3(Random.Range(-300, 300), 0, Random.Range(-300, 300)), Quaternion.Euler(0, 0, 0), "Seeker", GameMaster.GM.globalObjectList);

                createdObject.GetComponent<SeekerClass>().SetFractionId(i);
                createdObject.GetComponent<SeekerClass>().textHP.gameObject.GetComponent<TextMesh>().color = GameMaster.GM.fractionColors[i];

                if (createdObject.GetComponent<FractionIndexClass>().fractionId == 0)
                    createdObject.GetComponent<SeekerClass>().textHP.gameObject.GetComponent<TextMesh>().text = createdObject.GetComponent<SeekerClass>().level.ToString() + " *";
                
                warriorPosition = createdObject.transform.position + new Vector3(0, 2, 0);
                GameMaster.GM.GiveWeaponToObject(warriorPosition);
            }

        //for (int i = 0; i < 1000; i++)
        //{
        //    int rndNum = Random.Range(0, GameMaster.GM.detailsList.Count);
        //    GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.detailsList[rndNum], -100, 100, Random.Range(10, 500), Quaternion.Euler(0, 0, 0), "Follower", GameMaster.GM.globalObjectList);

        //    if (createdObject.GetComponent<Rigidbody>() == null)
        //        createdObject.AddComponent<Rigidbody>();

        //    createdObject.GetComponent<Rigidbody>().mass = 5;
        //    //createdObject.transform.localScale = new Vector3(2, 2, 2);
        //    //createdObject.GetComponent<Rigidbody>().useGravity = false;
        //    createdObject.GetComponent<Rigidbody>().angularDrag = 0.05f;
        //    createdObject.GetComponent<Rigidbody>().drag = 1f;
        //    //createdObject.GetComponent<Rigidbody>().AddForce(Random.Range(-200, 200), Random.Range(-200, 200), Random.Range(-200, 200), ForceMode.Impulse);
        //    createdObject.GetComponent<Rigidbody>().AddTorque(transform.up * Random.Range(-1000, 1000));
        //    createdObject.GetComponent<Rigidbody>().AddTorque(transform.forward * Random.Range(-1000, 1000));
        //    createdObject.GetComponent<Rigidbody>().AddTorque(transform.right * Random.Range(-1000, 1000));
        //    if (createdObject.GetComponent<MeshCollider>() != null && createdObject.GetComponent<MeshCollider>().convex == false)
        //        createdObject.GetComponent<MeshCollider>().convex = true;

        //    // GameObject Boom = Instantiate(GameMaster.GM.ExplosionPrefab, CreatedObject.transform.position, Quaternion.Euler(0, 0, 0));
        //    numCubes++;
        //}

        //for (int i = 0; i < 20; i++)
        //{
        //    int rndNum = Random.Range(0, GameMaster.GM.detailsList.Count);
        //    GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.rollerEnemyBasePrefab, -1000, 1000, Random.Range(2, 40), Quaternion.Euler(0, 0, 0), "RollerEnemyBase", GameMaster.GM.globalObjectList);

        //    //createdObject.GetComponent<FractionIndexClass>().SetFractionId(Random.Range(0, GameMaster.GM.mainBaseCount));
        //    createdObject.GetComponent<FractionIndexClass>().SetFractionId(5);
                        
        //    int rnDShip = Random.Range(0, GameMaster.GM.mainBaseCount);
           
        //   // createdObject.GetComponent<RollerEnemyBase>().targetToChase = GameMaster.GM.shipObjectList[rnDShip];
            

        //}

        //for (int i = 0; i < 0; i++) // Создаем оружие RocketLauncherPrefab, RocketLauncherMiniPrefab, MachineGunPrefab в рандомной позиции от 50 до 950 (вся карта)
        //{
        //    GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.rocketLauncherPrefab, -1000, 1000, 4, Quaternion.Euler(0, 0, 0), "RocketLauncher", GameMaster.GM.weaponObjectList);
        //    createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.rocketLauncherMiniPrefab, -1000, 1000, 4, Quaternion.Euler(0, 0, 0), "RocketLauncher", GameMaster.GM.weaponObjectList);
        //    createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.machineGunPrefab, -1000, 1000, 4, Quaternion.Euler(0, 0, 0), "MachineGun", GameMaster.GM.weaponObjectList);
        //}

        // Создаем пехоту TrooperPrefab рядом с TrooperBase
        //for (int i = 0; i < GameMaster.GM.mainBaseCount; i++)
        //    for (int j = 0; j < 10; j++)
        //    {
        //        GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.trooperPrefab, GameMaster.GM.shipObjectList[i].transform.position + new Vector3(Random.Range(-130, 130), -GameMaster.GM.shipObjectList[i].transform.position.y , Random.Range(-130, 130)), Quaternion.Euler(0, 0, 0), "Trooper", GameMaster.GM.unitList);
        //        //GameObject Boom = Instantiate(GameMaster.GM.ExplosionPrefab, CreatedObject.transform.position, Quaternion.Euler(0, 0, 0));
        //        //float randomScale = Random.Range(2f, 4f); // Рандомизируем размеры пехоты
        //        //createdObject.transform.localScale = new Vector3(randomScale, randomScale, randomScale); // Рандомизируем размеры пехоты
        //        createdObject.GetComponent<FractionIndexClass>().SetFractionId(i);
        //        createdObject.GetComponent<SeekerClass>().textHP.gameObject.GetComponent<TextMesh>().color = GameMaster.GM.fractionColors[i];

        //        if (createdObject.GetComponent<FractionIndexClass>().fractionId == 0)
        //            createdObject.GetComponent<SeekerClass>().textHP.gameObject.GetComponent<TextMesh>().text = createdObject.GetComponent<SeekerClass>().level.ToString() + " *";

        //        warriorPosition = createdObject.transform.position + new Vector3(0, 0, 0);
        //        GameMaster.GM.GiveWeaponToObject(warriorPosition);

        //        int rnDShip = Random.Range(0, GameMaster.GM.mainBaseCount);
        //        while (rnDShip == i)
        //            rnDShip = Random.Range(0, GameMaster.GM.mainBaseCount);
        //        if (i != 0)
        //            createdObject.GetComponent<TrooperClass>().targetToChase = GameMaster.GM.shipObjectList[rnDShip];
        //        else
        //            createdObject.GetComponent<TrooperClass>().targetToChase = GameMaster.GM.player.gameObject;
        //    }

        //for (int i = 0; i < GameMaster.GM.mainBaseCount; i++)
        //    for (int j = 0; j < 10; j++)
        //    {
        //        GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.lightShipPrefab, GameMaster.GM.shipObjectList[i].transform.position + new Vector3(Random.Range(-130, 130), -GameMaster.GM.shipObjectList[i].transform.position.y + 3, Random.Range(-130, 130)), Quaternion.Euler(0, 0, 0), "LightShip", GameMaster.GM.unitList);
        //        float RandomScale = Random.Range(2f, 3f); // Рандомизируем размеры пехоты
        //        //CreatedObject.transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale); // Рандомизируем размеры пехоты

        //        createdObject.GetComponent<FractionIndexClass>().SetFractionId(i);
        //        createdObject.GetComponent<SeekerClass>().textHP.gameObject.GetComponent<TextMesh>().color = GameMaster.GM.fractionColors[i];

        //        if (createdObject.GetComponent<FractionIndexClass>().fractionId == 0)
        //            createdObject.GetComponent<SeekerClass>().textHP.gameObject.GetComponent<TextMesh>().text = createdObject.GetComponent<SeekerClass>().level.ToString() + " *";
                
        //        warriorPosition = createdObject.transform.position + new Vector3(0, 2, 0);
        //        GameMaster.GM.GiveWeaponToObject(warriorPosition);
        //        int rndShip = Random.Range(0, GameMaster.GM.mainBaseCount);

        //        while (rndShip == i)
        //            rndShip = Random.Range(0, GameMaster.GM.mainBaseCount);

        //        if (i != 0)
        //            createdObject.GetComponent<TrooperClass>().targetToChase = GameMaster.GM.shipObjectList[rndShip];
        //        else
        //            createdObject.GetComponent<TrooperClass>().targetToChase = GameMaster.GM.player.gameObject;
        //    }
    }
        
    public void GenerateCubes ()
    {
        //Debug.Log("Troopers:" + GameObject.FindGameObjectsWithTag("Trooper").Length.ToString() + " Cubes: " + GameObject.FindGameObjectsWithTag("Follower").Length.ToString() + " Rockets: " + GameObject.FindGameObjectsWithTag("RocketAmmo").Length.ToString() + " TowerGun: " + GameObject.FindGameObjectsWithTag("TowerGun").Length.ToString() + " TowerRocket: " + GameObject.FindGameObjectsWithTag("Tower").Length.ToString());

        statsText.text = "Ships: " + GameObject.FindGameObjectsWithTag("Ship").Length.ToString() + "\n" + "Seekers: " + GameObject.FindGameObjectsWithTag("Seeker").Length.ToString() + "\n" + "Units: " + GameObject.FindGameObjectsWithTag("Trooper").Length.ToString() + "\n" + "Cubes: " + (GameObject.FindGameObjectsWithTag("Follower").Length + GameObject.FindGameObjectsWithTag("OwnedFollower").Length).ToString() + "\n" + "Rockets: " + GameObject.FindGameObjectsWithTag("RocketAmmo").Length.ToString() + "\n" + "Gun-Towers: " + GameObject.FindGameObjectsWithTag("TowerGun").Length.ToString() + "\n" + "Rocket-Towers: " + GameObject.FindGameObjectsWithTag("Tower").Length.ToString() + "\n";
        statsText.text += "Total Objects: " + (GameObject.FindGameObjectsWithTag("Ship").Length + GameObject.FindGameObjectsWithTag("Seeker").Length + GameObject.FindGameObjectsWithTag("Trooper").Length + GameObject.FindGameObjectsWithTag("Follower").Length + GameObject.FindGameObjectsWithTag("OwnedFollower").Length + GameObject.FindGameObjectsWithTag("RocketAmmo").Length + GameObject.FindGameObjectsWithTag("TowerGun").Length + GameObject.FindGameObjectsWithTag("Tower").Length).ToString();


        int rndNum = Random.Range(0, GameMaster.GM.detailsList.Count);
        //GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.detailsList[rndNum], new Vector3(Random.Range(0, 200), 10, Random.Range(0, 200)), Quaternion.Euler(0, 0, 0), "Follower", GameMaster.GM.globalObjectList);
        GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.detailsList[rndNum], new Vector3(spawnPlatform.transform.position.x + Random.Range(-100, 100), 20, spawnPlatform.transform.position.z + Random.Range(-100, 100)), Quaternion.Euler(0, 0, 0), "Follower", GameMaster.GM.globalObjectList);

        if (createdObject.GetComponent<Rigidbody>() == null)
            createdObject.AddComponent<Rigidbody>();

        createdObject.GetComponent<Rigidbody>().mass = 5;
        //createdObject.transform.localScale = new Vector3(2, 2, 2);
        //createdObject.GetComponent<Rigidbody>().useGravity = false;
        createdObject.GetComponent<Rigidbody>().angularDrag = 0.05f;
        createdObject.GetComponent<Rigidbody>().drag = 1f;
        //createdObject.GetComponent<Rigidbody>().AddForce(Random.Range(-200, 200), Random.Range(-200, 200), Random.Range(-200, 200), ForceMode.Impulse);
        createdObject.GetComponent<Rigidbody>().AddTorque(transform.up * Random.Range(-1000, 1000));
        createdObject.GetComponent<Rigidbody>().AddTorque(transform.forward * Random.Range(-1000, 1000));
        createdObject.GetComponent<Rigidbody>().AddTorque(transform.right * Random.Range(-1000, 1000));

        if (createdObject.GetComponent<MeshCollider>() != null && createdObject.GetComponent<MeshCollider>().convex == false)
            createdObject.GetComponent<MeshCollider>().convex = true;

        // GameObject Boom = Instantiate(GameMaster.GM.ExplosionPrefab, CreatedObject.transform.position, Quaternion.Euler(0, 0, 0));
        numCubes++;
    }

    public void GenerateRollerBalls()
    {
        int rndNum = Random.Range(0, GameMaster.GM.detailsList.Count);
        GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.rollerEnemyBasePrefab, 0, 100, Random.Range(10, 20), Quaternion.Euler(0, 0, 0), "Roller", GameMaster.GM.globalObjectList);

        //createdObject.GetComponent<FractionIndexClass>().SetFractionId(Random.Range(0, GameMaster.GM.mainBaseCount));
        createdObject.GetComponent<FractionIndexClass>().SetFractionId(5);

        int rnDShip = Random.Range(0, GameMaster.GM.mainBaseCount);

        // createdObject.GetComponent<RollerEnemyBase>().targetToChase = GameMaster.GM.shipObjectList[rnDShip];
    }

    public static void printDeadHandler()
    {
        GameMaster.GM.playerShipHp.color = Color.red;
        GameMaster.GM.playerShipHp.text ="PlayerShip Destroyed";
    }
}

                
