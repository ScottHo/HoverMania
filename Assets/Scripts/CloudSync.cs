using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

public class CloudSync
{
    public static DummyDatabase databaseRepository = DatabaseManager.Instance.database;
    public static RequestHandler requestHandler = new RequestHandler();
    /*
     * Get the top X hi scores from the server, order them by rank, add them to the local database
     */
    public static async Task GetHiScores()
    {
        databaseRepository.ClearLeaderboard();
        var scores = await requestHandler.GetHiScores(LevelFactory.NumLevels(false));
        foreach (var level in scores)
        {
            List<Tuple<string, int>> t = new List<Tuple<string, int>>();
            foreach (var user in level.Value)
            {
                t.Add(Tuple.Create(user.Key, user.Value));
            }
            t = t.OrderBy(x => x.Item2).ToList();
            int i = 1;
            foreach (var user in t)
            {
                databaseRepository.AddToLeaderboard(user.Item1, int.Parse(level.Key), i, user.Item2);
                i++;
            }
        }
        databaseRepository.Commit();
    }

    public static async Task SyncCurrentUser()
    {
        int userID = databaseRepository.GetUserID();
        if (userID < 0)
            return;
        var scores = await requestHandler.GetUserRank(userID, LevelFactory.NumLevels(false));
        foreach (var level in scores)
        {
            if (level.Value.Count == 2)
            {
                int score = level.Value[0];
                int rank = level.Value[1];
                databaseRepository.AddToLeaderboard(databaseRepository.GetUsername(), level.Key, rank, score);
                databaseRepository.SetLevelTime(level.Key, score);
            }
        }
        databaseRepository.Commit();
    }

    public static async Task UploadHiScore(int levelID, int timeCentiseconds)
    {
        int userID = databaseRepository.GetUserID();
        if (userID < 0)
            return;
        await requestHandler.AddHiScore(userID, levelID, timeCentiseconds);
        await GetHiScores();
        await SyncCurrentUser();
    }

    public static async Task<UserCreatedStatus> ChangeUsername(string username)
    {
        int userID = databaseRepository.GetUserID();
        if (userID < 0)
            return UserCreatedStatus.InvalidID;
        var status =  await requestHandler.ChangeUsername(userID, username);
        if (status == UserCreatedStatus.Success)
        {
            databaseRepository.SetUser(userID, username);
            databaseRepository.Commit();
        }
        return status;
    }

    public static async Task<UserCreatedStatus> NewUser(string username)
    {
        var v = await requestHandler.NewUser(username);
        if (v.Item2 == UserCreatedStatus.Success)
        {
            databaseRepository.SetUser(v.Item1, username);
            databaseRepository.Commit();
        }
        return v.Item2;
    }
}
