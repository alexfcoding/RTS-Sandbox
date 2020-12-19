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
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        spawnPlatform = GameMaster.GM.ConstructObject(GameMaster.GM.spawnPlatform, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0), "SpawnPlatform", GameMaster.GM.globalObjectList);

        InvokeRepeating("UpdateStats", 0f, 1f);
        InvokeRepeating("GenerateCubes", 0f, 0.3f);
        //InvokeRepeating("GenerateRollerBalls", 0f, 10f);

        GameMaster.GM.SetFactionColors();

        SetupFactionsAndPlayer(GameMaster.GM.mainBaseCount); 
        CreateShipPlatforms();
        CreateSeekers(2);
                
        //CreateResources(1000);
        //CreateRollerBalls(20);
        //CreateWeapons(100);
        //CreateTroopers(20);
        //CreateLightShips(10);
    }

    public void SetupFactionsAndPlayer(int baseCount)
    {
        for (int i = 0; i < baseCount; i++)
        {
            GameObject newShipObject = GameMaster.GM.ConstructObject(GameMaster.GM.shipPrefab, new Vector3(2500 * Mathf.Cos(spawnCircleAngle * 3.14f / 180), 110, 2500 * Mathf.Sin(spawnCircleAngle * 3.14f / 180)), Quaternion.Euler(0, Random.Range(-180, 180), 0), "Seeker", GameMaster.GM.shipObjectList);
            newShipObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            newShipObject.GetComponent<Ship>().SetFractionId(i);

            if (i == 0)
            {
                GameMaster.GM.shipObjectList[0].GetComponent<Ship>().shipIsDeadEvent += printDeadHandler;
                GameMaster.GM.player.transform.position = GameMaster.GM.shipObjectList[0].transform.position + new Vector3(600, -100 + 5, 0);
                GameMaster.GM.player.GetComponent<Player>().playerHealth3dText.color = GameMaster.GM.fractionColors[0];
                GameMaster.GM.ConstructObject(GameMaster.GM.machineGunPrefab, GameMaster.GM.player.transform.TransformPoint(10000, 0, -6000), Quaternion.Euler(0, 0, 0), "MachineGun", GameMaster.GM.weaponObjectList);
                GameMaster.GM.ConstructObject(GameMaster.GM.rocketLauncherPrefab, GameMaster.GM.player.transform.TransformPoint(10000, 0, -3000), Quaternion.Euler(0, 0, 0), "RocketLauncher", GameMaster.GM.weaponObjectList);
                GameMaster.GM.ConstructObject(GameMaster.GM.rocketLauncherMiniPrefab, GameMaster.GM.player.transform.TransformPoint(10000, 0, 0), Quaternion.Euler(0, 0, 0), "RocketLauncher", GameMaster.GM.weaponObjectList);
                GameMaster.GM.ConstructObject(GameMaster.GM.bombLauncherPrefab, GameMaster.GM.player.transform.TransformPoint(10000, 0, 3000), Quaternion.Euler(0, 0, 0), "BombLauncher", GameMaster.GM.weaponObjectList);
                
                if (GameMaster.GM.aiPlayerBase == true)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        GameObject newTower = GameMaster.GM.ConstructObject(GameMaster.GM.TowerGunPrefab, new Vector3(newShipObject.transform.position.x - 500 * Mathf.Cos(spawnCircleAngle2 * 3.14f / 180), 0, newShipObject.transform.position.z - 500 * Mathf.Sin(spawnCircleAngle2 * 3.14f / 180)), Quaternion.Euler(0, Random.Range(-180, 180), 0), "GunTower", GameMaster.GM.globalObjectList);
                        newTower.GetComponent<Tower>().SetFractionId(newShipObject.GetComponent<Ship>().factionId);
                        newTower.GetComponent<Tower>().isVulnerable = true;
                        newTower.GetComponent<Tower>().health = 5000;
                        newTower.GetComponent<Tower>().maxHP = 5000;
                        spawnCircleAngle2 += 360 / 20;
                    }
                }
            }
            else
            {
                for (int j = 0; j < 20; j++)
                {
                    GameObject newTower = GameMaster.GM.ConstructObject(GameMaster.GM.TowerGunPrefab, new Vector3(newShipObject.transform.position.x - 500 * Mathf.Cos(spawnCircleAngle2 * 3.14f / 180), 0, newShipObject.transform.position.z - 500 * Mathf.Sin(spawnCircleAngle2 * 3.14f / 180)), Quaternion.Euler(0, Random.Range(-180, 180), 0), "GunTower", GameMaster.GM.globalObjectList);
                    newTower.GetComponent<Tower>().SetFractionId(newShipObject.GetComponent<Ship>().factionId);
                    newTower.GetComponent<Tower>().isVulnerable = true;
                    newTower.GetComponent<Tower>().health = 5000;
                    newTower.GetComponent<Tower>().maxHP = 5000;
                    spawnCircleAngle2 += 360 / 20;
                }
            }

            spawnCircleAngle += 360 / GameMaster.GM.mainBaseCount;
        }
    }

    public void CreateShipPlatforms()
    {
        for (int i = 0; i < GameMaster.GM.shipObjectList.Count; i++)
        {
            GameObject newPlatformObject = GameMaster.GM.ConstructObject(GameMaster.GM.platformPrefab, GameMaster.GM.shipObjectList[i].transform.position - new Vector3(0, GameMaster.GM.shipObjectList[i].transform.position.y - 20, 0), Quaternion.Euler(0, 0, 0), "Platform", GameMaster.GM.platformObjectList);
            newPlatformObject.transform.SetParent(GameMaster.GM.shipObjectList[i].transform);
            newPlatformObject.transform.localPosition = new Vector3(0, -275, -81);
            GameMaster.GM.shipObjectList[i].GetComponent<Ship>().EarnMoney(0);
            newPlatformObject.gameObject.GetComponent<FactionIndex>().healthBar.transform.localScale = new Vector3(0, 0, 0);
        }
    }

    public void CreateSeekers(int count)
    {
        for (int i = 0; i < GameMaster.GM.mainBaseCount; i++)
            for (int j = 0; j < count; j++)
            {
                GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.seekerPrefab, GameMaster.GM.shipObjectList[i].transform.position - new Vector3(0, GameMaster.GM.shipObjectList[i].transform.position.y, 0) +
                    new Vector3(Random.Range(-300, 300), 0, Random.Range(-300, 300)), Quaternion.Euler(0, 0, 0), "Seeker", GameMaster.GM.globalObjectList);

                createdObject.GetComponent<Seeker>().SetFractionId(i);
                createdObject.GetComponent<Seeker>().textHP.gameObject.GetComponent<TextMesh>().color = GameMaster.GM.fractionColors[i];

                warriorPosition = createdObject.transform.position + new Vector3(0, 2, 0);
                GameMaster.GM.GiveWeaponToObject(warriorPosition);
            }
    }

    public void CreateResources(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int rndNum = Random.Range(0, GameMaster.GM.detailsList.Count);
            GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.detailsList[rndNum], -100, 100, Random.Range(10, 500), Quaternion.Euler(0, 0, 0), "Follower", GameMaster.GM.globalObjectList);

            if (createdObject.GetComponent<Rigidbody>() == null)
                createdObject.AddComponent<Rigidbody>();

            createdObject.GetComponent<Rigidbody>().mass = 5;
            createdObject.GetComponent<Rigidbody>().angularDrag = 0.05f;
            createdObject.GetComponent<Rigidbody>().drag = 1f;
            createdObject.GetComponent<Rigidbody>().AddTorque(transform.up * Random.Range(-1000, 1000));
            createdObject.GetComponent<Rigidbody>().AddTorque(transform.forward * Random.Range(-1000, 1000));
            createdObject.GetComponent<Rigidbody>().AddTorque(transform.right * Random.Range(-1000, 1000));
            
            if (createdObject.GetComponent<MeshCollider>() != null && createdObject.GetComponent<MeshCollider>().convex == false)
                createdObject.GetComponent<MeshCollider>().convex = true;

            numCubes++;
        }
    }

    public void CreateRollerBalls(int count)
    {
        for (int i = 0; i < count; i++)
        {            
            GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.rollerEnemyBasePrefab, -1000, 1000, Random.Range(2, 40), Quaternion.Euler(0, 0, 0), "RollerEnemyBase", GameMaster.GM.globalObjectList);
            createdObject.GetComponent<FactionIndex>().SetFractionId(5);
        }
    }
    
    public void CreateWeapons(int count)
    {
        for (int i = 0; i < count; i++) // Создаем оружие RocketLauncherPrefab, RocketLauncherMiniPrefab, MachineGunPrefab в рандомной позиции от 50 до 950 (вся карта)
        {
            GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.rocketLauncherPrefab, -1000, 1000, 4, Quaternion.Euler(0, 0, 0), "RocketLauncher", GameMaster.GM.weaponObjectList);
            createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.rocketLauncherMiniPrefab, -1000, 1000, 4, Quaternion.Euler(0, 0, 0), "RocketLauncher", GameMaster.GM.weaponObjectList);
            createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.machineGunPrefab, -1000, 1000, 4, Quaternion.Euler(0, 0, 0), "MachineGun", GameMaster.GM.weaponObjectList);
        }
    }
    
    public void CreateTroopers(int count)
    {        
        for (int i = 0; i < GameMaster.GM.mainBaseCount; i++)
            for (int j = 0; j < count; j++)
            {
                GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.trooperPrefab, GameMaster.GM.shipObjectList[i].transform.position + new Vector3(Random.Range(-130, 130), -GameMaster.GM.shipObjectList[i].transform.position.y, Random.Range(-130, 130)), Quaternion.Euler(0, 0, 0), "Trooper", GameMaster.GM.unitList);
                
                createdObject.GetComponent<FactionIndex>().SetFractionId(i);
                createdObject.GetComponent<Seeker>().textHP.gameObject.GetComponent<TextMesh>().color = GameMaster.GM.fractionColors[i];

                if (createdObject.GetComponent<FactionIndex>().factionId == 0)
                    createdObject.GetComponent<Seeker>().textHP.gameObject.GetComponent<TextMesh>().text = createdObject.GetComponent<Seeker>().level.ToString() + " *";

                warriorPosition = createdObject.transform.position + new Vector3(0, 0, 0);
                GameMaster.GM.GiveWeaponToObject(warriorPosition);

                int rnDShip = Random.Range(0, GameMaster.GM.mainBaseCount);
                while (rnDShip == i)
                    rnDShip = Random.Range(0, GameMaster.GM.mainBaseCount);
                if (i != 0)
                    createdObject.GetComponent<Trooper>().targetToChase = GameMaster.GM.shipObjectList[rnDShip];
                else
                    createdObject.GetComponent<Trooper>().targetToChase = GameMaster.GM.player.gameObject;
            }
    }

    public void CreateLightShips(int count)
    {
        for (int i = 0; i < GameMaster.GM.mainBaseCount; i++)
            for (int j = 0; j < count; j++)
            {
                GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.lightShipPrefab, GameMaster.GM.shipObjectList[i].transform.position + new Vector3(Random.Range(-130, 130), -GameMaster.GM.shipObjectList[i].transform.position.y + 3, Random.Range(-130, 130)), Quaternion.Euler(0, 0, 0), "LightShip", GameMaster.GM.unitList);
                
                //float RandomScale = Random.Range(2f, 3f); // Рандомизируем размеры пехоты
                //CreatedObject.transform.localScale = new Vector3(RandomScale, RandomScale, RandomScale); // Рандомизируем размеры пехоты

                createdObject.GetComponent<FactionIndex>().SetFractionId(i);
                createdObject.GetComponent<Seeker>().textHP.gameObject.GetComponent<TextMesh>().color = GameMaster.GM.fractionColors[i];

                if (createdObject.GetComponent<FactionIndex>().factionId == 0)
                    createdObject.GetComponent<Seeker>().textHP.gameObject.GetComponent<TextMesh>().text = createdObject.GetComponent<Seeker>().level.ToString() + " *";

                warriorPosition = createdObject.transform.position + new Vector3(0, 2, 0);
                GameMaster.GM.GiveWeaponToObject(warriorPosition);
                int rndShip = Random.Range(0, GameMaster.GM.mainBaseCount);

                while (rndShip == i)
                    rndShip = Random.Range(0, GameMaster.GM.mainBaseCount);

                if (i != 0)
                    createdObject.GetComponent<Trooper>().targetToChase = GameMaster.GM.shipObjectList[rndShip];
                else
                    createdObject.GetComponent<Trooper>().targetToChase = GameMaster.GM.player.gameObject;
            }
    }

    public void GenerateCubes ()
    {
        int rndNum = Random.Range(0, GameMaster.GM.detailsList.Count);
        GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.detailsList[rndNum], new Vector3(spawnPlatform.transform.position.x + Random.Range(-100, 100), 20, spawnPlatform.transform.position.z + Random.Range(-100, 100)), Quaternion.Euler(0, 0, 0), "Follower", GameMaster.GM.globalObjectList);

        if (createdObject.GetComponent<Rigidbody>() == null)
            createdObject.AddComponent<Rigidbody>();

        createdObject.GetComponent<Rigidbody>().mass = 5;
        createdObject.GetComponent<Rigidbody>().angularDrag = 0.05f;
        createdObject.GetComponent<Rigidbody>().drag = 1f;
        createdObject.GetComponent<Rigidbody>().AddTorque(transform.up * Random.Range(-1000, 1000));
        createdObject.GetComponent<Rigidbody>().AddTorque(transform.forward * Random.Range(-1000, 1000));
        createdObject.GetComponent<Rigidbody>().AddTorque(transform.right * Random.Range(-1000, 1000));

        if (createdObject.GetComponent<MeshCollider>() != null && createdObject.GetComponent<MeshCollider>().convex == false)
            createdObject.GetComponent<MeshCollider>().convex = true;

        numCubes++;
    }

    public void GenerateRollerBalls()
    {
        int rndNum = Random.Range(0, GameMaster.GM.detailsList.Count);
        GameObject createdObject = GameMaster.GM.ConstructObject(GameMaster.GM.rollerEnemyBasePrefab, 0, 100, Random.Range(10, 20), Quaternion.Euler(0, 0, 0), "Roller", GameMaster.GM.globalObjectList);

        createdObject.GetComponent<FactionIndex>().SetFractionId(5);
    }

    public static void printDeadHandler()
    {
        GameMaster.GM.playerShipHp.color = Color.red;
        GameMaster.GM.playerShipHp.text ="PlayerShip Destroyed";
    }

    public void UpdateStats()
    {
        statsText.text = "Ships: " + GameObject.FindGameObjectsWithTag("Ship").Length.ToString() + "\n" + "Seekers: " + GameObject.FindGameObjectsWithTag("Seeker").Length.ToString() + "\n" + "Units: " + GameObject.FindGameObjectsWithTag("Trooper").Length.ToString() + "\n" + "Cubes: " + (GameObject.FindGameObjectsWithTag("Follower").Length + GameObject.FindGameObjectsWithTag("OwnedFollower").Length).ToString() + "\n" + "Rockets: " + GameObject.FindGameObjectsWithTag("RocketAmmo").Length.ToString() + "\n" + "Gun-Towers: " + GameObject.FindGameObjectsWithTag("TowerGun").Length.ToString() + "\n" + "Rocket-Towers: " + GameObject.FindGameObjectsWithTag("Tower").Length.ToString() + "\n";
        statsText.text += "Total Objects: " + (GameObject.FindGameObjectsWithTag("Ship").Length + GameObject.FindGameObjectsWithTag("Seeker").Length + GameObject.FindGameObjectsWithTag("Trooper").Length + GameObject.FindGameObjectsWithTag("Follower").Length + GameObject.FindGameObjectsWithTag("OwnedFollower").Length + GameObject.FindGameObjectsWithTag("RocketAmmo").Length + GameObject.FindGameObjectsWithTag("TowerGun").Length + GameObject.FindGameObjectsWithTag("Tower").Length).ToString();
    }
}

                
