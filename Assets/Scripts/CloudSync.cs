using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Codice.Client.Common;

public class CloudSync
{
    public static IDatabaseRepository databaseRepository = DatabaseManager.Instance.database;
    public static RequestHandler requestHandler = new RequestHandler();
    /*
     * Get the top X hi scores from the server, order them by rank, add them to the local database
     */
    public static async void GetHiScores()
    {
        databaseRepository.ClearLeaderboard();
        var scores = await requestHandler.GetHiScores(LevelFactory.NumLevels());
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
    }

    public static async void UploadHiScore(int levelID, int timeCentiseconds)
    {
        int userID = databaseRepository.GetUserID();
        if (userID < 0)
            return;
        await requestHandler.AddHiScore(userID, levelID, timeCentiseconds);
        GetHiScores();
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
        }
        return status;
    }

    public static async Task<UserCreatedStatus> NewUser(string username)
    {
        var v = await requestHandler.NewUser(username);
        if (v.Item2 == UserCreatedStatus.Success)
        {
            databaseRepository.SetUser(v.Item1, username);
        }
        return v.Item2;
    }
}
