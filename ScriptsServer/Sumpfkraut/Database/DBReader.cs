﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using SQLiteDataReader = Mono.Data.Sqlite.SqliteDataReader;
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;

namespace GUC.Scripts.Sumpfkraut.Database
{
    public class DBReader : GUC.Utilities.ExtendedObject
    {

        #region attributes

        new public static readonly string _staticName = "DBReader (static)";

        public static readonly string DataSource = "Data Source=save.db";

        #endregion



        public DBReader ()
        {
            SetObjName("DBReader (default)");
        }
        
        
        
        #region methods for database-interaction

        /**
         *   Executes a defined sql-query and stores the results as strings.
         *   Stores the results for each sql-statement provided, so that after defining 2 statements 
         *   (statement ends with ;) there is a list of 2 results. Each result ist in itself a list of rows.
         *   Per row there can be 0 or more objects/table column entries of data.
         *   @param results stores the results of 1 or multuple provided sql-queries
         *   @param select is the optional string after the SELECT-statement in sql
         *   @param from is the optional string after the FROM-statement in sql
         *   @param where is the optional string after the WHERE-statement in sql
         *   @param orderBy is the optional string after the ORDER BY-statement in sql
         */
        public static void LoadFromDB (ref List<List<List<object>>> results, string dataSource, 
            string select, string from, string where, string orderBy)
        { 
            string completeQuery = string.Format("SELECT {0} FROM {1} WHERE {2} ORDER BY {3};",
                select, from, where, orderBy);

            LoadFromDB(ref results, completeQuery, dataSource);
        }

        public static void LoadFromDB (ref List<List<List<object>>> results,
            string completeQuery, string dataSource)
        {
            using (SqliteConnection con = new SqliteConnection())
            {
                con.ConnectionString = dataSource;
                con.Open();

                // security check and close connection if necessary
                if (!DBSecurity.IsSecureSQLCommand(completeQuery))
                {
                    MakeLogWarningStatic(typeof(DBReader), 
                        "LoadFromDB: Prevented forwarding of insecure sql-command: " 
                        + completeQuery);
                    if (con.State.ToString() == "Open")
                    {
                        con.Close();
                        con.Dispose();
                    }
                    return;
                }

                using (SQLiteCommand cmd = new SQLiteCommand(completeQuery, con))
                {
                    SQLiteDataReader rdr = null;
                    try
                    {
                        rdr = cmd.ExecuteReader();
                        if (rdr == null) { return; }
                        if (!rdr.HasRows) { return; }

                        // temporary array to put all data of a row into
                        object[] rowArr = null;

                        do
                        {
                            // add new result-list
                            results.Add(new List<List<object>>());
                            while (rdr.Read())
                            {
                                // create and fill array of the temporary data row
                                rowArr = new object[rdr.FieldCount];
                                rdr.GetValues(rowArr);
                                results[results.Count - 1].Add(new List<object>(rowArr));
                            }
                        }
                        while (rdr.NextResult());

                    }
                    catch (Exception ex)
                    {
                        throw new Exception("LoadFromDB: Could not execute SQLiteDataReader: " + ex);
                    }
                    finally
                    {
                        if (rdr != null)
                        {
                            rdr.Close();
                            rdr.Dispose();
                        }
                    }
                }

                // close connection if still opened
                if (con.State.ToString() == "Open")
                {
                    con.Close();
                    con.Dispose();
                }
            }
            
        }

        public static int SaveToDB (string dataSource, string completeQuery)
        {
            int changedRows = 0;

            using (SqliteConnection con = new SqliteConnection())
            {
                con.ConnectionString = dataSource;
                con.Open();

                // security check and close connection if necessary
                if (!DBSecurity.IsSecureSQLCommand(completeQuery))
                {
                    MakeLogWarningStatic(typeof(DBReader),
                        "SaveToDB: Prevented forwarding of insecure sql-command: "
                        + completeQuery);
                    if (con.State.ToString() == "Open")
                    {
                        con.Close();
                        con.Dispose();
                    }
                    return changedRows;
                }

                using (SQLiteCommand cmd = new SQLiteCommand(Sqlite.getSqlite().connection))
                {
                    cmd.CommandText = completeQuery;

                    SQLiteDataReader rdr = null;
                    try
                    {
                        changedRows = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Could not execute SQLiteDataReader in SaveToDB: " + ex);
                    }
                    finally
                    {
                        if (rdr != null)
                        {
                            rdr.Close();
                        }
                    }
                }
            }

            return changedRows;
        }

        #endregion



        #region utility methods

        public static int[] ParseParamToIntArray (string param)
        {
            string[] data = param.Split(new char[]{',', '='});
            int resultLength = data.Length / 2;
            int[] result = new int[resultLength];
            if ((data != null) && (data.Length > 0))
            {
                int resultIndex = 0;
                int val = 0;
                int i = 0;
                while (i < data.Length)
                {
                    if ((i + 1) >= data.Length) { break; }

                    try
                    {
                        if (Int32.TryParse(data[i + 1], out val))
                        {
                            result[resultIndex] = val;
                            resultIndex++;
                        }
                        else
                        {
                            throw new Exception("Couldn't convert part of param-string to int in ParseMultiParamToArray.");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Couldn't cast converted part of param-string from int to enum-entry DamageTypeIndex: " + ex);
                    }
                                
                    i += 2;
                }
            }

            return result;
        }

        public static string[] ParseParamToStringArray (string param)
        {
            string[] data = param.Split(new char[]{',', '='});
            int resultLength = data.Length / 2;
            string[] result = new string[resultLength];
            if ((data != null) && (data.Length > 0))
            {
                int resultIndex = 0;
                int i = 0;
                while (i < data.Length)
                {
                    if ((i + 1) >= data.Length) { break; }

                    result[resultIndex] = data[i + 1];
                    resultIndex++;
                                
                    i += 2;
                }
            }

            return result;
        }

        #endregion

    }
}