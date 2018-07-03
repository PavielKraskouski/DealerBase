using DealerBase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DealerBase.Windows
{
    /// <summary>
    /// Логика взаимодействия для DealerWindow.xaml
    /// </summary>
    public partial class DealerWindow : Window
    {
        public Dealer Dealer { get; set; }

        private void UpdateContacts()
        {
            Contacts.Items.Clear();
            Dealer.Contacts.OrderBy(x => x.Surname).ThenBy(x => x.Name).ThenBy(x => x.Patronymic).ThenBy(x => x.Position).ForEach(x => Contacts.Items.Add(x.ToTextBlock()));
        }

        private void UpdateEvents()
        {
            Events.Items.Clear();
            IEnumerable<Event> upcomingEvents = Dealer.Events.Where(x => x.DateAdded.Date > DateTime.Now.Date).OrderByDescending(x => x.DateAdded).ThenBy(x => x.Header);
            IEnumerable<Event> currentEvents = Dealer.Events.Where(x => x.DateAdded.Date == DateTime.Now.Date).OrderByDescending(x => x.DateAdded).ThenBy(x => x.Header);
            IEnumerable<Event> pastEvents = Dealer.Events.Where(x => x.DateAdded.Date < DateTime.Now.Date).OrderByDescending(x => x.DateAdded).ThenBy(x => x.Header);
            if (upcomingEvents.Count() != 0)
            {
                Events.Items.Add(new Separator() { Tag = "Предстоящие" });
                upcomingEvents.ForEach(x => Events.Items.Add(x.ToTextBlock()));
            }
            if (currentEvents.Count() != 0)
            {
                Events.Items.Add(new Separator() { Tag = "Текущие" });
                currentEvents.ForEach(x => Events.Items.Add(x.ToTextBlock()));
            }
            if (pastEvents.Count() != 0)
            {
                Events.Items.Add(new Separator() { Tag = "Прошедшие" });
                pastEvents.ForEach(x => Events.Items.Add(x.ToTextBlock()));
            }
        }

        public DealerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Entities.BusinessEntity.Select().ForEach(x => BusinessEntity.Items.Add(Entities.BusinessEntity.ToTextBlock(x)));
            Entities.Activity.Select().ForEach(x => Activity.Items.Add(Entities.Activity.ToTextBlock(x)));
            Entities.ActivityDirection.Select().ForEach(x => ActivityDirection.Items.Add(Entities.ActivityDirection.ToTextBlock(x)));
            Entities.Region.Select().ForEach(x => Region.Items.Add(Entities.Region.ToTextBlock(x)));
            if (Dealer == null)
            {
                Dealer = new Dealer();
                BusinessEntity.SelectedIndex = 0;
                Activity.SelectedIndex = 0;
                ActivityDirection.SelectedIndex = 0;
                Region.SelectedIndex = 0;
                Title = "Добавление дилера";
            }
            else
            {
                BusinessEntity.SelectedItem = BusinessEntity.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == Dealer.BusinessEntityId);
                _Name.Text = Dealer.Name;
                Activity.SelectedItem = Activity.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == Dealer.ActivityId);
                ActivityDirection.SelectedItem = ActivityDirection.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == Dealer.ActivityDirectionId);
                Rating.SelectedIndex = 5 - (int)Dealer.Rating;
                Relevance.SelectedIndex = Dealer.IsRelevant ? 0 : 1;
                Region.SelectedItem = Region.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == Dealer.RegionId);
                City.Text = Dealer.City;
                Street.Text = Dealer.Street;
                House.Text = Dealer.House;
                Block.Text = Dealer.Block;
                Room.Text = Dealer.Room;
                UpdateContacts();
                Contacts.SelectedIndex = 0;
                Note.Text = Dealer.Note;
                Conditions.Text = Dealer.Conditions;
                UpdateEvents();
                Events.SelectedIndex = 1;
                Title = "Правка дилера";
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (TabControl.SelectedIndex)
            {
                case 0:
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Keyboard.Focus(_Name)));
                    break;
                case 1:
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Keyboard.Focus(City)));
                    break;
                case 3:
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Keyboard.Focus(Note)));
                    break;
                case 4:
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Keyboard.Focus(Conditions)));
                    break;

            }
        }

        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
            ContactWindow contactWindow = new ContactWindow()
            {
                Owner = this
            };
            if ((bool)contactWindow.ShowDialog())
            {
                contactWindow.Contact.Id = Dealer.Contacts.Count != 0 ? Dealer.Contacts.Max(x => x.Id) + 1 : 1;
                Dealer.Contacts.Add(contactWindow.Contact);
                UpdateContacts();
                Contacts.SelectedItem = Contacts.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == contactWindow.Contact.Id);
                Contacts.ScrollIntoView(Contacts.SelectedItem);
            }
        }

        private void EditContact_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded && Contacts.SelectedItem != null;
        }

        private void EditContact_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ContactWindow contactWindow = new ContactWindow()
            {
                Owner = this,
                Contact = Dealer.Contacts.First(x => x.Id == (long)(Contacts.SelectedItem as TextBlock).Tag).Clone()
            };
            if ((bool)contactWindow.ShowDialog())
            {
                Dealer.Contacts[Dealer.Contacts.FindIndex(x => x.Id == contactWindow.Contact.Id)] = contactWindow.Contact;
                UpdateContacts();
                Contacts.SelectedItem = Contacts.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == contactWindow.Contact.Id);
                Contacts.ScrollIntoView(Contacts.SelectedItem);
            }
        }

        private void Contact_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditContact.Command.Execute(null);
        }

        private void DeleteContact_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded && Contacts.SelectedItem != null;
        }

        private void DeleteContact_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ConfirmationWindow confirmationWindow = new ConfirmationWindow()
            {
                Owner = this
            };
            if ((bool)confirmationWindow.ShowDialog())
            {
                int selectedIndex = Contacts.SelectedIndex;
                Dealer.Contacts.Remove(Dealer.Contacts.First(x => x.Id == (long)(Contacts.SelectedItem as TextBlock).Tag));
                UpdateContacts();
                Contacts.SelectedIndex = Math.Max(0, selectedIndex - 1);
                Contacts.ScrollIntoView(Contacts.SelectedItem);
            }
        }

        private void AddEvent_Click(object sender, RoutedEventArgs e)
        {
            EventWindow eventWindow = new EventWindow()
            {
                Owner = this
            };
            if ((bool)eventWindow.ShowDialog())
            {
                eventWindow.Event.Id = Dealer.Events.Count != 0 ? Dealer.Events.Max(x => x.Id) + 1 : 1;
                Dealer.Events.Add(eventWindow.Event);
                UpdateEvents();
                Events.SelectedItem = Events.Items.FirstOrDefault<FrameworkElement>(x => x.Tag is long && (long)x.Tag == eventWindow.Event.Id);
                Events.ScrollIntoView(Events.SelectedItem);
            }
        }

        private void EditEvent_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded && Events.SelectedItem != null;
        }

        private void EditEvent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EventWindow eventWindow = new EventWindow()
            {
                Owner = this,
                Event = Dealer.Events.First(x => x.Id == (long)(Events.SelectedItem as TextBlock).Tag).Clone()
            };
            if ((bool)eventWindow.ShowDialog())
            {
                Dealer.Events[Dealer.Events.FindIndex(x => x.Id == eventWindow.Event.Id)] = eventWindow.Event;
                UpdateEvents();
                Events.SelectedItem = Events.Items.FirstOrDefault<FrameworkElement>(x => x.Tag is long && (long)x.Tag == eventWindow.Event.Id);
                Events.ScrollIntoView(Events.SelectedItem);
            }
        }

        private void Event_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditEvent.Command.Execute(null);
        }

        private void DeleteEvent_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded && Events.SelectedItem != null;
        }

        private void DeleteEvent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ConfirmationWindow confirmationWindow = new ConfirmationWindow()
            {
                Owner = this
            };
            if ((bool)confirmationWindow.ShowDialog())
            {
                int selectedIndex = Events.SelectedIndex;
                Dealer.Events.Remove(Dealer.Events.First(x => x.Id == (long)(Events.SelectedItem as TextBlock).Tag));
                UpdateEvents();
                Events.SelectedIndex = selectedIndex != 1 ? (Events.Items[Math.Min(Events.Items.Count - 1, selectedIndex - 1)] is TextBlock ? Math.Min(Events.Items.Count - 1, selectedIndex - 1) : Math.Min(Events.Items.Count - 1, selectedIndex - 2)) : 1;
                Events.ScrollIntoView(Events.SelectedItem);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Name.Text != string.Empty && !(Dealer.Name.ToUpper() != _Name.Text.ToUpper() && Dealer.Exists(_Name.Text));
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Dealer.BusinessEntityId = (long)(BusinessEntity.SelectedItem as TextBlock).Tag;
            Dealer.Name = _Name.Text;
            Dealer.ActivityId = (long)(Activity.SelectedItem as TextBlock).Tag;
            Dealer.ActivityDirectionId = (long)(ActivityDirection.SelectedItem as TextBlock).Tag;
            Dealer.Rating = 5 - Rating.SelectedIndex;
            Dealer.IsRelevant = Relevance.SelectedIndex == 0;
            Dealer.RegionId = (long)(Region.SelectedItem as TextBlock).Tag;
            Dealer.City = City.Text;
            Dealer.Street = Street.Text;
            Dealer.House = House.Text;
            Dealer.Block = Block.Text;
            Dealer.Room = Room.Text;
            Dealer.Note = Note.Text;
            Dealer.Conditions = Conditions.Text;
            DialogResult = true;
            SystemCommands.CloseWindow(this);
        }
    }
}