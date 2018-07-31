using DealerBase.Entities;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DealerBase.Windows
{
    /// <summary>
    /// Логика взаимодействия для NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        private void UpdateEvents()
        {
            Events.Items.Clear();
            Event.Select(DateTime.Now).ForEach(x =>
            {
                TextBlock textBlock = new TextBlock() { Tag = x.Field<long>("Id") };
                textBlock.Inlines.Add(new Run(x.Field<string>("Header"))
                {
                    FontSize = 20,
                    FontWeight = FontWeights.Bold
                });
                textBlock.Inlines.Add(new Run("\nДилер: ") { Foreground = Brushes.Silver });
                textBlock.Inlines.Add(new Run(String.Format("{0} \"{1}\"", x.Field<string>("Name"), x.Field<string>("Name1"))));
                Events.Items.Add(textBlock);
            });
        }

        private void ShowErrorWindow(byte errorCode)
        {
            new ErrorWindow(errorCode).ShowDialog(this);
            UpdateEvents();
            Events.SelectItem();
        }

        public NotificationWindow()
        {
            InitializeComponent();
            this.FixLayout();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateEvents();
            Events.SelectItem();
        }

        private void Edit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Events.SelectedItem != null;
        }

        private void Edit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Event.Exists((long)(Events.SelectedItem as TextBlock).Tag))
            {
                EventWindow eventWindow = new EventWindow(Event.FromDataRow(Event.SelectOne((long)(Events.SelectedItem as TextBlock).Tag)));
                if ((bool)eventWindow.ShowDialog(this))
                {
                    if (Event.Exists((long)(Events.SelectedItem as TextBlock).Tag))
                    {
                        long selectedEventId = (long)(Events.SelectedItem as TextBlock).Tag;
                        eventWindow.Event.Update();
                        UpdateEvents();
                        Events.SelectItem(Events.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == selectedEventId));
                    }
                    else
                    {
                        ShowErrorWindow(2);
                    }
                }
            }
            else
            {
                ShowErrorWindow(2);
            }
        }

        private void Event_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Edit.Command.Execute(null);
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Events.SelectedItem != null;
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Event.Exists((long)(Events.SelectedItem as TextBlock).Tag))
            {
                if ((bool)new ConfirmationWindow().ShowDialog(this))
                {
                    if (Event.Exists((long)(Events.SelectedItem as TextBlock).Tag))
                    {
                        Event.DeleteOne((long)(Events.SelectedItem as TextBlock).Tag);
                        UpdateEvents();
                        Events.SelectItem();
                    }
                    else
                    {
                        ShowErrorWindow(3);
                    }
                }
            }
            else
            {
                ShowErrorWindow(3);
            }
        }
    }
}