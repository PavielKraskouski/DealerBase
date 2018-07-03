using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DealerBase.Entities
{
    [Serializable]
    public class Dealer
    {
        public long Id { get; set; }
        public long BusinessEntityId { get; set; }
        public string Name { get; set; }
        public long ActivityId { get; set; }
        public long ActivityDirectionId { get; set; }
        public long Rating { get; set; }
        public bool IsRelevant { get; set; }
        public long RegionId { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Block { get; set; }
        public string Room { get; set; }
        public List<Contact> Contacts { get; set; }
        public string Note { get; set; }
        public string Conditions { get; set; }
        public List<Event> Events { get; set; }
        public DateTime DateAdded { get; set; }

        public Dealer()
        {
            Name = string.Empty;
            Rating = 5;
            IsRelevant = true;
            City = string.Empty;
            Street = string.Empty;
            House = string.Empty;
            Block = string.Empty;
            Room = string.Empty;
            Contacts = new List<Contact>();
            Note = string.Empty;
            Conditions = string.Empty;
            Events = new List<Event>();
        }

        public static EnumerableRowCollection<DataRow> Select(Filter filter)
        {
            return DBAccess.ExecuteReader(String.Format("SELECT * FROM Dealer {0}", filter.Query), filter.Parameters);
        }

        public static DataRow Select(long id)
        {
            return DBAccess.ExecuteReader("SELECT * FROM Dealer WHERE Id = @param1", id).First();
        }

        public static bool Exists(long id)
        {
            return DBAccess.ExecuteScalar<long>("SELECT COUNT(*) FROM Dealer WHERE Id = @param1", id) != 0;
        }

        public static bool Exists(string name)
        {
            return DBAccess.ExecuteScalar<long>("SELECT COUNT(*) FROM Dealer WHERE UPPER(Name) = @param1", name.ToUpper()) != 0;
        }

        public static long Count(Filter filter)
        {
            return DBAccess.ExecuteScalar<long>(String.Format("SELECT COUNT(*) FROM Dealer {0}", filter.Query), filter.Parameters);
        }

        public static long Count(string constantName, long constantId)
        {
            return DBAccess.ExecuteScalar<long>(String.Format("SELECT COUNT(*) FROM Dealer WHERE {0}Id = @param1", constantName), constantId);
        }

        public long Insert()
        {
            DBAccess.ExecuteNonQuery("INSERT INTO Dealer (BusinessEntityId, Name, ActivityId, ActivityDirectionId, Rating, IsRelevant, RegionId, City, Street, House, Block, Room, Note, Conditions) VALUES (@param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9, @param10, @param11, @param12, @param13, @param14)", BusinessEntityId, Name, ActivityId, ActivityDirectionId, Rating, IsRelevant ? 1 : 0, RegionId, City, Street, House, Block, Room, Note, Conditions);
            long dealerId = DBAccess.ExecuteScalar<long>("SELECT MAX(Id) FROM Dealer");
            Contacts.ForEach(x => x.Insert(dealerId));
            Events.ForEach(x => x.Insert(dealerId));
            return dealerId;
        }

        public void Update()
        {
            DBAccess.ExecuteNonQuery("UPDATE Dealer SET BusinessEntityId = @param1, Name = @param2, ActivityId = @param3, ActivityDirectionId = @param4, Rating = @param5, IsRelevant = @param6, RegionId = @param7, City = @param8, Street = @param9, House = @param10, Block = @param11, Room = @param12, Note = @param13, Conditions = @param14 WHERE Id = @param15", BusinessEntityId, Name, ActivityId, ActivityDirectionId, Rating, IsRelevant ? 1 : 0, RegionId, City, Street, House, Block, Room, Note, Conditions, Id);
            Contact.Delete(Id);
            Contacts.ForEach(x => x.Insert(Id));
            Event.Delete(Id);
            Events.ForEach(x => x.Insert(Id));
        }

        public static void Delete(long id)
        {
            DBAccess.ExecuteNonQuery("DELETE FROM Dealer WHERE Id = @param1", id);
        }

        public static Dealer FromDataRow(DataRow dataRow)
        {
            Dealer dealer = new Dealer()
            {
                Id = dataRow.Field<long>("Id"),
                BusinessEntityId = dataRow.Field<long>("BusinessEntityId"),
                Name = dataRow.Field<string>("Name"),
                ActivityId = dataRow.Field<long>("ActivityId"),
                ActivityDirectionId = dataRow.Field<long>("ActivityDirectionId"),
                Rating = dataRow.Field<long>("Rating"),
                IsRelevant = dataRow.Field<long>("IsRelevant") == 1,
                RegionId = dataRow.Field<long>("RegionId"),
                City = dataRow.Field<string>("City"),
                Street = dataRow.Field<string>("Street"),
                House = dataRow.Field<string>("House"),
                Block = dataRow.Field<string>("Block"),
                Room = dataRow.Field<string>("Room"),
                Note = dataRow.Field<string>("Note"),
                Conditions = dataRow.Field<string>("Conditions"),
                DateAdded = DateTime.Parse(dataRow.Field<string>("DateAdded"))
            };
            Contact.Select(dealer.Id).ForEach(x => dealer.Contacts.Add(Contact.FromDataRow(x)));
            Event.Select(dealer.Id).ForEach(x => dealer.Events.Add(Event.FromDataRow(x)));
            return dealer;
        }

        public static TextBlock ToTextBlock(DataRow dataRow)
        {
            TextBlock textBlock = new TextBlock() { Tag = dataRow.Field<long>("Id") };
            textBlock.Inlines.Add(new Run(String.Format("{0} \"{1}\"", BusinessEntity.Select(dataRow.Field<long>("BusinessEntityId")).Field<string>("Name"), dataRow.Field<string>("Name")))
            {
                FontSize = 20,
                FontWeight = FontWeights.Bold
            });
            textBlock.Inlines.Add(new Run("\nРегион: ") { Foreground = Brushes.Silver });
            textBlock.Inlines.Add(new Run(Region.Select(dataRow.Field<long>("RegionId")).Field<string>("Name")));
            textBlock.Inlines.Add(new Run("\nДеятельность: ") { Foreground = Brushes.Silver });
            textBlock.Inlines.Add(new Run(Activity.Select(dataRow.Field<long>("ActivityId")).Field<string>("Name")));
            textBlock.Inlines.Add(new Run("\nНаправление деятельности: ") { Foreground = Brushes.Silver });
            textBlock.Inlines.Add(new Run(ActivityDirection.Select(dataRow.Field<long>("ActivityDirectionId")).Field<string>("Name")));
            textBlock.Inlines.Add(new Run("\nРейтинг: ") { Foreground = Brushes.Silver });
            textBlock.Inlines.Add(new Run(new string('★', (int)dataRow.Field<long>("Rating"))));
            textBlock.Inlines.Add(new Run("\nДата добавления: ") { Foreground = Brushes.Silver });
            textBlock.Inlines.Add(new Run(DateTime.Parse(dataRow.Field<string>("DateAdded")).ToString("dd.MM.yyyy в HH:mm:ss")));
            return textBlock;
        }
    }
}