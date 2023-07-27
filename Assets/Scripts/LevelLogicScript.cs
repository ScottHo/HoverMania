using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using PlasticGui.Configuration.CloudEdition.Welcome;

public class LevelLogicScript : MonoBehaviour
{
    IDatabaseRepository databaseRepository;
    public TextMeshProUGUI currentSampleText;
    public TextMeshProUGUI batteryText;
    public Slider batterySlider;
    public LevelContainers levelContainers;
    int batteryLife = 10000;
    bool batteryDraining;
    int currentSamples = 0;
    int totalSamples = 0;


    void Start()
    {
        LoadLevelObjects();
        SetupDatabase();
        ShowSamplesCollected();
    }

    private void FixedUpdate()
    {
        if (batteryDraining)
        {
            batteryLife -= 1;
            if (batteryLife % 10 == 0)
            {
                UpdateBatteryLifeUI();
            }
        }
    }

    void LoadLevelObjects()
    {
        int idToLoad = 1;
        if (UILogicScript.Instance != null)
        {
            idToLoad = UILogicScript.Instance.selectedLevelID;
        }
        Debug.Log("Loading level " + idToLoad);
        foreach (GameObject levelContainer in levelContainers.levels)
        {
            if (levelContainer.CompareTag(idToLoad.ToString()))
            {
                levelContainer.SetActive(true);
                totalSamples = levelContainer.transform.Find("SampleContainer").childCount;
                break;
            }
        }
    }

    void SetupDatabase()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            databaseRepository = new DummyDatabase("");
        }
        else
        {
            string databasePath = Application.persistentDataPath + "/main_db.sqlite";
            if (!System.IO.File.Exists(databasePath))
            {
                SqliteDatabase.CreateDatabase(databasePath);
            }
            databaseRepository = new SqliteDatabase("URI=file:" + databasePath);
            try
            {
                databaseRepository.switchUser(1);
            }
            catch (SqliteException)
            {
                databaseRepository.createUser();
            }
        }
    }

    private void ShowSamplesCollected()
    {
        currentSampleText.text = currentSamples.ToString() + " / " + totalSamples.ToString();
    }

    public void SampleCollected(Sample sample)
    {
        databaseRepository.addSample(sample);
        currentSamples++;
        ShowSamplesCollected();
    }

    public void UpdateBatteryLifeUI()
    {
        int batteryLifePercent = batteryLife / 100;
        if (batteryLifePercent <= 0)
        {
            batteryLifePercent = 0;
            GameOver();
        }
        batteryText.text = batteryLifePercent.ToString();
        batterySlider.value = batteryLifePercent; 
    }

    public void SetBatteryDraining(bool draining)
    {
        batteryDraining = draining;
    }
    public void DrainBattery(int amount)
    {
        batteryLife = batteryLife - amount;
        UpdateBatteryLifeUI();
    }

    public void GameOver()
    {
        SceneManager.LoadScene("UI");
    }
}

[System.Serializable]
public class LevelContainers
{
    public List<GameObject> levels;
}
