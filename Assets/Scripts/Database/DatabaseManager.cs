using UnityEngine;

public sealed class DatabaseManager
{
    public DummyDatabase database;
    DatabaseManager() {}
    private static readonly object l = new object ();
    private static DatabaseManager instance = null;
    public static DatabaseManager Instance
    {
        get
        {
            lock (l)
                {
                    if (instance == null)
                    {
                        instance = new DatabaseManager();
                        instance.Create();
                    }
                    return instance;
                }
        }
    }
    void Create()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            database = new DummyDatabase("");
            return;
        }
        string databasePath = Application.persistentDataPath + "/data.bin";
        database = new DummyDatabase(databasePath);
    }
}
