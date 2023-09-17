using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectorUIScript : MonoBehaviour
{
    public TextMeshProUGUI levelName;
    public TextMeshProUGUI bestTime;
    public Image levelImage;
    public Button levelButton;
    int id = -1;

    public void SetLevelID(int levelID)
    {
        var levelInfo = LevelFactory.GetLevelInfo(levelID);
        SetImageSprite(levelInfo.spriteName);
        id = levelID;
        levelName.text = "Level " + id;
        if (id == -1)
        {
            levelName.text = "???";
            levelButton.interactable = false;
        }
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
            bestTime.text = Utils.ToTimeString(timeCentiseconds);
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
            SetImageSprite("UnknownThumbnail");
        }
        else
        {
            bestTime.text = "INCOMPLETE";
            bestTime.color = Color.white;
            levelButton.interactable = true;
        }
    }
}
