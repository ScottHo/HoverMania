using NUnit.Framework;

public class DatabaseTests
{
    DummyDatabase db;
    [SetUp]
    public void SetUp()
    {
        db = new DummyDatabase("");
    }

    [Test]
    public void TestScores()
    {
        int levelTime = db.GetLevelTime(1);
        Assert.AreEqual(levelTime, -1);
        db.SetLevelTime(1, 999);
        levelTime = db.GetLevelTime(1);
        Assert.AreEqual(levelTime, 999);
        levelTime = db.GetLevelTime(2);
        Assert.AreEqual(levelTime, -1);
        db.SetLevelTime(2, 999);
        levelTime = db.GetLevelTime(2);
        Assert.AreEqual(levelTime, 999);
    }


    [Test]
    public void TestLocked()
    {
        bool locked = db.GetLevelLocked(1);
        Assert.True(locked);
        db.SetLevelLocked(1, false);
        locked = db.GetLevelLocked(1);
        Assert.False(locked);
        db.SetLevelLocked(1, true);
        locked = db.GetLevelLocked(1);
        Assert.True(locked);
    }

    [Test]
    public void TestUsers()
    {
        var user = db.GetUsername();
        Assert.AreEqual("", user);

        db.SetUser(3, "Test");

        user = db.GetUsername();
        Assert.AreEqual("Test", user);

        var userID = db.GetUserID();
        Assert.AreEqual(3, userID);
    }

    [Test]
    public void TestLeaderboard()
    {
        db.AddToLeaderboard("test", 1, 1, 9999);
        int rank = db.GetLeaderboardRank("test", 1);
        Assert.AreEqual(1, rank);
        var userScore = db.GetLeaderboardUserScores(1, 1);
        Assert.AreEqual("test", userScore.Item1);
        Assert.AreEqual(9999, userScore.Item2);
        db.AddToLeaderboard("test2", 1, 2, 1234);
        rank = db.GetLeaderboardRank("test2", 1);
        Assert.AreEqual(2, rank);
        userScore = db.GetLeaderboardUserScores(1, 2);
        Assert.AreEqual("test2", userScore.Item1);
        Assert.AreEqual(1234, userScore.Item2);
        db.ClearLeaderboard();
        rank = db.GetLeaderboardRank("test", 1);
        Assert.AreEqual(-1, rank);
    }
}
