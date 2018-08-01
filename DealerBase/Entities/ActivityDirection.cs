using System;
using System.Data;
using System.Linq;
using System.Windows.Controls;

namespace DealerBase.Entities
{
    [Serializable]
    public class ActivityDirection
    {
        public static EnumerableRowCollection<DataRow> Select()
        {
            return DBAccess.ExecuteReader("SELECT * FROM ActivityDirection ORDER BY UPPER(Name) ASC");
        }

        public static DataRow Select(long id)
        {
            return DBAccess.ExecuteReader("SELECT * FROM ActivityDirection WHERE Id = @param1", id).First();
        }

        public static bool Exists(long id)
        {
            return DBAccess.ExecuteScalar<long>("SELECT COUNT(*) FROM ActivityDirection WHERE Id = @param1", id) != 0;
        }

        public static bool Exists(string name, long id)
        {
            return DBAccess.ExecuteScalar<long>("SELECT COUNT(*) FROM ActivityDirection WHERE UPPER(Name) = @param1 AND Id != @param2", name.ToUpper(), id) != 0;
        }

        public static long Count()
        {
            return DBAccess.ExecuteScalar<long>("SELECT COUNT(*) FROM ActivityDirection");
        }

        public static long Insert(string name)
        {
            DBAccess.ExecuteNonQuery("INSERT INTO ActivityDirection (Name) VALUES (@param1)", name);
            return DBAccess.ExecuteScalar<long>("SELECT MAX(Id) FROM ActivityDirection");
        }

        public static void Update(long id, string name)
        {
            DBAccess.ExecuteNonQuery("UPDATE ActivityDirection SET Name = @param1 WHERE Id = @param2", name, id);
        }

        public static void Delete(long id)
        {
            DBAccess.ExecuteNonQuery("DELETE FROM ActivityDirection WHERE Id = @param1", id);
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