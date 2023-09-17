using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class CloudSync
{
    public static DummyDatabase databaseRepository = DatabaseManager.Instance.database;
    static string baseUrl = "https://4tdeo190bi.execute-api.us-east-2.amazonaws.com/prod/hovermania";

    public static UnityWebRequest GetHiScoresRequest()
    {
        databaseRepository.ClearLeaderboard();
        UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/hiscores?secret="
        + S.K() + "&levels=" + LevelFactory.NumLevels());
        request.SetRequestHeader("x-api-key", S.A());
        return request;
    }

    /*
     * From the top X hi scores from the server, order them by rank, add them to the local database
     */
    public static void ParseGetHiScoresRequest(UnityWebRequest response)
    {
        if (response.responseCode == 200)
        {
            var responseString = response.downloadHandler.text;
            var scores = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(responseString);
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
    }
    
    public static UnityWebRequest GetUserRankRequest()
    {
        int userID = databaseRepository.GetUserID();
        if (userID < 0)
            throw new WebRequestException("Invalid user_id in local data. Please restart game.");
        UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/hiscores/rank?secret="
                + S.K() + "&levels=" + LevelFactory.NumLevels() + "&user_id=" + userID);
        request.SetRequestHeader("x-api-key", S.A());
        return request;
    }

    public static void ParseGetUserRankRequest(UnityWebRequest request)
    {
        if (request.responseCode == 200)
        {
            var responseString = request.downloadHandler.text;
            var scores = JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(responseString);
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
            return;
        }
        throw new WebRequestException("Unknown Error");
    }

    public static UnityWebRequest AddHiScoreRequest(int levelID, int timeCentiseconds)
    {
        int userID = databaseRepository.GetUserID();
        if (userID < 0)
            throw new WebRequestException("Invalid user_id in local data. Please restart game.");
        var values = new Dictionary<string, string>
        {
            { "secret", S.K() },
            { "user_id", userID.ToString() },
            { "level", levelID.ToString() },
            { "score", timeCentiseconds.ToString() }
        };

        var s = JsonConvert.SerializeObject(values);
        UnityWebRequest request = UnityWebRequest.Post(baseUrl + "/hiscores", s, "application/json");
        request.SetRequestHeader("x-api-key", S.A());
        return request;
    }

    public static UnityWebRequest ChangeUsernameRequest(string username)
    {
        int userID = databaseRepository.GetUserID();
        if (userID < 0)
            throw new WebRequestException("Invalid user_id in local data. Please restart game.");
        var values = new Dictionary<string, string>
        {
            { "secret", S.K() },
            { "user_id", userID.ToString() },
            { "username", username }
        };
        var s = JsonConvert.SerializeObject(values);
        UnityWebRequest request = UnityWebRequest.Post(baseUrl + "/user/changeUsername", s, "application/json");
        request.SetRequestHeader("x-api-key", S.A());

        return request;
    }

    public static UserCreatedStatus ParseChangeUsernameRequest(UnityWebRequest request)
    {
        return ParseUserMessage(request);
    }

    public static UnityWebRequest NewUserRequest(string username)
    {
        var values = new Dictionary<string, string>
        {
            { "secret", S.K() },
            { "username", username }
        };
        var s = JsonConvert.SerializeObject(values);
        UnityWebRequest request = UnityWebRequest.Post(baseUrl + "/user/create", s, "application/json");
        request.SetRequestHeader("x-api-key", S.A());
        return request;
    }

    public static Tuple<int, UserCreatedStatus> ParseNewUserRequest(UnityWebRequest request)
    {
        if (request.responseCode == 201)
        {
            var s = request.downloadHandler.text;
            var d = JsonConvert.DeserializeObject<Dictionary<string, int>>(s);
            var i = d.GetValueOrDefault("user_id", -1);
            if (i > -1)
            {
                return new Tuple<int, UserCreatedStatus>(i, UserCreatedStatus.Success);
            }
        }
        UserCreatedStatus status = ParseUserMessage(request);
        return new Tuple<int, UserCreatedStatus>(-1, status);
    }

    static UserCreatedStatus ParseUserMessage(UnityWebRequest request)
    {
        if (request.responseCode == 202)
        {
            return UserCreatedStatus.Success;
        }
        if (request.responseCode == 400)
        {
            var d = request.downloadHandler.text;
            if (d.Contains("User ID not found"))
            {
                return UserCreatedStatus.InvalidID;
            }
            if (d.Contains("Username can only contain alpha numeric and underscore") ||
                d.Contains("Username must be in between 3-16 characters"))
            {
                return UserCreatedStatus.InvalidUsername;
            }
            return UserCreatedStatus.UnknownError;
        }
        if (request.responseCode == 200)
        {
            return UserCreatedStatus.UsernameExists;
        }
        return UserCreatedStatus.UnknownError;
    }
}
