
public readonly struct LevelInfo
{
    public LevelInfo(int id, string levelName, string spriteName)
    {
        this.id = id;
        this.levelName = levelName;
        this.spriteName = spriteName;
    }
    public readonly int id;
    public readonly string levelName;
    public readonly string spriteName;
}
public class LevelFactory
{
    public static LevelInfo GetLevelInfo(int id)
    {
        if (id == 1)
        {
            return new LevelInfo(1, "Demo Level 1", "Level1Thumbnail");
        }
        if (id == 2)
        {
            return new LevelInfo(2, "Demo Level 2", "Level2Thumbnail");
        }
        if (id == 3)
        {
            return new LevelInfo(3, "Demo Level 3", "Level3Thumbnail");
        }
        else
        {
            return new LevelInfo(-1, "???", "UnknownThumbnail");
        }
    }

    public static int NumLevels()
    {
        return 3;
    }
}


