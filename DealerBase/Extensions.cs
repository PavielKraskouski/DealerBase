using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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

        public static void SelectItem(this Selector selector, object selectedItem = null, int selectedIndex = 0)
        {
            if (selectedItem != null)
            {
                selector.SelectedItem = selectedItem;
            }
            else
            {
                selector.SelectedIndex = selectedIndex;
            }
            if (selector is ListBox listBox)
            {
                listBox.ScrollIntoView(selector.SelectedItem);
            }
        }

        public static bool? ShowDialog(this Window window, Window owner)
        {
            window.Owner = owner;
            return window.ShowDialog();
        }

        public static void FixLayout(this Window window)
        {
            bool arrangeRequired = false;
            double deltaWidth = 0;
            double deltaHeight = 0;

            void Window_SourceInitialized(object sender, EventArgs e)
            {
                window.InvalidateMeasure();
                arrangeRequired = true;
                window.SourceInitialized -= Window_SourceInitialized;
            }

            void CalculateDeltaSize()
            {
                deltaWidth = window.ActualWidth - deltaWidth;
                deltaHeight = window.ActualHeight - deltaHeight;
            }

            void Window_LayoutUpdated(object sender, EventArgs e)
            {
                if (arrangeRequired)
                {
                    if (window.SizeToContent == SizeToContent.WidthAndHeight)
                    {
                        CalculateDeltaSize();
                    }
                    window.Left -= deltaWidth * 0.5;
                    window.Top -= deltaHeight * 0.5;
                    window.LayoutUpdated -= Window_LayoutUpdated;
                }
                else
                {
                    CalculateDeltaSize();
                }
            }

            window.SourceInitialized += Window_SourceInitialized;
            window.LayoutUpdated += Window_LayoutUpdated;
        }
    }
}