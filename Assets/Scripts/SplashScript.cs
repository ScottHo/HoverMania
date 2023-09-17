using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Net.Http;
using System.Collections;
using UnityEngine.Networking;

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

        StartCoroutine(SyncAndWait());
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

    IEnumerator SyncAndWait()
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

        var request = CloudSync.GetHiScoresRequest();
        yield return request.SendWebRequest();
        bool finishWithError = false;

        try
        {
            CloudSync.ParseGetHiScoresRequest(request);
        } catch (WebRequestException e) {
            Debug.LogException(e);
            finishWithError = true;
        }
        if (!finishWithError)
        {
            int newUserFlag = 0;
            if (database.GetUserID() == -1)
            {
                newUserFlag = 1;
            }
            PlayerPrefs.SetInt("ShowNewUserPopup", newUserFlag);

            if (newUserFlag == 0)
            {
                var otherRequest = CloudSync.GetUserRankRequest();
                yield return otherRequest.SendWebRequest();
                try
                {
                    CloudSync.ParseGetUserRankRequest(otherRequest);
                }
                catch (WebRequestException e)
                {
                    Debug.LogException(e);
                    finishWithError = true;
                }
            }
        }

        if (!finishWithError)
        {
            PlayerPrefs.SetInt("LeaderboardConnected", 1);
            PlayerPrefs.SetInt("ShowOfflinePopup", 0);
        }
        else
        {
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
