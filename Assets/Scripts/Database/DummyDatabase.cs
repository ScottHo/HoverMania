using System.Collections.Generic;

public class DummyDatabase : IDatabaseRepository
{
    int _money = 0;
    Dictionary<int, int> _scoresDict = new Dictionary<int, int>();

    public DummyDatabase(string connection)
    {
    }

    ~DummyDatabase()
    {
    }

    public int CreateUser()
    {
        return 1;
    }

    public void SwitchUser(int user_id)
    {
    }

    public void SetMoney(int money)
    {
        _money = money;
    }
    public int Money()
    {
        return _money;
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
        _scoresDict[levelID] = timeCentiseconds;
    }
}
