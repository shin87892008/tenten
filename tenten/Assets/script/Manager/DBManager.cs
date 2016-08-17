using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class DBManager : Singleton<DBManager>
{
    private bool _IsLoad = false;
    public bool IsLoad
    {
        get
        {
            return _IsLoad;
        }
    }
    public void Init()
    {
        Load_DB();
    }

    private void Load_DB()
    {
        string conn = Static_Value.Strings.DATABASE_PATH;
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();

        string dbquery = "select * from Character";
        dbcmd.CommandText = dbquery;
        IDataReader reader = dbcmd.ExecuteReader();

        while(reader.Read())
        {
            int value = reader.GetInt32(0);
            Debug.LogError(value.ToString());
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;


        _IsLoad = true;
    }
}
