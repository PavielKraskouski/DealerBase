using System;
using System.Data;
using System.Windows.Controls;

namespace DealerBase.Entities
{
    [Serializable]
    public class Email
    {
        public long Id { get; set; }
        public string Value { get; set; }

        public Email()
        {
            Value = string.Empty;
        }

        public static EnumerableRowCollection<DataRow> Select(Filter filter)
        {
            return DBAccess.ExecuteReader(String.Format("SELECT DISTINCT Value FROM Email JOIN Contact ON Contact.Id = Email.ContactId JOIN Dealer ON Dealer.Id = Contact.DealerId {0} ORDER BY UPPER(Value) ASC", filter.Query), filter.Parameters);
        }

        public static EnumerableRowCollection<DataRow> Select(long contactId)
        {
            return DBAccess.ExecuteReader("SELECT * FROM Email WHERE ContactId = @param1", contactId);
        }

        public static long Count(Filter filter)
        {
            return DBAccess.ExecuteScalar<long>(String.Format("SELECT COUNT(DISTINCT Value) FROM Email JOIN Contact ON Contact.Id = Email.ContactId JOIN Dealer ON Dealer.Id = Contact.DealerId {0}", filter.Query), filter.Parameters);
        }

        public void Insert(long contactId)
        {
            DBAccess.ExecuteNonQuery("INSERT INTO Email (Value, ContactId) VALUES (@param1, @param2)", Value, contactId);
        }

        public static Email FromDataRow(DataRow dataRow)
        {
            return new Email()
            {
                Id = dataRow.Field<long>("Id"),
                Value = dataRow.Field<string>("Value")
            };
        }

        public TextBlock ToTextBlock()
        {
            return new TextBlock()
            {
                Text = Value,
                Tag = Id
            };
        }
    }
}