using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class LevelLogicScript : MonoBehaviour
{
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
    int loadedId = -1;
    IDatabaseRepository databaseRepository;

    void Start()
    {
        databaseRepository = UILogicScript.Instance.databaseRepository;
        SetupScene();
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
        loadedId = idToLoad;
    }

    private void ShowSamplesCollected()
    {
        currentSampleText.text = currentSamples.ToString() + " / " + totalSamples.ToString();
    }

    public void SampleCollected(Sample sample)
    {
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
        TimeSpan time = TimeSpan.FromMilliseconds(elapsedTime*1000);
        timerText.text = time.ToString(@"mm\:ss\.ff");
    }

    public void GameOver()
    {
        if (!gameIsOver)
        {
            quitButton.enabled = false;
            gameIsOver = true;
            gameOverContainer.SetActive(true);
            if (currentSamples == totalSamples)
            {
                string text = "Mission Complete!";
                int timeCentiseconds = (int) (elapsedTime * 100);
                int previousTimeCentiseconds = databaseRepository.GetLevelTime(loadedId);
                if (previousTimeCentiseconds > timeCentiseconds)
                {
                    string newTime = TimeSpan.FromMilliseconds(
                            timeCentiseconds * 10).ToString(@"mm\:ss\.ff");
                    string oldTime = TimeSpan.FromMilliseconds(
                            previousTimeCentiseconds * 10).ToString(@"mm\:ss\.ff");
                    text += "\nNew Record!";
                    text += "\nPrevious Time: " + oldTime;
                    text += "\nNew Time: " + newTime;
                }
                gameOverText.text = text;
                databaseRepository.SetLevelTime(loadedId, timeCentiseconds);
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
