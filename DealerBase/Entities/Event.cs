using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DealerBase.Entities
{
    [Serializable]
    public class Event
    {
        public long Id { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }

        public Event()
        {
            Header = string.Empty;
            Description = string.Empty;
        }

        public static EnumerableRowCollection<DataRow> Select(long dealerId)
        {
            return DBAccess.ExecuteReader("SELECT * FROM Event WHERE DealerId = @param1", dealerId);
        }

        public static DataRow SelectOne(long id)
        {
            return DBAccess.ExecuteReader("SELECT * FROM Event WHERE Id = @param1", id).First();
        }

        public static EnumerableRowCollection<DataRow> Select(DateTime dateTime)
        {
            return DBAccess.ExecuteReader("SELECT Event.Id, BusinessEntity.Name, Dealer.Name, Header FROM Event JOIN Dealer ON Dealer.Id = Event.DealerId JOIN BusinessEntity ON BusinessEntity.Id = Dealer.BusinessEntityId WHERE DATE(Event.DateAdded) = DATE(@param1) ORDER BY UPPER(Dealer.Name) ASC, UPPER(Header) ASC", dateTime);
        }

        public static bool Exists(long id)
        {
            return DBAccess.ExecuteScalar<long>("SELECT COUNT(*) FROM Event WHERE Id = @param1", id) != 0;
        }

        public static long Count(DateTime dateTime)
        {
            return DBAccess.ExecuteScalar<long>("SELECT COUNT(*) FROM Event WHERE DATE(DateAdded) = DATE(@param1)", dateTime);
        }

        public void Insert(long dealerId)
        {
            DBAccess.ExecuteNonQuery("INSERT INTO Event (Header, Description, DateAdded, DealerId) VALUES (@param1, @param2, @param3, @param4)", Header, Description, DateAdded, dealerId);
        }

        public void Update()
        {
            DBAccess.ExecuteNonQuery("UPDATE Event SET Header = @param1, Description = @param2, DateAdded = @param3 WHERE Id = @param4", Header, Description, DateAdded, Id);
        }

        public static void Delete(long dealerId)
        {
            DBAccess.ExecuteNonQuery("DELETE FROM Event WHERE DealerId = @param1", dealerId);
        }

        public static void DeleteOne(long id)
        {
            DBAccess.ExecuteNonQuery("DELETE FROM Event WHERE Id = @param1", id);
        }

        public static Event FromDataRow(DataRow dataRow)
        {
            return new Event()
            {
                Id = dataRow.Field<long>("Id"),
                Header = dataRow.Field<string>("Header"),
                Description = dataRow.Field<string>("Description"),
                DateAdded = DateTime.Parse(dataRow.Field<string>("DateAdded"))
            };
        }

        public TextBlock ToTextBlock()
        {
            TextBlock textBlock = new TextBlock() { Tag = Id };
            textBlock.Inlines.Add(new Run(Header)
            {
                FontSize = 20,
                FontWeight = FontWeights.Bold
            });
            textBlock.Inlines.Add(new Run("\nДата: ") { Foreground = Brushes.Silver });
            textBlock.Inlines.Add(new Run(DateAdded.ToString("d MMMM yyyy г.")));
            return textBlock;
        }
    }
}