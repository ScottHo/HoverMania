
using System.Collections.Generic;

interface IDatabaseRepository
{
    void setMoney(int money);
    int money();

    void addSample(Sample sample);
    List<Sample> samples();

    void createUser();

    void switchUser(int userId);
}

