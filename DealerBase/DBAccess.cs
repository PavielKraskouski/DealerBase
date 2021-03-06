﻿using DealerBase.Windows;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace DealerBase
{
    [SQLiteFunction(Name = "lower", Arguments = 1, FuncType = FunctionType.Scalar)]
    public class LowerFunction : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            return (args[0] as string).ToLower();
        }
    }

    [SQLiteFunction(Name = "upper", Arguments = 1, FuncType = FunctionType.Scalar)]
    public class UpperFunction : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            return (args[0] as string).ToUpper();
        }
    }

    public static class DBAccess
    {
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string DBPath = Path.Combine(BaseDirectory, "DealerBase.db");
        private static readonly string DBBackupPath = Path.Combine(BaseDirectory, "DealerBase_Backup.db");
        private static readonly string DBConnectionString = $"Data Source = {DBPath}";
        private static readonly string DBBackupConnectionString = $"Data Source = {DBBackupPath}";

        public static void CreateDatabase()
        {
            if (!File.Exists(DBPath))
            {
                if (!File.Exists(DBBackupPath))
                {
                    SQLiteConnection.CreateFile(DBPath);
                    ExecuteNonQuery(Properties.Resources.DealerBase);
                }
                else
                {
                    RestoreDatabase();
                }
            }
        }

        private static void CloneDatabase(string sourceConnectionString, string destinationConnectionString)
        {
            using (SQLiteConnection source = new SQLiteConnection(sourceConnectionString))
            using (SQLiteConnection destination = new SQLiteConnection(destinationConnectionString))
            {
                source.Open();
                destination.Open();
                source.BackupDatabase(destination, "main", "main", -1, null, -1);
            }
        }

        public static void BackupDatabase()
        {
            CloneDatabase(DBConnectionString, DBBackupConnectionString);
        }

        public static void BackupDatabase(string fileName)
        {
            CloneDatabase(DBConnectionString, $"Data Source = {fileName}");
        }

        private static void RestoreDatabase()
        {
            CloneDatabase(DBBackupConnectionString, DBConnectionString);
        }

        private static void Execute(Action<SQLiteCommand> action, string commandText, params object[] parameters)
        {
            if (File.Exists(DBPath))
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(DBConnectionString))
                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    using (SQLiteCommand pragmaCommand = new SQLiteCommand("PRAGMA foreign_keys = 1", connection))
                    {
                        connection.Open();
                        pragmaCommand.ExecuteNonQuery();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            command.Parameters.Add(new SQLiteParameter($"@param{i + 1}", parameters[i]));
                        }
                        action(command);
                    }
                }
                catch (SQLiteException)
                {
                    new ErrorWindow(7).ShowDialog(MainWindow.ActiveWindow);
                    Environment.Exit(7);
                }
            }
            else
            {
                new ErrorWindow(7).ShowDialog(MainWindow.ActiveWindow);
                Environment.Exit(7);
            }
        }

        public static EnumerableRowCollection<DataRow> ExecuteReader(string commandText, params object[] parameters)
        {
            using (DataTable dataTable = new DataTable())
            {
                Execute(x =>
                {
                    using (SQLiteDataReader reader = x.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }, commandText, parameters);
                return dataTable.AsEnumerable();
            }
        }

        public static void ExecuteNonQuery(string commandText, params object[] parameters)
        {
            Execute(x => x.ExecuteNonQuery(), commandText, parameters);
        }

        public static T ExecuteScalar<T>(string commandText, params object[] parameters)
        {
            T result = default(T);
            Execute(x => result = (T)x.ExecuteScalar(), commandText, parameters);
            return result;
        }
    }
}