using NUnit.Framework;
using Mono.Data.Sqlite;
using System.Collections.Generic;


public class SqliteTests
{
    SqliteDatabase sqliteDatabase;
    [SetUp]
    public void SetUp()
    {
        sqliteDatabase = new SqliteDatabase("Data Source=:memory:");
    }
    // A Test behaves as an ordinary method
    [Test]
    public void TestMoney()
    {
        Assert.Throws<SqliteException>(throwGetMoney);
        sqliteDatabase.CreateUser();
        int money = sqliteDatabase.Money();
        Assert.AreEqual(0, money);
        sqliteDatabase.SetMoney(100);
        money = sqliteDatabase.Money();
        Assert.AreEqual(100, money);
    }

    void throwGetMoney()
    {
        sqliteDatabase.Money();
    }

    [Test]
    public void TestCreateUser()
    {
        sqliteDatabase.CreateUser();
        Assert.AreEqual(1, sqliteDatabase.userID);
        sqliteDatabase.CreateUser();
        Assert.AreEqual(2, sqliteDatabase.userID);
        sqliteDatabase.SwitchUser(1);
        Assert.AreEqual(1, sqliteDatabase.userID);
        Assert.Throws<SqliteException>(throwSwitchUser);

    }

    void throwSwitchUser()
    {
        sqliteDatabase.SwitchUser(3);
    }

    [Test]
    public void TestScores()
    {
        sqliteDatabase.CreateUser();
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
}
