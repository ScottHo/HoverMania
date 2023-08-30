using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILogicScript : MonoBehaviour
{
    public LevelSelectContainers levelSelectContainers;
    public Button playButton;
    public Button leftSelectButton;
    public Button rightSelectButton;
    public int selectedLevelID;
    public static UILogicScript Instance;
    IDatabaseRepository databaseRepository;
    int levelOffset = 0;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
        }
        else if (Instance != this)
        {
            //Instance is not the same as the one we have, destroy old one, and reset to newest one
            Destroy(Instance.gameObject);
            Instance = this;
            Init();
        }
    }

    void Init()
    {
        DontDestroyOnLoad(gameObject);
        databaseRepository = DatabaseManager.Instance.database;
    }

    private void Start()
    {
        AddListeners();
        UpdateLevelsLocked();
        FillLevelSelectContainers();
    }

    void AddListeners()
    {
        playButton.onClick.AddListener(PlayLevel);
        foreach (var container in levelSelectContainers.containers)
        {
            Button button = container.GetComponent<Button>();
            button.onClick.AddListener(() => { LevelClicked(container); });
        }
        leftSelectButton.onClick.AddListener(() => { shiftLevelsLeft(); });
        rightSelectButton.onClick.AddListener(() => { shiftLevelsRight(); });
    }

    void UpdateLevelsLocked()
    {
        for (int i = 0; i < LevelFactory.NumLevels(); i++)
        {
            if (i < 3)
            {
                databaseRepository.SetLevelLocked(i+1, false);
                continue;
            }
            if (databaseRepository.GetLevelTime(i) > 0)
            {
                databaseRepository.SetLevelLocked(i + 1, false);
            }
            else
            {
                databaseRepository.SetLevelLocked(i + 1, true);
            }
        }
    }

    void FillLevelSelectContainers()
    {
        foreach (var it in levelSelectContainers.containers.Select((Value, Index) => new { Value, Index }))
        {
            GameObject container = it.Value;
            LevelSelectorUIScript script = container.GetComponent<LevelSelectorUIScript>();
            int levelID = it.Index + levelOffset + 1;
            container.SetActive(true);
            if (levelID > LevelFactory.NumLevels())
            {
                container.SetActive(false);
                continue;
            }
            if (databaseRepository.GetLevelLocked(levelID))
            {
                LevelInfo info = LevelFactory.GetLevelInfo(-1);
                script.SetLevelInfo(info);
                script.SetLocked(true);
            }
            else
            {
                LevelInfo info = LevelFactory.GetLevelInfo(levelID);
                script.SetLevelInfo(info);
                script.SetLocked(false);
                script.SetBestTime(databaseRepository.GetLevelTime(info.id));
            }
        }
        bool leftActive = true;
        bool rightActive = true;
        if (levelOffset == 0)
        {
            leftActive = false;
        }
        if (levelOffset + 3 >= LevelFactory.NumLevels())
        {
            rightActive = false;
        }
        leftSelectButton.gameObject.SetActive(leftActive);
        rightSelectButton.gameObject.SetActive(rightActive);
    }

    void shiftLevelsLeft()
    {
        levelOffset -= 3;
        FillLevelSelectContainers();
    }

    void shiftLevelsRight()
    {
        levelOffset += 3;
        FillLevelSelectContainers();
    }

    void PlayLevel()
    {
        if (selectedLevelID > 0)
        {
            SceneManager.LoadScene("Level");
        }
    }

    public void LevelClicked(GameObject container)
    {
        LevelSelectorUIScript script = container.GetComponent<LevelSelectorUIScript>();
        selectedLevelID = script.GetId();
    }
}

[System.Serializable]
public class LevelSelectContainers
{
    public List<GameObject> containers;
}