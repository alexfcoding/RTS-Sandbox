using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{    
    public Slider factionsSlider;
    public Slider moneySlider;
    public GameObject factionShip;
    List<GameObject> shipsList;
    public TMP_Text factionCountText;
    public TMP_Text moneyCountText;
    List<Color> factionColors;
        
    public void Start()
    {        
        shipsList = new List<GameObject>();
        SetBaseCount();
        SetMoneyCount();
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
        gameObject.GetComponent<AudioSource>().Play();

        int factionCount = (int)factionsSlider.value;       
        Settings.SET.mainBaseCount = factionCount;
       
        factionCountText.GetComponent<TMP_Text>().text = $"Factions: {factionCount.ToString()}";

        float spawnCircleAngle = Random.Range(0, 360);

        if (shipsList != null)
        {
            for (int i = 0; i < shipsList.Count; i++)
            {
                Destroy(shipsList[i].gameObject);
            }
        }

        factionColors = new List<Color>();

        for (int i = 0; i < factionCount; i++)
        {
            float r, g, b;

            r = (float)(Random.Range(0, 255) / 255f);
            g = (float)(Random.Range(0, 255) / 255f);
            b = (float)(Random.Range(0, 255) / 255f);

            Color newRandomColor = new Color(r, g, b, 1f);
            factionColors.Add(newRandomColor);
        }

        factionColors[0] = new Color(0, 1, 1, 1f);
        factionColors[1] = new Color(1, 0, 1, 1f);            

        for (int i = 0; i < factionCount; i++)
        {
            GameObject newShipObject = Instantiate(factionShip, new Vector3(200 * Mathf.Cos(spawnCircleAngle * 3.14f / 180), 0, 200 * Mathf.Sin(spawnCircleAngle * 3.14f / 180)), Quaternion.Euler(0, 0, 0));
            newShipObject.transform.LookAt(new Vector3(0, 0, 0));
            newShipObject.transform.localScale = new Vector3(0.045f, 0.045f, 0.045f);
            
            int lightsCount = 0;

            if (newShipObject.gameObject.GetComponentsInChildren<Light>() != null)
            {
                lightsCount = newShipObject.gameObject.GetComponentsInChildren<Light>().Length;

                for (int j = 0; j < lightsCount; j++)
                {
                    newShipObject.gameObject.GetComponentsInChildren<Light>()[j].color = factionColors[i];
                }
            }

            shipsList.Add(newShipObject);
            spawnCircleAngle += 360 / Settings.SET.mainBaseCount;
        }
    }

    public void SetMoneyCount()
    {
        int startMoney = (int)moneySlider.value;
        Settings.SET.startMoney = startMoney;
        moneyCountText.GetComponent<TMP_Text>().text = $"Start Money: {startMoney.ToString()} %";
    }
}
