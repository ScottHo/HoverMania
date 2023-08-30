
using System.Collections.Generic;

public interface IDatabaseRepository
{
    int CreateUser();
    void SwitchUser(int userID);
    void SetMoney(int money);
    int Money();

    void SetLevelTime(int levelID, int timeCentiseconds);

    int GetLevelTime(int levelID);

    void SetLevelLocked(int levelID, bool locked);

    bool GetLevelLocked(int levelID);
}

