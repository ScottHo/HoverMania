using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class SqliteDatabase : IDatabaseRepository
{
    IDbConnection dbcon;

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
            "CREATE TABLE IF NOT EXISTS level_table(" +
            "level_id INT, " +
            "time_centiseconds INT, " +
            "locked INT)";
        RunNonQuery(q_createTable);
        q_createTable =
            "CREATE TABLE IF NOT EXISTS leaderboard_table(" +
            "username TEXT, " +
            "level_id INT, " +
            "rank INT, " + 
            "time_centiseconds INT)";
        RunNonQuery(q_createTable);
        q_createTable =
            "CREATE TABLE IF NOT EXISTS users_table(" +
            "user_id INT, " +
            "username TEXT)";
        RunNonQuery(q_createTable);
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

    public void SetUser(int user_id, string username)
    {
        string query = "DELETE FROM users_table";
        RunNonQuery(query);
        query = "INSERT INTO users_table (user_id, username) " +
            "VALUES (" + user_id + ", '" + username + "')";
        RunNonQuery(query);
    }

    public string GetUsername()
    {
        string query = "SELECT * FROM users_table";
        IDataReader reader = RunQuery(query);
        if (reader.Read())
        {
            return Convert.ToString(reader[1]);
        }
        return "";
    }

    public int GetUserID()
    {
        string query = "SELECT * FROM users_table";
        IDataReader reader = RunQuery(query);
        if (reader.Read())
        {
            return Convert.ToInt32(reader[0]);
        }
        return -1;
    }


    public void SetLevelTime(int levelID, int timeCentiseconds)
    {
        string query = "SELECT * FROM level_table WHERE (level_id = " + levelID + ")";
        IDataReader reader = RunQuery(query);
        if (reader.Read())
        {
            if (Convert.ToInt32(reader[1]) < timeCentiseconds)
                return;
            query = "UPDATE level_table " +
                "SET time_centiseconds = " + timeCentiseconds + " " +
                "WHERE(level_id = " + levelID + ")";
            RunNonQuery(query);
        }
        else
        {
            query = "INSERT INTO level_table (level_id, time_centiseconds, locked) " +
            "VALUES ("+ levelID + ", " + timeCentiseconds + ", " + 0 + ")";
            RunNonQuery(query);
        }
    }

    public int GetLevelTime(int levelID)
    {
        string query = "SELECT * FROM level_table WHERE(level_id = " + levelID + ")";
        IDataReader reader = RunQuery(query);
        if (reader.Read())
        {
            var score = Convert.ToInt32(reader[1]);
            if (score < 10)
            {
                score = -1;
            }
            return score;
        }
        return -1;
    }

    public void SetLevelLocked(int levelID, bool locked)
    {
        string query = "SELECT * FROM level_table WHERE (level_id = " + levelID + ")";
        IDataReader reader = RunQuery(query);
        if (reader.Read())
        {
            query = "UPDATE level_table " +
                "SET locked = " + (locked ? 1 : 0) + " " +
                "WHERE(level_id = " + levelID + ")";
            RunNonQuery(query);
        }
        else
        {
            query = "INSERT INTO level_table (level_id, time_centiseconds, locked) " +
            "VALUES (" + levelID + ", "  + -1 + ", " + (locked ? 1 : 0) + ")";
            RunNonQuery(query);
        }

    }
    public bool GetLevelLocked(int levelID)
    {
        string query = "SELECT * FROM level_table WHERE(level_id = " + levelID + ")";
        IDataReader reader = RunQuery(query);
        if (reader.Read())
        {
            return Convert.ToBoolean(reader[2]);
        }
        return true;
    }
    
    public void ClearLeaderboard()
    {
        string query = "DELETE FROM leaderboard_table;";
        RunNonQuery(query);
    }

    public void AddToLeaderboard(string username, int levelID, int rank, int timeCentiseconds)
    {
        string query = "INSERT INTO leaderboard_table (username, level_id, rank, time_centiseconds) " +
            "VALUES ('" + username + "', " + levelID + ", " + rank + ", " + timeCentiseconds + ")";
        RunNonQuery(query);
    }

    public int GetLeaderboardRank(string username, int levelID)
    {
        string query = "SELECT rank FROM leaderboard_table WHERE(level_id = "
                + levelID + " AND username = '" + username + "')";
        IDataReader reader = RunQuery(query);
        if (reader.Read())
        {
            return Convert.ToInt32(reader[0]);
        }
        return -1;
    }

    public Tuple<string, int> GetLeaderboardUserScores(int levelID, int rank)
    {
        string query = "SELECT username, time_centiseconds FROM leaderboard_table WHERE(level_id = "
                + levelID + " AND rank = " + rank + ")";
        IDataReader reader = RunQuery(query);
        if (reader.Read())
        {
            var username = Convert.ToString(reader[0]);
            var score = Convert.ToInt32(reader[1]);
            if (score < 10)
            {
                score = -1;
            }
            return new Tuple<string, int>(username, score);
        }
        return new Tuple<string, int>("", -1);
    }
}
