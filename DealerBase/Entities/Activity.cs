using System;
using System.Data;
using System.Linq;
using System.Windows.Controls;

namespace DealerBase.Entities
{
    [Serializable]
    public class Activity
    {
        public static EnumerableRowCollection<DataRow> Select()
        {
            return DBAccess.ExecuteReader("SELECT * FROM Activity ORDER BY UPPER(Name) ASC");
        }

        public static DataRow Select(long id)
        {
            return DBAccess.ExecuteReader("SELECT * FROM Activity WHERE Id = @param1", id).First();
        }

        public static bool Exists(long id)
        {
            return DBAccess.ExecuteScalar<long>("SELECT COUNT(*) FROM Activity WHERE Id = @param1", id) != 0;
        }

        public static bool Exists(string name)
        {
            return DBAccess.ExecuteScalar<long>("SELECT COUNT(*) FROM Activity WHERE UPPER(Name) = @param1", name.ToUpper()) != 0;
        }

        public static long Count()
        {
            return DBAccess.ExecuteScalar<long>("SELECT COUNT(*) FROM Activity");
        }

        public static long Insert(string name)
        {
            DBAccess.ExecuteNonQuery("INSERT INTO Activity (Name) VALUES (@param1)", name);
            return DBAccess.ExecuteScalar<long>("SELECT MAX(Id) FROM Activity");
        }

        public static void Update(long id, string name)
        {
            DBAccess.ExecuteNonQuery("UPDATE Activity SET Name = @param1 WHERE Id = @param2", name, id);
        }

        public static void Delete(long id)
        {
            DBAccess.ExecuteNonQuery("DELETE FROM Activity WHERE Id = @param1", id);
        }

        public static TextBlock ToTextBlock(DataRow dataRow)
        {
            return new TextBlock()
            {
                Text = dataRow.Field<string>("Name"),
                Tag = dataRow.Field<long>("Id")
            };
        }
    }
}