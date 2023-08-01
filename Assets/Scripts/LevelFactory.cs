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
            return new LevelInfo(1, "Demo Level 1", "Level1Thumbnail", 1);
        }
        if (id == 2)
        {
            return new LevelInfo(2, "Demo Level 2", "Level2Thumbnail", 2);
        }
        if (id == 3)
        {
            return new LevelInfo(3, "Demo Level 3", "Level3Thumbnail", 3);
        }
        else
        {
            return new LevelInfo(-1, "Dev Level", "UnknownThumbnail", 4);
        }
    }
}


