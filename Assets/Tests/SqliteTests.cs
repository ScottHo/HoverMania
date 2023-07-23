using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Mono.Data.Sqlite;

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
        sqliteDatabase.createUser();
        int money = sqliteDatabase.money();
        Assert.AreEqual(0, money);
        sqliteDatabase.setMoney(100);
        money = sqliteDatabase.money();
        Assert.AreEqual(100, money);
    }

    void throwGetMoney()
    {
        sqliteDatabase.money();
    }

    [Test]
    public void TestCreateUser()
    {
        sqliteDatabase.createUser();
        Assert.AreEqual(1, sqliteDatabase.user_id);
        sqliteDatabase.createUser();
        Assert.AreEqual(2, sqliteDatabase.user_id);
        sqliteDatabase.switchUser(1);
        Assert.AreEqual(1, sqliteDatabase.user_id);
        Assert.Throws<SqliteException>(throwSwitchUser);

    }

    void throwSwitchUser()
    {
        sqliteDatabase.switchUser(3);
    }
}
