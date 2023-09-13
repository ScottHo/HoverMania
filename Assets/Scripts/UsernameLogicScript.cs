using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Net.Http;

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
    IDatabaseRepository databaseRepository;

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

    public async void ChangeUsername()
    {
        if (!ValidateUsername())
        {
            return;
        }
        try
        {
            changeUsernameButton.enabled = false;
            cancelButton.enabled = false;
            errorText.text = "Syncing...";
            errorText.color = Color.white;
            var status = await CloudSync.ChangeUsername(inputField.text);
            if (status == UserCreatedStatus.UsernameExists)
            {
                errorText.text = "Username already exists";
                errorText.color = Color.red;
                changeUsernameButton.enabled = true;
                cancelButton.enabled = true;
            }
            await CloudSync.GetHiScores();
            leaderboard.GetComponent<LeaderboardScript>().SetUser();
            changeUsernameButton.enabled = true;
            cancelButton.enabled = true;
            Hide();
        }
        catch (HttpRequestException e)
        {
            Debug.LogException(e);
            PlayerPrefs.SetInt("LeaderboardConnected", 0);
            GoOffline();
        }
    }

    public async void NewUser()
    {
        if (!ValidateUsername())
        {
            return;
        }
        try
        {
            var status = await CloudSync.NewUser(inputField.text);
            if (status == UserCreatedStatus.UsernameExists)
            {
                errorText.text = "Username already exists";
                return;
            }
            leaderboard.GetComponent<LeaderboardScript>().SetUser();
            Hide();
        }
        catch (HttpRequestException e)
        {
            Debug.LogException(e);
            PlayerPrefs.SetInt("LeaderboardConnected", 0);
            GoOffline();
        }
    }
}
