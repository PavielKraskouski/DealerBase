using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Controls;

namespace DealerBase
{
    public static class Extensions
    {
        public static T Clone<T>(this T source)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, source);
                memoryStream.Position = 0;
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        public static void RemoveRange(this ItemCollection itemCollection, int index, int count)
        {
            index = Math.Min(itemCollection.Count - 1, Math.Max(0, index));
            count = Math.Min(itemCollection.Count - index, Math.Max(0, count));
            for (int i = index + count - 1; i >= index; i--)
            {
                itemCollection.RemoveAt(i);
            }
        }

        public static T FirstOrDefault<T>(this ItemCollection itemCollection, Func<T, bool> predicate)
        {
            return itemCollection.Cast<T>().FirstOrDefault(predicate);
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}