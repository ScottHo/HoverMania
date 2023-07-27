using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class UILogicScript : MonoBehaviour
{
    public LevelSelectContainers levelSelectContainers;
    public Button playButton;
    int selectedLevelID = -1;

    void Start()
    {
        playButton.onClick.AddListener(PlayLevel);
        foreach (var container in levelSelectContainers.containers)
        {
            Button button = container.GetComponent<Button>();
            button.onClick.AddListener(() => { LevelClicked(container); });
        }
        levelSelectContainers.containers[0].GetComponent<Button>().Select();
    }

    void PlayLevel()
    {
        SceneManager.LoadScene("Level");
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