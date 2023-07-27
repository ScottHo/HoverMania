using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class LevelLogicScript : MonoBehaviour
{
    IDatabaseRepository databaseRepository;
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI currentSampleText;
    public TextMeshProUGUI batteryText;
    public Button quitButton;
    public GameObject gameOverContainer;
    public TextMeshProUGUI gameOverText;
    public Button gameOverButton;
    public Button retryButton;
    public Slider batterySlider;
    public LevelContainers levelContainers;
    public int defaultId = 1;
    int batteryLife = 10000;
    bool batteryDraining;
    int currentSamples = 0;
    int totalSamples = 0;
    float elapsedTime = 0;
    public bool gameIsOver = false;

    void Start()
    {

        SetupScene();
        SetupDatabase();
        ShowSamplesCollected();
    }

    private void FixedUpdate()
    {
        if (gameIsOver)
            return;
        elapsedTime += Time.deltaTime;
        if (batteryDraining)
        {
            batteryLife -= 1;
            if (batteryLife % 10 == 0)
            {
                UpdateBatteryLifeUI();
            }
        }
        UpdateTimeUI();
    }

    void SetupScene()
    {
        gameOverContainer.SetActive(false);
        quitButton.onClick.AddListener(GameOver);

        int idToLoad = defaultId;
        if (UILogicScript.Instance != null)
        {
            idToLoad = UILogicScript.Instance.selectedLevelID;
        }
        levelNameText.text = LevelFactory.GetLevelInfo(idToLoad).levelName;
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
        if (currentSamples == totalSamples)
        {
            GameOver();
        }
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

    public void UpdateTimeUI()
    {
        TimeSpan time = TimeSpan.FromSeconds(elapsedTime);
        timerText.text = time.ToString(@"mm\:ss");
    }

    public void GameOver()
    {
        if (!gameIsOver)
        {
            gameIsOver = true;
            gameOverContainer.SetActive(true);
            if (currentSamples == totalSamples)
            {
                gameOverText.text = "Mission Compelte!";
            }
            else
            {
                gameOverText.text = "Mission Failed!";
            }
            gameOverButton.onClick.AddListener(ReturnToMenu);
            retryButton.onClick.AddListener(RetryMission);
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("UI");
    }

    public void RetryMission()
    {
        SceneManager.LoadScene("Level");
    }
}

[System.Serializable]
public class LevelContainers
{
    public List<GameObject> levels;
}
