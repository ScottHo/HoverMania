
public readonly struct LevelInfo
{
    public LevelInfo(int id, string spriteName)
    {
        this.id = id;
        this.spriteName = spriteName;
    }
    public readonly int id;
    public readonly string spriteName;
}
public class LevelFactory
{
    public static LevelInfo GetLevelInfo(int id)
    {
        if (id == 1)
        {
            return new LevelInfo(id, "Level1Thumbnail");
        }
        if (id == 2)
        {
            return new LevelInfo(id, "Level2Thumbnail");
        }
        if (id == 3)
        {
            return new LevelInfo(id, "Level3Thumbnail");
        }
        if (id == 4)
        {
            return new LevelInfo(id, "Level1Thumbnail");
        }
        if (id == 5)
        {
            return new LevelInfo(id, "Level1Thumbnail");
        }
        if (id == 6)
        {
            return new LevelInfo(id, "Level1Thumbnail");
        }
        if (id == 7)
        {
            return new LevelInfo(id, "Level1Thumbnail");
        }
        if (id == 8)
        {
            return new LevelInfo(id, "Level1Thumbnail");
        }
        if (id == 9)
        {
            return new LevelInfo(id, "Level1Thumbnail");
        }
        if (id == 10)
        {
            return new LevelInfo(id, "Level1Thumbnail");
        }
        else
        {
            return new LevelInfo(-1, "UnknownThumbnail");
        }
    }

    public static int NumLevels(bool demo)
    {
        if (demo)
        {
            return 3;
        }
        return 10;
    }
}


