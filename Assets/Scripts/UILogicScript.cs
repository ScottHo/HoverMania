using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILogicScript : MonoBehaviour
{
    public LevelSelectContainers levelSelectContainers;
    public Button playButton;
    public int selectedLevelID;
    public static UILogicScript Instance;
    IDatabaseRepository databaseRepository;
    int levelOffset = 1;


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