using DealerBase.Entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DealerBase.Windows
{
    /// <summary>
    /// Логика взаимодействия для ContactWindow.xaml
    /// </summary>
    public partial class ContactWindow : Window
    {
        public Contact Contact { get; set; }

        private void UpdateValues()
        {
            Values.Items.Clear();
            switch (CommunicationMean.SelectedIndex)
            {
                case 0:
                    Contact.Phones.OrderBy(x => x.Value).ForEach(x => Values.Items.Add(x.ToTextBlock()));
                    break;
                case 1:
                    Contact.Faxes.OrderBy(x => x.Value).ForEach(x => Values.Items.Add(x.ToTextBlock()));
                    break;
                case 2:
                    Contact.Emails.OrderBy(x => x.Value).ForEach(x => Values.Items.Add(x.ToTextBlock()));
                    break;
            }
        }

        public ContactWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Contact == null)
            {
                Contact = new Contact();
                Title = "Добавление контактного лица";
            }
            else
            {
                Surname.Text = Contact.Surname;
                _Name.Text = Contact.Name;
                Patronymic.Text = Contact.Patronymic;
                Position.Text = Contact.Position;
                UpdateValues();
                Values.SelectedIndex = 0;
                Title = "Правка контактного лица";
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabControl.SelectedIndex == 0)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Keyboard.Focus(Surname)));
            }
        }

        private void CommunicationMean_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CommunicationMean.IsLoaded)
            {
                TextBlock.Text = CommunicationMean.SelectedIndex < 2 ? "Список номеров:" : "Список адресов:";
                UpdateValues();
                Values.SelectedIndex = 0;
                Values.ScrollIntoView(Values.SelectedItem);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NumberAddressWindow numberAddressWindow = new NumberAddressWindow()
            {
                Owner = this
            };
            if ((bool)numberAddressWindow.ShowDialog())
            {
                long insertedValueId;
                switch (CommunicationMean.SelectedIndex)
                {
                    case 0:
                        insertedValueId = Contact.Phones.Count != 0 ? Contact.Phones.Max(x => x.Id) + 1 : 1;
                        Contact.Phones.Add(new Phone()
                        {
                            Id = insertedValueId,
                            Value = numberAddressWindow.Value
                        });
                        break;
                    case 1:
                        insertedValueId = Contact.Faxes.Count != 0 ? Contact.Faxes.Max(x => x.Id) + 1 : 1;
                        Contact.Faxes.Add(new Fax()
                        {
                            Id = insertedValueId,
                            Value = numberAddressWindow.Value
                        });
                        break;
                    default:
                        insertedValueId = Contact.Emails.Count != 0 ? Contact.Emails.Max(x => x.Id) + 1 : 1;
                        Contact.Emails.Add(new Email()
                        {
                            Id = insertedValueId,
                            Value = numberAddressWindow.Value
                        });
                        break;
                }
                UpdateValues();
                Values.SelectedItem = Values.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == insertedValueId);
                Values.ScrollIntoView(Values.SelectedItem);
            }
        }

        private void Edit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded && Values.SelectedItem != null;
        }

        private void Edit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NumberAddressWindow numberAddressWindow = new NumberAddressWindow()
            {
                Owner = this,
                Value = (Values.SelectedItem as TextBlock).Text
            };
            if ((bool)numberAddressWindow.ShowDialog())
            {
                long selectedValueId = (long)(Values.SelectedItem as TextBlock).Tag;
                switch (CommunicationMean.SelectedIndex)
                {
                    case 0:
                        Contact.Phones.First(x => x.Id == selectedValueId).Value = numberAddressWindow.Value;
                        break;
                    case 1:
                        Contact.Faxes.First(x => x.Id == selectedValueId).Value = numberAddressWindow.Value;
                        break;
                    case 2:
                        Contact.Emails.First(x => x.Id == selectedValueId).Value = numberAddressWindow.Value;
                        break;
                }
                UpdateValues();
                Values.SelectedItem = Values.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == selectedValueId);
                Values.ScrollIntoView(Values.SelectedItem);
            }
        }

        private void Value_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Edit.Command.Execute(null);
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded && Values.SelectedItem != null;
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ConfirmationWindow confirmationWindow = new ConfirmationWindow()
            {
                Owner = this
            };
            if ((bool)confirmationWindow.ShowDialog())
            {
                int selectedValueIndex = Values.SelectedIndex;
                switch (CommunicationMean.SelectedIndex)
                {
                    case 0:
                        Contact.Phones.Remove(Contact.Phones.First(x => x.Id == (long)(Values.SelectedItem as TextBlock).Tag));
                        break;
                    case 1:
                        Contact.Faxes.Remove(Contact.Faxes.First(x => x.Id == (long)(Values.SelectedItem as TextBlock).Tag));
                        break;
                    case 2:
                        Contact.Emails.Remove(Contact.Emails.First(x => x.Id == (long)(Values.SelectedItem as TextBlock).Tag));
                        break;
                }
                UpdateValues();
                Values.SelectedIndex = Math.Max(0, selectedValueIndex - 1);
                Values.ScrollIntoView(Values.SelectedItem);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Accept_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Name.Text != string.Empty;
        }

        private void Accept_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Contact.Surname = Surname.Text;
            Contact.Name = _Name.Text;
            Contact.Patronymic = Patronymic.Text;
            Contact.Position = Position.Text;
            DialogResult = true;
            SystemCommands.CloseWindow(this);
        }
    }
}