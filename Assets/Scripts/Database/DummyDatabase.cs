using System.Collections.Generic;

public class DummyDatabase : IDatabaseRepository
{
    int _money = 0;
    List<Sample> _sampleList = new List<Sample>();

    public DummyDatabase(string connection)
    {
    }

    ~DummyDatabase()
    {
    }

    public void createUser()
    {
    }

    public void switchUser(int user_id)
    {
    }

    public void setMoney(int money)
    {
        _money = money;
    }
    public int money()
    {
        return _money;
    }

    public void addSample(Sample sample)
    {
        _sampleList.Add(sample);
    }

    public List<Sample> samples()
    {
        return _sampleList;
    }
}
