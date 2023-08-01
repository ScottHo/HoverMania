using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class SqliteDatabase : IDatabaseRepository
{
    IDbConnection dbcon;
    public int userID;

    public static void CreateDatabase(string path)
    {
        SqliteConnection.CreateFile(path);
    }
    public SqliteDatabase(string connection)
    {
        dbcon = new SqliteConnection(connection);
        dbcon.Open();
        SetupDatabase();
    }

    ~SqliteDatabase()
    {
        dbcon.Close();
    }

    void SetupDatabase()
    {
        string q_createTable =
            "CREATE TABLE IF NOT EXISTS user_table(" +
            "user_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "money INT)";
        RunNonQuery(q_createTable);

        q_createTable =
            "CREATE TABLE IF NOT EXISTS scores_table(" +
            "user_id INT, " +
            "level_id INT, " +
            "time_centiseconds INT, " +
            "FOREIGN KEY (user_id) REFERENCES user_table(user_id) )";
        RunNonQuery(q_createTable);
    }


    public int CreateUser()
    {
        string query = "INSERT INTO user_table (money) " +
            "VALUES (0)";
        RunNonQuery(query);
        query = "SELECT MAX (user_id) " +
            "FROM user_table";
        IDataReader reader = RunQuery(query);
        reader.Read();
        this.userID = Convert.ToInt32(reader[0]);
        return this.userID;
    }

    public void SwitchUser(int user_id)
    {
        string query = "SELECT * FROM user_table WHERE user_id = " + user_id;
        IDataReader reader = RunQuery(query);
        if (reader.Read())
        {
            if (reader.FieldCount > 0)
            {
                this.userID = user_id;
                return;
            }
        }
        throw new SqliteException("User Id " + user_id + " does not exist, could not switch user");
    }

    IDataReader RunQuery(string query)
    {
        Debug.Log("Running query: \n" + query);
        IDbCommand dbcmd;
        IDataReader reader;

        dbcmd = dbcon.CreateCommand();

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        return reader;
    }

    void RunNonQuery(string query)
    {
        Debug.Log("Running query: \n" + query);
        IDbCommand dbcmd;
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = query;
        dbcmd.ExecuteNonQuery();
    }

    public void SetMoney(int money)
    {
        string query = "UPDATE user_table " +
                "SET money = " + money + " " +
                "WHERE user_id = " + userID + "";
        RunNonQuery(query);
    }
    public int Money()
    {
        string query = "SELECT * FROM user_table WHERE user_id = " + userID;
        IDataReader reader = RunQuery(query);
        int ret = 0;
        if (reader.Read())
        {
            ret = Convert.ToInt32(reader[1]);
        }
        else
        {
            throw new SqliteException("Nothing to read when trying to access money for user_id" + userID);
        }
        return ret;
    }

    public void SetLevelTime(int levelID, int timeCentiseconds)
    {
        string query = "SELECT * FROM scores_table WHERE (user_id = "
            + userID + " AND level_id = " + levelID + ")";
        IDataReader reader = RunQuery(query);
        if (reader.Read())
        {
            query = "UPDATE scores_table " +
                "SET time_centiseconds = " + timeCentiseconds + " " +
                "WHERE(user_id = "
            + userID + " AND level_id = " + levelID + ")";
            RunNonQuery(query);
        }
        else
        {
            query = "INSERT INTO scores_table (user_id, level_id, time_centiseconds) " +
            "VALUES ("+ userID + ", " + levelID + ", " + timeCentiseconds + ")";
            RunNonQuery(query);
        }
    }

    public int GetLevelTime(int levelID)
    {
        string query = "SELECT * FROM scores_table WHERE(user_id = "
            + userID + " AND level_id = " + levelID + ")";
        IDataReader reader = RunQuery(query);
        if (reader.Read())
        {
            return Convert.ToInt32(reader[2]);
        }
        return -1;
    }
}
