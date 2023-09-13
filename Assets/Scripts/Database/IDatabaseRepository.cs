
using System;

public interface IDatabaseRepository
{
    void SetUser(int user_id, string username);

    string GetUsername();

    int GetUserID();

    void SetLevelTime(int levelID, int timeCentiseconds);

    int GetLevelTime(int levelID);

    void SetLevelLocked(int levelID, bool locked);

    bool GetLevelLocked(int levelID);

    void ClearLeaderboard();

    void AddToLeaderboard(string username, int levelID, int rank, int timeCentiseconds);

    int GetLeaderboardRank(string username, int levelID);

    Tuple<string, int> GetLeaderboardUserScores(int levelID, int rank);
}

