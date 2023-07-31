using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILogicScript : MonoBehaviour
{
    public IDatabaseRepository databaseRepository;
    public LevelSelectContainers levelSelectContainers;
    public Button playButton;
    public int selectedLevelID;
    public static UILogicScript Instance;
    int levelOffset = 1;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupDatabase();
        }
        else if (Instance != this)
        {
            //Instance is not the same as the one we have, destroy old one, and reset to newest one
            Destroy(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupDatabase();
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
                databaseRepository.SwitchUser(1);
            }
            catch (SqliteException)
            {
                databaseRepository.CreateUser();
            }
        }
    }

    private void Start()
    {
        playButton.onClick.AddListener(PlayLevel);
        foreach (var container in levelSelectContainers.containers)
        {
            Button button = container.GetComponent<Button>();
            button.onClick.AddListener(() => { LevelClicked(container); });
        }
        FillLevelSelectContainers();
    }

    void FillLevelSelectContainers()
    {
        foreach (var it in levelSelectContainers.containers.Select((Value, Index) => new { Value, Index }))
        {
            GameObject container = it.Value;
            LevelInfo info = LevelFactory.GetLevelInfo(it.Index + levelOffset);
            LevelSelectorUIScript script = container.GetComponent<LevelSelectorUIScript>();
            script.SetLevelInfo(info);
            script.SetBestTime(databaseRepository.GetLevelTime(info.id));
        }
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