using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

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
    public Image fader;
    public AudioSource audioSource;
    public int defaultId = 1;
    int defaultBatteryLife = 100;
    int batteryLife = 100;
    bool batteryDraining;
    float batteryUpdateTime = 0;
    int currentSamples = 0;
    int totalSamples = 0;
    float elapsedTime = 0;
    public bool gameIsOver = false;
    int loadedId = -1;
    public bool fading = false;
    bool fadeIn = true;
    bool syncInProgress = false;
    Action actionAfterFade;
    IDatabaseRepository databaseRepository;

    void Start()
    {
        databaseRepository = DatabaseManager.Instance.database;
        SetupScene();
        SetupFader(true, null);
    }

    private void FixedUpdate()
    {
        if (fading)
        {
            Fade();
            return;
        }
        if (gameIsOver)
            return;
        elapsedTime += Time.deltaTime;
        if (batteryDraining)
        {
            batteryUpdateTime += Time.deltaTime;
            if (batteryUpdateTime > 1f)
            {
                batteryLife -= 1;
                UpdateBatteryLifeUI();
                batteryUpdateTime = 0;
            }
        }
        UpdateTimeUI();
    }

    void SetupScene()
    {
        gameOverContainer.SetActive(false);
        quitButton.onClick.AddListener(GameOver);

        int idToLoad = PlayerPrefs.GetInt("SelectedLevel");
        if (idToLoad == -1)
        {
            idToLoad = defaultId;

        }
        Debug.Log("Loading level " + idToLoad);
        foreach (GameObject levelContainer in levelContainers.levels)
        {
            if (levelContainer.CompareTag(idToLoad.ToString()))
            {
                levelContainer.SetActive(true);
                totalSamples = levelContainer.transform.Find("SampleContainer").childCount;
            }
            else
            {
                levelContainer.SetActive(false);
            }
        }
        loadedId = idToLoad;
        ShowSamplesCollected();
        UpdateBatteryLifeUI();
    }

    void SetupFader(bool fadeIn, Action action)
    {
        fading = true;
        this.fadeIn = fadeIn;
        this.actionAfterFade = action;
        Color color = Color.black;
        if (fadeIn)
        {
            color.a = 1f;
        }
        else
        {
            color.a = 0f;
        }
        fader.color = color;
    }
     
    void Fade()
    {
        Color color = fader.color;
        if (fadeIn)
        {
            color.a -= .05f;
            if (color.a <= 0f)
            {
                FinishFade();
            }
        }
        else // Fade out
        {
            color.a += .05f;
            if (color.a >= 1f)
            {
                FinishFade();
            }
        }
        fader.color = color;
    }

    void FinishFade()
    {
        if (syncInProgress)
            return;
        fading = false;

        if (actionAfterFade != null)
        {
            actionAfterFade();
        }
    }

    private void ShowSamplesCollected()
    {
        currentSampleText.text = currentSamples.ToString() + " / " + totalSamples.ToString();
    }

    public void SampleCollected(int samples)
    {
        AudioManager.PlayEffect(AudioAction.Collect, ref audioSource);
        currentSamples += samples;
        ShowSamplesCollected();
        if (currentSamples == totalSamples)
        {
            GameOver();
        }
    }

    public void IncreaseBatteryLife(int life)
    {
        AudioManager.PlayEffect(AudioAction.Battery, ref audioSource);
        if (batteryLife + life > defaultBatteryLife)
        {
            batteryLife = defaultBatteryLife;
        }
        else
        {
            batteryLife += life;
        }
    }

    public void UpdateBatteryLifeUI()
    {
        if (batteryLife <= 0)
        {
            batteryLife = 0;
            GameOver();
        }
        batteryText.text = batteryLife.ToString();
        batterySlider.value = batteryLife;
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
                Win();
            }
            else
            {
                Lose();
            }
            gameOverButton.onClick.AddListener(ReturnToMenu);
            retryButton.onClick.AddListener(RetryMission);
        }
    }

    void Win()
    {
        AudioManager.PlayEffect(AudioAction.Win, ref audioSource);
        string text = "Mission Complete!";
        int previousTimeCentiseconds = -1;
        if (databaseRepository != null)
        {
            previousTimeCentiseconds = databaseRepository.GetLevelTime(loadedId);

        }
        int timeCentiseconds = (int)(elapsedTime * 100);
        if (previousTimeCentiseconds == -1)
        {
            string newTime = TimeSpan.FromMilliseconds(
                    timeCentiseconds * 10).ToString(@"mm\:ss\.ff");
            text += "\n\nNew Record!";
            text += "\nTime: " + newTime;
        }
        else if (previousTimeCentiseconds > timeCentiseconds)
        {
            string newTime = TimeSpan.FromMilliseconds(
                    timeCentiseconds * 10).ToString(@"mm\:ss\.ff");
            string oldTime = TimeSpan.FromMilliseconds(
                    previousTimeCentiseconds * 10).ToString(@"mm\:ss\.ff");
            text += "\n\nNew Record!";
            text += "\nPrevious Time: " + oldTime;
            text += "\nNew Time: " + newTime;
        }
        

        if (PlayerPrefs.GetInt("LeaderboardConnected") == 1)
        {
            UploadScore(timeCentiseconds);
        }
        gameOverText.text = text;

        databaseRepository.SetLevelTime(loadedId, timeCentiseconds);
    }

    async void UploadScore(int timeCentiseconds)
    {
        syncInProgress = true;
        await CloudSync.UploadHiScore(loadedId, timeCentiseconds);
        syncInProgress = false;
    }

    void Lose()
    {
        gameOverText.text = "Mission Failed!";
    }

    public void ReturnToMenu()
    {
        Action a = () => {
            SceneManager.LoadScene("UI");
        };
        SetupFader(false, a);
    }

    public void RetryMission()
    {
        gameOverContainer.SetActive(false);
        Action a = () => {
            StartCoroutine(LoadLevelAfterDelay(.5f));
        };
        SetupFader(false, a);
      }
    IEnumerator LoadLevelAfterDelay(float delay)
    {
        // Loading in feels to fast, wait a second
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Level");
    }
}

[System.Serializable]
public class LevelContainers
{
    public List<GameObject> levels;
}
