using System;
using System.Collections.Generic;

public class DummyDatabase : IDatabaseRepository
{
    Dictionary<int, int> _scoresDict = new Dictionary<int, int>();
    Dictionary<int, bool> _lockedDict = new Dictionary<int, bool>();


    public DummyDatabase(string connection)
    {
    }

    ~DummyDatabase()
    {
    }

    public void SetUser(int user_id, string username)
    {
    }

    public string GetUsername()
    {
        return "";
    }

    public int GetUserID()
    {
        return -1;
    }

    public int GetLevelTime(int levelID)
    {
        if (_scoresDict.ContainsKey(levelID))
        {
            return _scoresDict[levelID];
        }
        return -1;
    }

    public void SetLevelTime(int levelID, int timeCentiseconds)
    {
        if (_scoresDict.ContainsKey(levelID))
        {
            if (timeCentiseconds < _scoresDict[levelID])
                _scoresDict[levelID] = timeCentiseconds;
        }
    }

    public void SetLevelLocked(int levelID, bool locked)
    {
        _lockedDict[levelID] = locked;
    }

    public bool GetLevelLocked(int levelID)
    {
        if (_lockedDict.ContainsKey(levelID))
        {
            return _lockedDict[levelID];
        }
        return true;
    }

    public void ClearLeaderboard()
    {

    }

    public void AddToLeaderboard(string username, int levelID, int rank, int timeCentiseconds)
    {
    
    }

    public int GetLeaderboardRank(string username, int levelID)
    {
        return 1;
    }

    public Tuple<string, int> GetLeaderboardUserScores(int levelID, int rank)
    {
        return new Tuple<string, int>("1", 1);
    }
}
