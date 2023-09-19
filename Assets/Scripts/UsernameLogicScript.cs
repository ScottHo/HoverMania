using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class UsernameLogicScript : MonoBehaviour
{
    public TextMeshProUGUI placeholder;
    public TextMeshProUGUI errorText;
    public TMP_InputField inputField;
    public GameObject container;
    public GameObject offlineCointainer;
    public Button offlineButton;
    public Button newUserButton;
    public Button changeUsernameButton;
    public Button cancelButton;
    public GameObject leaderboard;
    DummyDatabase databaseRepository;

    void Start()
    {
        databaseRepository = DatabaseManager.Instance.database;
        ResetValues();
        if (offlineButton != null )
            offlineButton.onClick.AddListener(() => { GoOffline(); });

        if (newUserButton != null)
            newUserButton.onClick.AddListener(() => { NewUser(); });

        if (changeUsernameButton != null)
            changeUsernameButton.onClick.AddListener(() => { ChangeUsername(); });

        if (cancelButton != null)
            cancelButton.onClick.AddListener(() => { Hide(); });

    }

    public void ResetValues()
    {
        errorText.text = string.Empty;
        errorText.color = Color.red;
        inputField.text = string.Empty;
        placeholder.text = databaseRepository.GetUsername();
    }

    public void Show()
    {
        if (PlayerPrefs.GetInt("LeaderboardConnected") == 0)
        {
            return;
        }
        ResetValues();
        container.SetActive(true);
        changeUsernameButton.enabled = true;
        cancelButton.enabled = true;
    }

    public void Hide()
    {
        container.SetActive(false);
    }

    public void GoOffline()
    {
        PlayerPrefs.SetInt("LeaderboardConnected", 0);
        offlineCointainer.SetActive(true);
        Hide();
    }

    public bool ValidateUsername()
    {
        string username = inputField.text;
        if (!Regex.Match(username, "^[A-Za-z0-9_-]*$").Success)
        {
            errorText.text = "Username can only contain alphanumeric characters and underscores";
            return false;
        }
        if (username.Length < 3 || username.Length > 16)
        {
            errorText.text = "Username must be 3-16 characters long";
            return false;
        }
        errorText.text = string.Empty;
        return true;
    }

    public void ChangeUsername()
    {
        if (!ValidateUsername())
        {
            return;
        }
        changeUsernameButton.enabled = false;
        cancelButton.enabled = false;
        errorText.text = "Syncing...";
        errorText.color = Color.white;
        StartCoroutine(DoChangeUsername());
    }

    IEnumerator DoChangeUsername()
    {
        UnityWebRequest request = null;
        try
        {
            request = CloudSync.ChangeUsernameRequest(inputField.text);
        } catch (WebRequestException e)
        {
            errorText.text = e.Message;
        }
        if (request != null)
        {
            yield return request.SendWebRequest();
            var status = CloudSync.ParseChangeUsernameRequest(request);
            if (status == UserCreatedStatus.Success)
            {
                databaseRepository.SetUser(databaseRepository.GetUserID(), inputField.text);
                databaseRepository.Commit();
                var otherRequest = CloudSync.GetHiScoresRequest();
                yield return otherRequest.SendWebRequest();
                CloudSync.ParseGetHiScoresRequest(otherRequest);
                var thirdRequest = CloudSync.GetUserRankRequest();
                yield return thirdRequest.SendWebRequest();
                CloudSync.ParseGetUserRankRequest(thirdRequest);
                leaderboard.GetComponent<LeaderboardScript>().SetUser();
                changeUsernameButton.enabled = true;
                cancelButton.enabled = true;
                Hide();
            }

            else if (status == UserCreatedStatus.UsernameExists)
            {
                errorText.text = "Username already exists";
                errorText.color = Color.red;
                changeUsernameButton.enabled = true;
                cancelButton.enabled = true;
            }
            else
            {
                GoOffline();
            }
        }
    }

    public void NewUser()
    {
        if (!ValidateUsername())
        {
            return;
        }
        StartCoroutine(DoNewUser());
        
    }
    
    IEnumerator DoNewUser()
    {
        var request = CloudSync.NewUserRequest(inputField.text);
        yield return request.SendWebRequest();
        var response = CloudSync.ParseNewUserRequest(request);
        var status = response.Item2;
        var user_id = response.Item1;
        if (status == UserCreatedStatus.UsernameExists)
        {
            errorText.text = "Username already exists";
        }
        else if (status == UserCreatedStatus.Success)
        {
            databaseRepository.SetUser(user_id, inputField.text);
            databaseRepository.Commit();
            leaderboard.GetComponent<LeaderboardScript>().SetUser();
            Hide();
        }
        else
        {
            GoOffline();
        }
    }
}
