using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for global variables and functions
/// 
/// \code
/// sdfsdfsdfsd
/// fsdfsdf
/// sdf
/// \endcode
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class GameMaster : MonoBehaviour
{
    public static GameMaster GM;

    /// <summary>
    /// Count of base ships (including player's)
    /// </summary>
    public int mainBaseCount;

    public List<Color> fractionColors;

    /// <summary>
    /// Player health
    /// </summary>
    public Text playerHp;

    /// <summary>
    /// Player's main ship health
    /// </summary>
    public Text playerShipHp;

    /// <summary>
    /// Player money text
    /// </summary>
    public Text playerMoneyText;

    /// <summary>
    /// The enemy money text
    /// </summary>
    public Text enemyMoneyText;

    /// <summary>
    /// Small explosion effect
    /// </summary>
    public GameObject effectBoom;

    /// <summary>
    /// Effect for player and enemy death
    /// </summary>
    public GameObject enemyDestroy;

    /// <summary>
    /// Text with health above main bases
    /// </summary>
    public GameObject text3dDamage;

    /// <summary>
    /// Smoke after death
    /// </summary>
    public GameObject smokeAfterDeath;

    /// <summary>
    /// Main camera
    /// </summary>
    public GameObject myCamera;

    /// <summary>
    /// Seeker prefab
    /// </summary>
    public GameObject seekerPrefab;
       
    /// <summary>
    /// Rocket launcher prefab
    /// </summary>
    public GameObject rocketLauncherPrefab;

    /// <summary>
    /// Rocket launcher mini prefab
    /// </summary>
    public GameObject rocketLauncherMiniPrefab;

    /// <summary>
    /// Machine gun prefab
    /// </summary>
    public GameObject machineGunPrefab;

    /// <summary>
    /// Trooper prefab
    /// </summary>
    public GameObject trooperPrefab;

    /// <summary>
    /// Tower prefab
    /// </summary>
    public GameObject TowerPrefab;


    /// <summary>
    /// FLight ship prefab
    /// </summary>
    public GameObject lightShipPrefab;

    /// <summary>
    /// Main ship base prefab
    /// </summary>
    public GameObject shipPrefab;

    /// <summary>
    /// Platform prefab for collecting resources
    /// </summary>
    public GameObject platformPrefab;

    /// <summary>
    /// Barracs prefab
    /// </summary>
    public GameObject trooperBase;

    /// <summary>
    /// Factory prefab
    /// </summary>
    public GameObject factoryPrefab;

    /// <summary>
    /// Healthbar above all units
    /// </summary>
    public GameObject healthBar;

    /// <summary>
    /// Team select sprite
    /// </summary>
    public GameObject teamSelect;

    /// <summary>
    /// Team marker sprite
    /// </summary>
    public GameObject teamMarker;

    /// <summary>
    /// Global object list
    /// </summary>
    public List<GameObject> globalObjectList;

    /// <summary>
    /// Rockets object list
    /// </summary>
    public List<GameObject> bulletObjectList;

    /// <summary>
    /// Weapon object list
    /// </summary>
    public List<GameObject> weaponObjectList;

    /// <summary>
    /// Main ship object list
    /// </summary>
    public List<GameObject> shipObjectList;

    /// <summary>
    /// Platform object list
    /// </summary>
    public List<GameObject> platformObjectList;

    /// <summary>
    /// Unit list
    /// </summary>
    public List<GameObject> unitList;

    /// <summary>
    /// Trooper barracs list
    /// </summary>
    public List<GameObject> trooperBaseList;

    /// <summary>
    /// Resources list (followers)
    /// </summary>
    public List<GameObject> detailsList;

    /// <summary>
    /// Player object
    /// </summary>
    public Transform player;

    /// <summary>
    /// To shake a camera
    /// </summary>
    public bool shakeCamera;

    /// <summary>
    /// Timer
    /// </summary>
    public float timer;

    /// <summary>
    /// Total number of cubes
    /// </summary>
    public int numCubes;

    /// <summary>
    /// Boom sound
    /// </summary>
    public AudioSource boomSound;

    /// <summary>
    /// Enemy explode sound
    /// </summary>
    public AudioClip explodeEnemySound;

    void Awake()
    {
        if (GM != null)
            GameObject.Destroy(GM);
        else
            GM = this;

        DontDestroyOnLoad(this);

        mainBaseCount = 3;
    }

    public void SetFractionColors ()
    {
        fractionColors = new List<Color>();

        for (int i = 0; i < GameMaster.GM.mainBaseCount; i++)
        {
            float r, g, b;

            r = (float)(Random.Range(0, 255) / 255f);
            g = (float)(Random.Range(0, 255) / 255f);
            b = (float)(Random.Range(0, 255) / 255f);

            Color newRandomColor = new Color(r, g, b, 1f);
            fractionColors.Add(newRandomColor);
        }
    }

    /// <summary>
    /// A custom constructor for objects
    /// </summary>
    /// <param name="prefab">The prefab.</param>
    /// <param name="spawnWidth">Random width range for spawning.</param>
    /// <param name="SpawnLength">Random length range for spawning.</param>
    /// <param name="height">Spawning height.</param>
    /// <param name="Q">Quaternion.</param>
    /// <param name="objectName">Name of the object.</param>
    /// <param name="listToAdd">A list to add created object.</param>
    /// <returns></returns>
    public GameObject ConstructObject(GameObject prefab, float spawnWidth, float SpawnLength, float height, Quaternion Q, string objectName, List<GameObject> listToAdd)
    {
        Vector3 RandomSpawnPosition = new Vector3(Random.Range(spawnWidth, SpawnLength), height, Random.Range(spawnWidth, SpawnLength));
        GameObject CreatedObject = Instantiate(prefab, RandomSpawnPosition, Q) as GameObject;
        CreatedObject.gameObject.name = objectName;
        listToAdd.Add(CreatedObject);
        numCubes++;
        return CreatedObject;
    }


    /// <summary>
    /// ConstructObject method overload for fixed Vector3 SpawnPosition.
    /// </summary>
    public GameObject ConstructObject(GameObject prefab, Vector3 spawnPosition, Quaternion Q, string objectName, List<GameObject> listToAdd)
    {
        GameObject CreatedObject = Instantiate(prefab, spawnPosition, Q) as GameObject;
        CreatedObject.gameObject.name = objectName;
        listToAdd.Add(CreatedObject);
        numCubes++;
        return CreatedObject;
    }

    public void PlayEnemyExplosionSound(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(explodeEnemySound, position);
    }

    /// <summary>
    /// Recursive destroying. Used for destroy all object childrens.
    /// </summary>
    /// <param name="objectTransform">Object Transform to destroy.</param>
    /// <param name="gameObj">Game object to destroy.</param>
    /// <param name="timeToDestroy">Time to destroy object.</param>
    public void RecursiveDestroy (Transform objectTransform, GameObject gameObj,  float timeToDestroy)
    {
        foreach (Transform child in objectTransform)
        {
            if (child.GetComponent<MeshRenderer>() != null)
                child.GetComponent<MeshRenderer>().enabled = false;

            if (child.GetComponent<Collider>() != null)
                child.GetComponent<Collider>().enabled = false;

            if (child.GetComponent<ParticleSystem>() != null)
                child.GetComponent<ParticleSystem>().Stop();

            RecursiveDestroy(child, gameObj, timeToDestroy);
        }

        if (gameObj.GetComponent<MeshRenderer>() != null)
            gameObj.GetComponent<MeshRenderer>().enabled = false;

        if (gameObj.GetComponent<PlayerClass>() == null)
           Destroy(gameObj, timeToDestroy);
    }

    /// <summary>
    /// Gives weapon to selected object.
    /// </summary>
    /// <param name="objectPosition">The object position.</param>
    public void GiveWeaponToObject (Vector3 objectPosition)
    {
        int rnd = Random.Range(0, 100);

        if (rnd >= 65 && rnd < 100)
            GameMaster.GM.ConstructObject(GameMaster.GM.rocketLauncherMiniPrefab, objectPosition, Quaternion.Euler(0, 0, 0), "RocketLauncher", GameMaster.GM.weaponObjectList);

        if (rnd >= 30 && rnd < 65)
         GameMaster.GM.ConstructObject(GameMaster.GM.rocketLauncherPrefab, objectPosition, Quaternion.Euler(0, 0, 0), "RocketLauncher", GameMaster.GM.weaponObjectList);

        if (rnd <= 30)
            GameMaster.GM.ConstructObject(GameMaster.GM.machineGunPrefab, objectPosition, Quaternion.Euler(0, 0, 0), "RocketLauncher", GameMaster.GM.weaponObjectList);
    }
    
    public void FixedUpdate()
    {
        timer += Time.deltaTime;
    }
}
