public readonly struct LevelInfo
{
    public LevelInfo(int id, string levelName, string spriteName, int difficulty)
    {
        this.id = id;
        this.levelName = levelName;
        this.spriteName = spriteName;
        this.difficulty = difficulty;
    }
    public readonly int id;
    public readonly string levelName;
    public readonly string spriteName;
    public readonly int difficulty;
}
public class LevelFactory
{
    public static LevelInfo GetLevelInfo(int id)
    {
        if (id == 1)
        {
            return new LevelInfo(1, "Demo Level 1", "CheckMark", 1);
        }
        if (id == 2)
        {
            return new LevelInfo(2, "Demo Level 2", "CheckMark", 2);
        }
        if (id == 3)
        {
            return new LevelInfo(3, "Demo Level 3", "CheckMark", 3);
        }
        else
        {
            return new LevelInfo(-1, "Dev Level", "CheckMark", 4);
        }
    }
}


