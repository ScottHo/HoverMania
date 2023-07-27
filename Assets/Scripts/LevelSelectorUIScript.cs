using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LevelSelectorUIScript : MonoBehaviour
{
    public Difficulty difficulty;
    public TextMeshProUGUI levelName;
    public Image levelImage;
    int id = -1;

    public void SetLevelInfo(LevelInfo levelInfo)
    {
        levelName.text = levelInfo.levelName;
        SetDifficulty(levelInfo.difficulty);
        SetImageSprite(levelInfo.spriteName);
        id = levelInfo.id;
    }
    void SetDifficulty(int i)
    {
        difficulty.star2.enabled = false;
        difficulty.star3.enabled = false;
        difficulty.star4.enabled = false;
        if (i > 1)
        {
            difficulty.star2.enabled = true;
        }
        if (i > 2)
        {
            difficulty.star3.enabled = true;
        }
        if (i > 3)
        {
            difficulty.star4.enabled = true;
        }
    }

    public int GetId()
    {
        return id;
    }
    void SetImageSprite(string spriteName)
    {
        levelImage.sprite = Resources.Load<Sprite>(spriteName);
    }
}

[System.Serializable]
public class Difficulty
{
    public Image star1;
    public Image star2;
    public Image star3;
    public Image star4;
}