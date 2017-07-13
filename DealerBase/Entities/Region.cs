using System.Data;
using System.Linq;
using System.Windows.Controls;

namespace DealerBase.Entities
{
    public static class Region
    {
        public static EnumerableRowCollection<DataRow> Select()
        {
            return DBAccess.ExecuteReader("SELECT * FROM Region ORDER BY UPPER(Name) ASC");
        }

        public static DataRow Select(long id)
        {
            return DBAccess.ExecuteReader("SELECT * FROM Region WHERE Id = @param1", id).First();
        }

        public static bool Exists(long id)
        {
            return DBAccess.ExecuteScalar<long>("SELECT COUNT(*) FROM Region WHERE Id = @param1", id) != 0;
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