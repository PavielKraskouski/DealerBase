﻿using DealerBase.Windows;
using System.Collections.Generic;
using System.Windows.Controls;

namespace DealerBase
{
    public class Filter
    {
        private static readonly string[] SortingMethods = new string[]
        {
            " ORDER BY DATETIME(DateAdded) ASC, UPPER(Dealer.Name) ASC", " ORDER BY UPPER(Dealer.Name) ASC", " ORDER BY Rating ASC, UPPER(Dealer.Name) ASC",
            " ORDER BY DATETIME(DateAdded) DESC, UPPER(Dealer.Name) ASC", " ORDER BY UPPER(Dealer.Name) DESC", " ORDER BY Rating DESC, UPPER(Dealer.Name) ASC"
        };

        public string Query { get; private set; }
        public object[] Parameters { get; private set; }

        public Filter(bool ordered)
        {
            List<object> parameters = new List<object>();
            Query = "WHERE 1 = 1";
            if (MainWindow.Instance.Search.Text != string.Empty)
            {
                parameters.Add(MainWindow.Instance.Search.Text.ToUpper());
                Query += $" AND UPPER(Dealer.Name) LIKE @param{parameters.Count} || '%'";
            }
            if (MainWindow.Instance.Region.SelectedIndex != 0)
            {
                parameters.Add((MainWindow.Instance.Region.SelectedItem as TextBlock).Tag);
                Query += $" AND RegionId = @param{parameters.Count}";
            }
            if (MainWindow.Instance.Activity.SelectedIndex != 0)
            {
                parameters.Add((MainWindow.Instance.Activity.SelectedItem as TextBlock).Tag);
                Query += $" AND ActivityId = @param{parameters.Count}";
            }
            if (MainWindow.Instance.ActivityDirection.SelectedIndex != 0)
            {
                parameters.Add((MainWindow.Instance.ActivityDirection.SelectedItem as TextBlock).Tag);
                Query += $" AND ActivityDirectionId = @param{parameters.Count}";
            }
            parameters.Add(1 - MainWindow.Instance.Relevance.SelectedIndex);
            Query += $" AND IsRelevant = @param{parameters.Count}";
            if (ordered)
            {
                Query += SortingMethods[MainWindow.Instance.Sort.SelectedIndex];
            }
            Parameters = parameters.ToArray();
        }
    }
}