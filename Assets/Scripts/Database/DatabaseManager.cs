
using Mono.Data.Sqlite;
using UnityEngine;

public sealed class DatabaseManager
{
    public IDatabaseRepository database;
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
        }
        else
        {
            string databasePath = Application.persistentDataPath + "/main_db.sqlite";
            if (!System.IO.File.Exists(databasePath))
            {
                SqliteDatabase.CreateDatabase(databasePath);
            }
            database = new SqliteDatabase("URI=file:" + databasePath);
            try
            {
                database.SwitchUser(1);
            }
            catch (SqliteException)
            {
                database.CreateUser();
            }
        }
    }
}
