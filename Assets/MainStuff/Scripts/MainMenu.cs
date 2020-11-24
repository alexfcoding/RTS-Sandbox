using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{    
    public Slider slider;
    public GameObject factionShip;
    List<GameObject> shipsList;  

    public void Start()
    {
        Application.targetFrameRate = 60;
        shipsList = new List<GameObject>();
        SetBaseCount();
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void SetBaseCount()
    {
        Settings.SET.mainBaseCount = (int) slider.value;      

        float spawnCircleAngle = Random.Range(0, 360);
        if (shipsList != null)
        {
            for (int i = 0; i < shipsList.Count; i++)
            {
                Destroy(shipsList[i].gameObject);
            }
        }          
        
        for (int i = 0; i < Settings.SET.mainBaseCount; i++)
        {           
            GameObject newShipObject = Instantiate(factionShip, new Vector3(200 * Mathf.Cos(spawnCircleAngle * 3.14f / 180), 0, 200 * Mathf.Sin(spawnCircleAngle * 3.14f / 180)), Quaternion.Euler(0, 0, 0));
            newShipObject.transform.LookAt(new Vector3(0, 0, 0));
            newShipObject.transform.localScale = new Vector3(0.045f, 0.045f, 0.045f);
            shipsList.Add(newShipObject);    
            spawnCircleAngle += 360 / Settings.SET.mainBaseCount;
        }
    }
}
