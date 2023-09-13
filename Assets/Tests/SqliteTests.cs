using NUnit.Framework;

public class SqliteTests
{
    SqliteDatabase sqliteDatabase;
    [SetUp]
    public void SetUp()
    {
        sqliteDatabase = new SqliteDatabase("Data Source=:memory:");
    }

    [Test]
    public void TestScores()
    {
        int levelTime = sqliteDatabase.GetLevelTime(1);
        Assert.AreEqual(levelTime, -1);
        sqliteDatabase.SetLevelTime(1, 999);
        levelTime = sqliteDatabase.GetLevelTime(1);
        Assert.AreEqual(levelTime, 999);
        levelTime = sqliteDatabase.GetLevelTime(2);
        Assert.AreEqual(levelTime, -1);
        sqliteDatabase.SetLevelTime(2, 999);
        levelTime = sqliteDatabase.GetLevelTime(2);
        Assert.AreEqual(levelTime, 999);
    }


    [Test]
    public void TestLocked()
    {
        bool locked = sqliteDatabase.GetLevelLocked(1);
        Assert.True(locked);
        sqliteDatabase.SetLevelLocked(1, false);
        locked = sqliteDatabase.GetLevelLocked(1);
        Assert.False(locked);
        sqliteDatabase.SetLevelLocked(1, true);
        locked = sqliteDatabase.GetLevelLocked(1);
        Assert.True(locked);
    }

    [Test]
    public void TestUsers()
    {
        var user = sqliteDatabase.GetUsername();
        Assert.AreEqual("", user);

        sqliteDatabase.SetUser(3, "Test");

        user = sqliteDatabase.GetUsername();
        Assert.AreEqual("Test", user);

        var userID = sqliteDatabase.GetUserID();
        Assert.AreEqual(3, userID);
    }

    [Test]
    public void TestLeaderboard()
    {
        sqliteDatabase.AddToLeaderboard("test", 1, 1, 9999);
        int rank = sqliteDatabase.GetLeaderboardRank("test", 1);
        Assert.AreEqual(1, rank);
        var userScore = sqliteDatabase.GetLeaderboardUserScores(1, 1);
        Assert.AreEqual("test", userScore.Item1);
        Assert.AreEqual(9999, userScore.Item2);
        sqliteDatabase.AddToLeaderboard("test2", 1, 2, 1234);
        rank = sqliteDatabase.GetLeaderboardRank("test2", 1);
        Assert.AreEqual(2, rank);
        userScore = sqliteDatabase.GetLeaderboardUserScores(1, 2);
        Assert.AreEqual("test2", userScore.Item1);
        Assert.AreEqual(1234, userScore.Item2);
        sqliteDatabase.ClearLeaderboard();
        rank = sqliteDatabase.GetLeaderboardRank("test", 1);
        Assert.AreEqual(-1, rank);
    }
}
