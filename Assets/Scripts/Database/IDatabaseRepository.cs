
interface IDatabaseRepository
{
    void setMoney(int money);
    int money();

    void addSample(Sample sample);
    Sample[] samples();
}