﻿using System;
using System.Data;
using System.Windows.Controls;

namespace DealerBase.Entities
{
    [Serializable]
    public class Fax
    {
        public long Id { get; set; }
        public string Value { get; set; }

        public Fax()
        {
            Value = string.Empty;
        }

        public static EnumerableRowCollection<DataRow> Select(long contactId)
        {
            return DBAccess.ExecuteReader("SELECT * FROM Fax WHERE ContactId = @param1", contactId);
        }

        public void Insert(long contactId)
        {
            DBAccess.ExecuteNonQuery("INSERT INTO Fax (Value, ContactId) VALUES (@param1, @param2)", Value, contactId);
        }

        public static Fax FromDataRow(DataRow dataRow)
        {
            return new Fax()
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