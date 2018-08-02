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
        public Dealer Dealer { get; private set; }

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

        public DealerWindow(Dealer dealer = null)
        {
            InitializeComponent();
            Dealer = dealer;
            this.FixLayout();
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
                BusinessEntity.SelectItem();
                Activity.SelectItem();
                ActivityDirection.SelectItem();
                Region.SelectItem();
                Title = "Добавление дилера";
            }
            else
            {
                BusinessEntity.SelectItem(BusinessEntity.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == Dealer.BusinessEntityId));
                _Name.Text = Dealer.Name;
                Activity.SelectItem(Activity.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == Dealer.ActivityId));
                ActivityDirection.SelectItem(ActivityDirection.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == Dealer.ActivityDirectionId));
                Rating.SelectItem(selectedIndex: 5 - (int)Dealer.Rating);
                Relevance.SelectItem(selectedIndex: Dealer.IsRelevant ? 0 : 1);
                Region.SelectItem(Region.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == Dealer.RegionId));
                City.Text = Dealer.City;
                Street.Text = Dealer.Street;
                House.Text = Dealer.House;
                Block.Text = Dealer.Block;
                Room.Text = Dealer.Room;
                UpdateContacts();
                Contacts.SelectItem();
                Note.Text = Dealer.Note;
                Conditions.Text = Dealer.Conditions;
                UpdateEvents();
                Events.SelectItem(selectedIndex: 1);
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
            ContactWindow contactWindow = new ContactWindow();
            if ((bool)contactWindow.ShowDialog(this))
            {
                contactWindow.Contact.Id = Dealer.Contacts.Count != 0 ? Dealer.Contacts.Max(x => x.Id) + 1 : 1;
                Dealer.Contacts.Add(contactWindow.Contact);
                UpdateContacts();
                Contacts.SelectItem(Contacts.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == contactWindow.Contact.Id));
            }
        }

        private void EditContact_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded && Contacts.SelectedItem != null;
        }

        private void EditContact_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ContactWindow contactWindow = new ContactWindow(Dealer.Contacts.First(x => x.Id == (long)(Contacts.SelectedItem as TextBlock).Tag).Clone());
            if ((bool)contactWindow.ShowDialog(this))
            {
                Dealer.Contacts[Dealer.Contacts.FindIndex(x => x.Id == contactWindow.Contact.Id)] = contactWindow.Contact;
                UpdateContacts();
                Contacts.SelectItem(Contacts.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == contactWindow.Contact.Id));
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
            if ((bool)new ConfirmationWindow().ShowDialog(this))
            {
                Dealer.Contacts.Remove(Dealer.Contacts.First(x => x.Id == (long)(Contacts.SelectedItem as TextBlock).Tag));
                UpdateContacts();
                Contacts.SelectItem();
            }
        }

        private void AddEvent_Click(object sender, RoutedEventArgs e)
        {
            EventWindow eventWindow = new EventWindow();
            if ((bool)eventWindow.ShowDialog(this))
            {
                eventWindow.Event.Id = Dealer.Events.Count != 0 ? Dealer.Events.Max(x => x.Id) + 1 : 1;
                Dealer.Events.Add(eventWindow.Event);
                UpdateEvents();
                Events.SelectItem(Events.Items.FirstOrDefault<FrameworkElement>(x => x.Tag is long && (long)x.Tag == eventWindow.Event.Id));
            }
        }

        private void EditEvent_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded && Events.SelectedItem != null;
        }

        private void EditEvent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EventWindow eventWindow = new EventWindow(Dealer.Events.First(x => x.Id == (long)(Events.SelectedItem as TextBlock).Tag).Clone());
            if ((bool)eventWindow.ShowDialog(this))
            {
                Dealer.Events[Dealer.Events.FindIndex(x => x.Id == eventWindow.Event.Id)] = eventWindow.Event;
                UpdateEvents();
                Events.SelectItem(Events.Items.FirstOrDefault<FrameworkElement>(x => x.Tag is long && (long)x.Tag == eventWindow.Event.Id));
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
            if ((bool)new ConfirmationWindow().ShowDialog(this))
            {
                Dealer.Events.Remove(Dealer.Events.First(x => x.Id == (long)(Events.SelectedItem as TextBlock).Tag));
                UpdateEvents();
                Events.SelectItem(selectedIndex: 1);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Name.Text != string.Empty && !ErrorWindow.CriticalError && !Dealer.Exists(_Name.Text, Dealer.Id);
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