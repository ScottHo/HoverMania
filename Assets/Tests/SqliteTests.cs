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

    [Test]
    public void TestSamples()
    {
        sqliteDatabase.createUser();
        List<Sample> samples = sqliteDatabase.samples();
        Assert.AreEqual(0, samples.Count);
        Sample sample = SampleFactory.createSample(0, 1);
        sqliteDatabase.addSample(sample);
        samples = sqliteDatabase.samples();
        Assert.AreEqual(1, samples.Count);
        Assert.AreEqual(1, samples[0].quantity);
    }
}
