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
        private static readonly string DBConnectionString = String.Format("Data Source = {0}", DBPath);
        private static readonly string DBBackupConnectionString = String.Format("Data Source = {0}", DBBackupPath);

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
            CloneDatabase(DBConnectionString, String.Format("Data Source = {0}", fileName));
        }

        private static void RestoreDatabase()
        {
            CloneDatabase(DBBackupConnectionString, DBConnectionString);
        }

        private static void Execute(Action<SQLiteCommand> action, string commandText, params object[] parameters)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnectionString))
            using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
            using (SQLiteCommand pragmaCommand = new SQLiteCommand("PRAGMA foreign_keys = 1", connection))
            {
                connection.Open();
                pragmaCommand.ExecuteNonQuery();
                for (int i = 0; i < parameters.Length; i++)
                {
                    command.Parameters.Add(new SQLiteParameter(String.Format("@param{0}", i + 1), parameters[i]));
                }
                action(command);
            }
        }

        public static EnumerableRowCollection<DataRow> ExecuteReader(string commandText, params object[] parameters)
        {
            DataTable dataTable = new DataTable();
            Execute(x => dataTable.Load(x.ExecuteReader()), commandText, parameters);
            return dataTable.AsEnumerable();
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