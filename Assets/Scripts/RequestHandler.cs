using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

public class RequestHandler
{
    private readonly HttpClient client;
    private string secret;
    private string baseUrl = "https://4tdeo190bi.execute-api.us-east-2.amazonaws.com/prod/hovermania";
    public RequestHandler()
    {
        client = new HttpClient();
        secret = S.K();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /*
     *  Returns in levelID : userID: timeCentiseconds
     */
    public async Task<Dictionary<string, Dictionary<string, int>>> GetHiScores(int numLevels)
    {
        var responseString = await client.GetStringAsync(baseUrl + "/hiscores?secret="
                + secret + "&levels=" + numLevels);
        Debug.Log(responseString);
        var values = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string,int>>>(responseString);
        return values;
    }

    /*
     * Returns in levelID : rank)
     */
    public async Task<Dictionary<string, int>> GetUserRank(int userID, int numLevels)
    {
        var responseString = await client.GetStringAsync(baseUrl + "/hiscores/rank?secret="
                + secret + "&levels=" + numLevels + "&user_id=" + userID);
        Debug.Log(responseString);
        var values = JsonConvert.DeserializeObject<Dictionary<string, int>>(responseString);
        return values;
    }

    public async Task<bool> AddHiScore(int userID, int levelID, int score)
    {
        var values = new Dictionary<string, string>
        {
            { "secret", secret },
            { "user_id", userID.ToString() },
            { "level", levelID.ToString() },
            { "score", score.ToString() }
        };
        var s = JsonConvert.SerializeObject(values);
        var content = new StringContent(s, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(baseUrl + "/hiscores", content);
        Debug.Log(response);
        return response.StatusCode == System.Net.HttpStatusCode.Created;
    }

    /*
     * Returns user ID or -1
     */
    public async Task<Tuple<int, UserCreatedStatus>> NewUser(string username)
    {
        var values = new Dictionary<string, string>
        {
            { "secret", secret },
            { "username", username }
        };
        var s = JsonConvert.SerializeObject(values);
        var content = new StringContent(s, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(baseUrl + "/user/create", content);
        Debug.Log(response);
        if (response.StatusCode == System.Net.HttpStatusCode.Created)
        {
            var d = JsonConvert.DeserializeObject<Dictionary<string, int>>(await response.Content.ReadAsStringAsync());
            var i = d.GetValueOrDefault("user_id", -1);
            if (i > -1)
            {
                return new Tuple<int, UserCreatedStatus>(i, UserCreatedStatus.Success);
            }
        }
        UserCreatedStatus status = await ParseUserMessage(response);
        return new Tuple<int, UserCreatedStatus>(-1, status);
    }

    /*
    * Returns UserCreatedStatus
    */
    public async Task<UserCreatedStatus> ChangeUsername(int userID, string username)
    {
        var values = new Dictionary<string, string>
        {
            { "secret", secret },
            { "user_id", userID.ToString() },
            { "username", username }
        };
        var s = JsonConvert.SerializeObject(values);
        var content = new StringContent(s, Encoding.UTF8, "application/json");
        var response = await client.PutAsync(baseUrl + "/user/changeUsername", content);
        Debug.Log(response);
        return await ParseUserMessage(response);
        
    }

    async Task<UserCreatedStatus> ParseUserMessage(HttpResponseMessage response)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
        {
            return UserCreatedStatus.Success;
        }
        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var d = await response.Content.ReadAsStringAsync();
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
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return UserCreatedStatus.UsernameExists;
        }
        return UserCreatedStatus.UnknownError;
    }


}
