using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DummyDatabase
{
    string path;
    _Database _database;
    public bool dataBinCorrupt = false;

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SaveToLocalStorage(string data);

    [DllImport("__Internal")]
    private static extern string LoadFromLocalStorage();

    [DllImport("__Internal")]
    private static extern int DataExistsInLocalStorage();
#endif

    [Serializable]
    struct _Database
    {
        public Dictionary<int, int> _scoresDict;
        public Dictionary<int, bool> _lockedDict;
        public Dictionary<int, List<_Leaderboard>> _leaderboardDict;
        public string username;
        public int userID;
    }

    [Serializable]
    struct _Leaderboard
    {
        public string user;
        public int score;
        public int rank;
    }

    public DummyDatabase(string path)
    {
        this.path = path;
        dataBinCorrupt = false;
        _database = Empty();

#if UNITY_WEBGL
        if (DataExistsInLocalStorage() == 1)
        {
            var s = LoadFromLocalStorage();
            if (s != null)
            {
                var bytes = Convert.FromBase64String(s);
                {
                    _database = Deserialize(new MemoryStream(bytes));
                }
            }
        }
            
#else
        if (File.Exists(path))
        {
            try
            {
                _database = Deserialize(File.Open(path, FileMode.Open));
            }
            catch (Exception e)
            {
                Debug.Log(e);
                _database = Empty();
            }
        }
#endif
        InitDatabase();
    }

    _Database Empty()
    {
        _Database ret = new _Database();
        ret._scoresDict = new Dictionary<int, int>();
        ret._lockedDict = new Dictionary<int, bool>();
        ret._leaderboardDict = new Dictionary<int, List<_Leaderboard>>();
        for (int i = 1; i <= LevelFactory.NumLevels(); i++)
        {
            ret._leaderboardDict[i] = new List<_Leaderboard>();
        }
        ret.username = "";
        ret.userID = -1;
        return ret;
    }

    void InitDatabase()
    {
        for (int i = 1; i <= LevelFactory.NumLevels(); i++)
        {
            if (!_database._leaderboardDict.ContainsKey(i))
            {
                _database._leaderboardDict[i] = new List<_Leaderboard>();
            }
        }
    }

    byte[] Serialize<Object>(Object obj, Stream stream)
    {
        try // try to serialize the collection to a file
        {
            BinaryFormatter bin = new BinaryFormatter();
            bin.Serialize(stream, obj);
            if (stream is MemoryStream memStream)
            {
                return memStream.ToArray();
            }
        }
        catch (IOException e)
        {
            Debug.Log(e);
        }
        catch (SerializationException e)
        {
            Debug.Log(e);
        }
        return new byte[0];
    }
    _Database Deserialize(Stream stream)
    {
        _Database ret = new _Database();
        try
        {
            using (stream)
            {
                BinaryFormatter bin = new BinaryFormatter();
                ret = (_Database)bin.Deserialize(stream);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            dataBinCorrupt = true;
            return Empty();
        }
        return ret;
    }

    public void Commit()
    {
#if UNITY_WEBGL
        MemoryStream stream = new MemoryStream();
        var bytes = Serialize(_database, stream);
        SaveToLocalStorage(Convert.ToBase64String(bytes));
#else
        Serialize(_database, File.Open(path, FileMode.Create));
#endif
    }

    public void SetUser(int user_id, string username)
    {
        _database.userID = user_id;
        _database.username = username;
    }

    public string GetUsername()
    {
        return _database.username;
    }

    public int GetUserID()
    {
        return _database.userID;
    }

    public int GetLevelTime(int levelID)
    {
        if (_database._scoresDict.ContainsKey(levelID))
        {
            return _database._scoresDict[levelID];
        }
        return -1;
    }

    public void SetLevelTime(int levelID, int timeCentiseconds)
    {
        if (_database._scoresDict.ContainsKey(levelID))
        {
            if (timeCentiseconds < _database._scoresDict[levelID])
                _database._scoresDict[levelID] = timeCentiseconds;
        }
        else
        {
            _database._scoresDict[levelID] = timeCentiseconds;
        }
    }

    public void SetLevelLocked(int levelID, bool locked)
    {
        _database._lockedDict[levelID] = locked;
    }

    public bool GetLevelLocked(int levelID)
    {
        if (_database._lockedDict.ContainsKey(levelID))
        {
            return _database._lockedDict[levelID];
        }
        return true;
    }

    public void ClearLeaderboard()
    {
        foreach (var l in _database._leaderboardDict)
        {
            l.Value.Clear();
        }
    }

    public void AddToLeaderboard(string username, int levelID, int rank, int timeCentiseconds)
    {
        var l = new _Leaderboard();
        l.user = username;
        l.rank = rank;
        l.score = timeCentiseconds;
        _database._leaderboardDict[levelID].Add(l);
    }

    public int GetLeaderboardRank(string username, int levelID)
    {
        foreach (var l in _database._leaderboardDict[levelID])
        {
            if (l.user == username)
            {
                return l.rank;
            }
        }
        return -1;
    }

    public Tuple<string, int> GetLeaderboardUserScores(int levelID, int rank)
    {
        foreach (var l in _database._leaderboardDict[levelID])
        {
            if (l.rank == rank)
            {
                return new Tuple<string, int>(l.user, l.score);
            }
        }
        return new Tuple<string, int>("", -1);
    }

    public int totalTime()
    {
        int t = 0;
        foreach (var k in _database._scoresDict.Keys)
        {
            int _t = GetLevelTime(k);
            t += _t;
        }
        return t;
    }

    public bool allLevelsComplete()
    {
        return _database._scoresDict.Keys.Count == LevelFactory.NumLevels();
    }
}
