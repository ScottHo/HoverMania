using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LeaderboardScript : MonoBehaviour
{
    public LeaderboardRow[] leaderboards;
    public TextMeshProUGUI header;
    public LeaderboardRow youLeaderboard;


    public void Start()
    {
        CheckOffline();
    }

    bool CheckOffline()
    {
        if (PlayerPrefs.GetInt("LeaderboardConnected") == 0)
        {
            GoOffline();
            return true;
        }
        return false;
    }

    public void ClearLeaderboard()
    {
        for (int r = 1; r <= 5; r++)
        {
            leaderboards[r - 1].userText.text = "-";
            leaderboards[r - 1].timeText.text = "-- : -- : --";
        }
        youLeaderboard.rankText.text = "?";
        youLeaderboard.star.color = Color.white;
        youLeaderboard.timeText.text = "-- : -- : --";
    }

    public void SetUser()
    {
        if (CheckOffline())
            return;
        DummyDatabase databaseRepository = DatabaseManager.Instance.database;
        var username = databaseRepository.GetUsername();
        if (username != "")
        {
            youLeaderboard.userText.text = username;
        }
    }

    public void ShowLevel(int level)
    {
        if (CheckOffline())
            return;
        ClearLeaderboard();
        DummyDatabase databaseRepository = DatabaseManager.Instance.database;
        for (int r = 1; r <= 5; r++)
        {
            var userScores = databaseRepository.GetLeaderboardUserScores(level, r);
            Debug.Log(userScores.ToString());
            if (userScores.Item2 > 0)
            {
                leaderboards[r - 1].userText.text = userScores.Item1;
                leaderboards[r - 1].timeText.text = Utils.ToTimeString(userScores.Item2);
            }
        }
        string username = databaseRepository.GetUsername();
        if (username == "")
        {
            return;
        }
        int rank = databaseRepository.GetLeaderboardRank(username, level);
        if (rank == -1)
        {
            return;
        }
        youLeaderboard.rankText.text = rank.ToString();
        var userScore = databaseRepository.GetLeaderboardUserScores(level, rank);
        if (userScore.Item2 < 1)
        {
            return;
        }
        youLeaderboard.timeText.text = Utils.ToTimeString(userScore.Item2);
        if (rank == 1)
        {
            youLeaderboard.star.color = leaderboards[0].star.color;
            return;
        }
        if (rank == 2)
        {
            youLeaderboard.star.color = leaderboards[1].star.color;
            return;
        }
        if (rank == 3)
        {
            youLeaderboard.star.color = leaderboards[2].star.color;
            return;
        }
        if (rank <= 10)
        {
            youLeaderboard.star.color = leaderboards[3].star.color;
            return;
        }
        youLeaderboard.star.color = Color.white;
    }

    public void GoOffline()
    {
        ClearLeaderboard();
        header.text = "OFFLINE";
        header.color = Color.red;
        youLeaderboard.userText.text = "-";
    }
}

[Serializable]
public class LeaderboardRow
{
    public TextMeshProUGUI userText;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI timeText;
    public Image star;
}