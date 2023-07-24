using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Mono.Data.Sqlite;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class SqliteDatabase : IDatabaseRepository
{
    IDbConnection dbcon;
    public int user_id;
    public SqliteDatabase(string connection)
    {
        //string connection = "URI=file:" + Application.persistentDataPath + "/" + "My_Database";
        dbcon = new SqliteConnection(connection);
        dbcon.Open();
        setupDatabase();
    }

    ~SqliteDatabase()
    {
        dbcon.Close();
    }

    void setupDatabase()
    {
        string q_createTable =
            "CREATE TABLE IF NOT EXISTS user_table(" +
            "user_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "money INT)";
        runNonQuery(q_createTable);

        q_createTable =
            "CREATE TABLE IF NOT EXISTS sample_table(" +
            "sample_id INT, " +
            "user_id INT, " +
            "amount INT, " +
            "FOREIGN KEY (user_id) REFERENCES user_table(user_id) )";
        runNonQuery(q_createTable);
    }


    public void createUser()
    {
        string query = "INSERT INTO user_table (money) " +
            "VALUES (0)";
        runNonQuery(query);
        query = "SELECT MAX (user_id) " +
            "FROM user_table";
        IDataReader reader = runQuery(query);
        reader.Read();
        this.user_id = Convert.ToInt32(reader[0]);
    }

    public void switchUser(int user_id)
    {
        string query = "SELECT * FROM user_table WHERE user_id = " + user_id;
        IDataReader reader = runQuery(query);
        if (reader.Read())
        {
            if (reader.FieldCount > 0)
            {
                this.user_id = user_id;
                return;
            }
        }
        throw new SqliteException("User Id " + user_id + " does not exist, could not switch user");
    }

    IDataReader runQuery(string query)
    {
        Debug.Log("Running query: \n" + query);
        IDbCommand dbcmd;
        IDataReader reader;

        dbcmd = dbcon.CreateCommand();

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        return reader;
    }

    void runNonQuery(string query)
    {
        Debug.Log("Running query: \n" + query);
        IDbCommand dbcmd;
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = query;
        dbcmd.ExecuteNonQuery();
    }

    public void setMoney(int money)
    {
        string query = "UPDATE user_table " +
                "SET money = " + money + " " +
                "WHERE user_id = " + user_id + "";
        runNonQuery(query);
    }
    public int money()
    {
        string query = "SELECT * FROM user_table WHERE user_id = " + user_id;
        IDataReader reader = runQuery(query);
        int ret = 0;
        if (reader.Read())
        {
            ret = Convert.ToInt32(reader[1]);
        }
        else
        {
            throw new SqliteException("Nothing to read when trying to access money for user_id" + user_id);
        }
        return ret;
    }

    public void addSample(Sample sample)
    {
        string query = "SELECT * FROM sample_table WHERE (user_id = "
            + user_id + " AND sample_id = " + sample.id + ")";
        IDataReader reader = runQuery(query);
        if (reader.Read())
        {
            query = "UPDATE sample_table " +
                "SET amount = amount + " + sample.quantity + " " +
                "WHERE(user_id = "
            + user_id + " AND sample_id = " + sample.id + ")";
            runNonQuery(query);
        }
        else
        {
            query = "INSERT INTO sample_table (sample_id, user_id, amount) " +
            "VALUES ("+ sample.id + ", " + user_id + ", 1)";
            runNonQuery(query);
        }
    }

    public List<Sample> samples()
    {
        string query = "SELECT * FROM sample_table WHERE user_id = " + user_id;
        IDataReader reader = runQuery(query);
        List<Sample> samples = new List<Sample>();
        int current = 0;
        while (reader.Read())
        {
            int sample_id = Convert.ToInt32(reader[0]);
            int quantity = Convert.ToInt32(reader[2]);
            samples.Add(SampleFactory.createSample(sample_id, quantity));
        }
        return samples;
    }
}
