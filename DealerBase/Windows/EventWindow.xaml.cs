using DealerBase.Entities;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DealerBase.Windows
{
    /// <summary>
    /// Логика взаимодействия для EventWindow.xaml
    /// </summary>
    public partial class EventWindow : Window
    {
        public Event Event { get; private set; }

        public EventWindow(Event _event = null)
        {
            InitializeComponent();
            Event = _event;
            this.FixLayout();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Event == null)
            {
                Event = new Event();
                Title = "Добавление события";
            }
            else
            {
                Header.Text = Event.Header;
                Description.Text = Event.Description;
                Date.SelectedDate = Event.DateAdded;
                Title = "Правка события";
            }
            Accept.Content = Owner is DealerWindow ? "OK" : "Сохранить";
            (Date.Template.FindName("PART_TextBox", Date) as DatePickerTextBox).ContextMenu = null;
            Keyboard.Focus(Header);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Accept_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Header.Text != string.Empty && DateTime.TryParse(Date.Text, out DateTime dateTime);
        }

        private void Accept_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Event.Header = Header.Text;
            Event.Description = Description.Text;
            Event.DateAdded = (DateTime)Date.SelectedDate;
            DialogResult = true;
            SystemCommands.CloseWindow(this);
        }
    }
}