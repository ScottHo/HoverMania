using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Net.Http;

public class SplashScript : MonoBehaviour
{
    public RawImage image;
    public float duration;
    bool fadingIn = true;
    bool startupFinished = false;


    private void Start()
    {
        startupFinished = false;
        image.color = Color.black;
        PlayerPrefs.SetInt("SelectedLevel", -1);
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            PlayerPrefs.SetInt("IsDemo", 1);
        }
        else
        {
            PlayerPrefs.SetInt("IsDemo", 0);
        }

        SyncAndWait();
    }

    void FixedUpdate()
    {
        if (fadingIn)
        {
            FadeIn();
        }
        else
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                FadeOut();
            }
        }
    }

    async void SyncAndWait()
    {
        DummyDatabase database = DatabaseManager.Instance.database;
        PlayerPrefs.SetInt("DataFileCorruptPopup", 0);
        if (database.dataBinCorrupt)
        {
            PlayerPrefs.SetInt("DataFileCorruptPopup", 1);
            database.dataBinCorrupt = false;
        }
        if (LevelFactory.NumLevels() == 3)
        {
            PlayerPrefs.SetInt("ShowDemoPopup", 1);
        }
        PlayerPrefs.SetInt("ShowDemoPopup", 0);

        try
        {
            await CloudSync.GetHiScores();
            await CloudSync.SyncCurrentUser();
            PlayerPrefs.SetInt("LeaderboardConnected", 1);
            int newUserFlag = 0;
            if (database.GetUserID() == -1)
            {
                newUserFlag = 1;
            }
            PlayerPrefs.SetInt("ShowNewUserPopup", newUserFlag);
            PlayerPrefs.SetInt("ShowOfflinePopup", 0);
        }
        catch (HttpRequestException e)
        {
            Debug.LogException(e);
            PlayerPrefs.SetInt("LeaderboardConnected", 0);
            PlayerPrefs.SetInt("ShowNewUserPopup", 0);
            PlayerPrefs.SetInt("ShowOfflinePopup", 1);
        }
        startupFinished = true;
    }

    void FadeIn()
    {
        Color color = image.color;
        color.b += .05f;
        color.r += .05f;
        color.g += .05f;
        image.color = color;
        if (color.b >= 1)
        {
            fadingIn = false;
        }
    }
    
    void FadeOut()
    {
        Color color = image.color;
        color.b -= .05f;
        color.r -= .05f;
        color.g -= .05f;
        image.color = color;
        if (color.b <= 0 && startupFinished)
        {
            SceneManager.LoadScene("UI");
        }
    }
}
