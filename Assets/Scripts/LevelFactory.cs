
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
        if (id <= NumLevels())
        {
            return new LevelInfo(id, "Thumb" + id);

        }
        else
        {
            return new LevelInfo(-1, "UnknownThumbnail");
        }
    }

    public static int NumLevels()
    {
        return 5;
    }
}


