using NUnit.Framework;
using System.Diagnostics;

public class RequestsHandlerTests
{
    RequestHandler handler;
    [SetUp]
    public void SetUp()
    {
        handler = new RequestHandler();
    }
    
    [Test]
    public async void TestScores()
    {
        var v2 = await handler.GetHiScores(3);
        Assert.AreEqual(3, v2.Count);
    }

    [Test]
    public async void TestAddScores()
    {
        var v2 = await handler.AddHiScore(1, 1, 999);
        Assert.True(v2);
    }

    [Test]
    public async void TestUserRank()
    {
        var v = await handler.GetUserRank(1, 3);
        Assert.AreEqual(3, v.Count);
    }

    [Test]
    public async void TestNewUser()
    {
        var v = await handler.NewUser("test23");
        Assert.AreNotEqual(-1, v);
    }

    [Test]
    public async void TestChangeUser()
    {
        var v = await handler.ChangeUsername(3, "asdf2g");
        Assert.AreEqual(UserCreatedStatus.Success, v);
        v = await handler.ChangeUsername(9999, "asdfg123");
        Assert.AreEqual(UserCreatedStatus.InvalidID, v);
    }

}
