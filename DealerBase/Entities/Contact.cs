using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DealerBase.Entities
{
    [Serializable]
    public class Contact
    {
        public long Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Position { get; set; }
        public List<Phone> Phones { get; set; }
        public List<Fax> Faxes { get; set; }
        public List<Email> Emails { get; set; }

        public Contact()
        {
            Surname = string.Empty;
            Name = string.Empty;
            Patronymic = string.Empty;
            Position = string.Empty;
            Phones = new List<Phone>();
            Faxes = new List<Fax>();
            Emails = new List<Email>();
        }

        public static EnumerableRowCollection<DataRow> Select(long dealerId)
        {
            return DBAccess.ExecuteReader("SELECT * FROM Contact WHERE DealerId = @param1", dealerId);
        }

        public void Insert(long dealerId)
        {
            DBAccess.ExecuteNonQuery("INSERT INTO Contact (Surname, Name, Patronymic, Position, DealerId) VALUES (@param1, @param2, @param3, @param4, @param5)", Surname, Name, Patronymic, Position, dealerId);
            long contactId = DBAccess.ExecuteScalar<long>("SELECT MAX(Id) FROM Contact");
            Phones.ForEach(x => x.Insert(contactId));
            Faxes.ForEach(x => x.Insert(contactId));
            Emails.ForEach(x => x.Insert(contactId));
        }

        public static void Delete(long dealerId)
        {
            DBAccess.ExecuteNonQuery("DELETE FROM Contact WHERE DealerId = @param1", dealerId);
        }

        public static Contact FromDataRow(DataRow dataRow)
        {
            Contact contact = new Contact()
            {
                Id = dataRow.Field<long>("Id"),
                Surname = dataRow.Field<string>("Surname"),
                Name = dataRow.Field<string>("Name"),
                Patronymic = dataRow.Field<string>("Patronymic"),
                Position = dataRow.Field<string>("Position")
            };
            Phone.Select(contact.Id).ForEach(x => contact.Phones.Add(Phone.FromDataRow(x)));
            Fax.Select(contact.Id).ForEach(x => contact.Faxes.Add(Fax.FromDataRow(x)));
            Email.Select(contact.Id).ForEach(x => contact.Emails.Add(Email.FromDataRow(x)));
            return contact;
        }

        public TextBlock ToTextBlock()
        {
            TextBlock textBlock = new TextBlock() { Tag = Id };
            textBlock.Inlines.Add(new Run($"{Surname} {Name} {Patronymic}".Trim())
            {
                FontSize = 20,
                FontWeight = FontWeights.Bold
            });
            textBlock.Inlines.Add(new Run("\nДолжность: ") { Foreground = Brushes.Silver });
            textBlock.Inlines.Add(new Run(Position == string.Empty ? "–" : Position));
            return textBlock;
        }
    }
}