using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelSelectorUIScript : MonoBehaviour
{
    public TextMeshProUGUI levelName;
    public TextMeshProUGUI bestTime;
    public Image levelImage;
    public Button levelButton;
    int id = -1;

    public void SetLevelInfo(LevelInfo levelInfo)
    {
        levelName.text = levelInfo.levelName;
        SetImageSprite(levelInfo.spriteName);
        id = levelInfo.id;
        levelButton.interactable = true;
    }

    public int GetId()
    {
        return id;
    }
    void SetImageSprite(string spriteName)
    {
        levelImage.sprite = Resources.Load<Sprite>(spriteName);
    }
    public void SetBestTime(int timeCentiseconds)
    {
        if (timeCentiseconds > 0)
        {
            TimeSpan time = TimeSpan.FromMilliseconds(timeCentiseconds * 10);
            bestTime.text = time.ToString(@"mm\:ss\.ff");
            bestTime.color = Color.green;
        }
    }

    public void SetLocked(bool locked)
    {
        if (locked)
        {
            bestTime.text = "LOCKED";
            bestTime.color = Color.red;
            levelButton.interactable = false;
        }
        else
        {
            bestTime.text = "INCOMPLETE";
            bestTime.color = Color.red;
            levelButton.interactable = true;
        }
    }
}
