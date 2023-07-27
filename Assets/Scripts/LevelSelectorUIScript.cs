using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LevelSelectorUIScript : MonoBehaviour
{
    Difficulty difficulty;
    TextMeshPro levelName;
    Image levelImage;
    int id = -1;

    public void SetLevelName(string name)
    {
        levelName.text = name;
    }
    public void SetDifficulty(int i)
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

    public void setId(int id)
    {
        this.id = id;
    }

    public int GetId()
    {
        return id;
    }
    public void setImageSprite(Sprite sprite)
    {
        levelImage.sprite = sprite;
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