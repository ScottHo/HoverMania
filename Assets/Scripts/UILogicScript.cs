using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILogicScript : MonoBehaviour
{
    public LevelSelectContainers levelSelectContainers;
    public GameObject leaderboard;
    public GameObject createUserPopup;
    public GameObject offlinePopup;
    public GameObject demoPopup;
    public GameObject corruptPopup;
    public Button playButton;
    public Button leftSelectButton;
    public Button rightSelectButton;
    public int selectedLevelID;
    DummyDatabase databaseRepository;
    int levelOffset = 0;

    private void Start()
    {
        databaseRepository = DatabaseManager.Instance.database;
        selectedLevelID = PlayerPrefs.GetInt("SelectedLevel");
        AddListeners();
        UpdateLevelsLocked();
        FillLevelSelectContainers();
        ShowPopups();
        SetUser();
    }

    void ShowPopups()
    {
        if (PlayerPrefs.GetInt("DataFileCorruptPopup") == 1)
        {
            corruptPopup.SetActive(true);
            PlayerPrefs.SetInt("DataFileCorruptPopup", 0);
        }
        if (PlayerPrefs.GetInt("ShowDemoPopup") == 1)
        {
            demoPopup.SetActive(true);
            PlayerPrefs.SetInt("ShowDemoPopup", 0);
            return;
        }
        if (PlayerPrefs.GetInt("ShowOfflinePopup") == 1)
        {
            offlinePopup.SetActive(true);
            PlayerPrefs.SetInt("ShowOfflinePopup", 0);
            return;
        }
        if (PlayerPrefs.GetInt("ShowNewUserPopup") == 1)
        {
            createUserPopup.SetActive(true);
            PlayerPrefs.SetInt("ShowNewUserPopup", 0);
            return;
        }
    }

    void AddListeners()
    {
        playButton.onClick.AddListener(PlayLevel);
        foreach (var container in levelSelectContainers.containers)
        {
            Button button = container.GetComponent<Button>();
            button.onClick.AddListener(() => { LevelClicked(container); });
        }
        leftSelectButton.onClick.AddListener(() => { ShiftLevelsLeft(); });
        rightSelectButton.onClick.AddListener(() => { ShiftLevelsRight(); });
    }

    void UpdateLevelsLocked()
    {
        databaseRepository.SetLevelLocked(1, false);
        for (int i = 2; i <= LevelFactory.NumLevels(); i++)
        {
            if (databaseRepository.GetLevelTime(i-1) > 0)
            {
                databaseRepository.SetLevelLocked(i, false);
            }
            else
            {
                databaseRepository.SetLevelLocked(i, true);
            }
        }
        databaseRepository.Commit();
    }

    void FillLevelSelectContainers()
    {
        if (selectedLevelID > 0)
        {
            levelOffset = (selectedLevelID / 3);
        }
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
                script.SetLevelID(levelID);
                script.SetLocked(true);
            }
            else
            {
                script.SetLevelID(levelID);
                script.SetLocked(false);
                script.SetBestTime(databaseRepository.GetLevelTime(levelID));
                if (levelID == selectedLevelID)
                {
                    container.GetComponent<Button>().Select();
                    LevelClicked(container);
                }
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

    void ClearLevelSelect()
    {
        EventSystem.current.SetSelectedGameObject(null);
        selectedLevelID = -1;
        PlayerPrefs.SetInt("SelectedLevel", selectedLevelID);
        LeaderboardScript lScript = leaderboard.GetComponent<LeaderboardScript>();
        lScript.ClearLeaderboard();
    }

    void ShiftLevelsLeft()
    {
        ClearLevelSelect();
        levelOffset -= 3;
        FillLevelSelectContainers();
    }

    void ShiftLevelsRight()
    {
        ClearLevelSelect();
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
        if (selectedLevelID > 0)
        {
            LeaderboardScript lScript = leaderboard.GetComponent<LeaderboardScript>();
            lScript.ShowLevel(selectedLevelID);
        }
        PlayerPrefs.SetInt("SelectedLevel", selectedLevelID);
    }

    public void SetUser()
    {
        leaderboard.GetComponent<LeaderboardScript>().SetUser();
    }

    public void HideOfflinePopup()
    {
        offlinePopup.SetActive(false);
    }
    public void HideDemoPopup()
    {
        demoPopup.SetActive(false);
    }
    public void HideCorruptPopup()
    {
        corruptPopup.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

[System.Serializable]
public class LevelSelectContainers
{
    public List<GameObject> containers;
}