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

        public NotificationWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateEvents();
            Events.SelectedIndex = 0;
        }

        private void Edit_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Events.SelectedItem != null;
        }

        private void Edit_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (Event.Exists((long)(Events.SelectedItem as TextBlock).Tag))
            {
                EventWindow eventWindow = new EventWindow()
                {
                    Owner = this,
                    Event = Event.FromDataRow(Event.SelectOne((long)(Events.SelectedItem as TextBlock).Tag))
                };
                if ((bool)eventWindow.ShowDialog())
                {
                    if (Event.Exists((long)(Events.SelectedItem as TextBlock).Tag))
                    {
                        long selectedEventId = (long)(Events.SelectedItem as TextBlock).Tag;
                        eventWindow.Event.Update();
                        UpdateEvents();
                        Events.SelectedItem = Events.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == selectedEventId);
                        Events.ScrollIntoView(Events.SelectedItem);
                    }
                    else
                    {
                        ErrorWindow errorWindow = new ErrorWindow()
                        {
                            Owner = this
                        };
                        errorWindow.ShowDialog();
                    }
                }
            }
            else
            {
                ErrorWindow errorWindow = new ErrorWindow()
                {
                    Owner = this
                };
                errorWindow.ShowDialog();
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
                ConfirmationWindow confirmationWindow = new ConfirmationWindow()
                {
                    Owner = this
                };
                if ((bool)confirmationWindow.ShowDialog())
                {
                    if (Event.Exists((long)(Events.SelectedItem as TextBlock).Tag))
                    {
                        int selectedIndex = Events.SelectedIndex;
                        Event.DeleteOne((long)(Events.SelectedItem as TextBlock).Tag);
                        UpdateEvents();
                        Events.SelectedIndex = Math.Max(0, Math.Min(Events.Items.Count - 1, selectedIndex - 1));
                        Events.ScrollIntoView(Events.SelectedItem);
                    }
                    else
                    {
                        ErrorWindow errorWindow = new ErrorWindow()
                        {
                            Owner = this
                        };
                        errorWindow.ShowDialog();
                    }
                }
            }
            else
            {
                ErrorWindow errorWindow = new ErrorWindow()
                {
                    Owner = this
                };
                errorWindow.ShowDialog();
            }
        }
    }
}