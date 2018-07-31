using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DealerBase.Windows
{
    /// <summary>
    /// Логика взаимодействия для NumberAddressWindow.xaml
    /// </summary>
    public partial class NumberAddressWindow : Window
    {
        public string Value { get; private set; }

        public NumberAddressWindow(string value = null)
        {
            InitializeComponent();
            Value = value;
            this.FixLayout();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Value == null)
            {
                Value = string.Empty;
                Title = "Добавление ";
            }
            else
            {
                Title = "Правка ";
            }
            if ((Owner as ContactWindow).CommunicationMean.SelectedIndex < 2)
            {
                Title += "номера";
                TextBlock.Text = "Номер:";
                TextBox.Mask = "+375(00)000-00-00";
            }
            else
            {
                Title += "адреса";
                TextBlock.Text = "Адрес:";
            }
            TextBox.Text = Value;
            Keyboard.Focus(TextBox);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Accept_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsLoaded)
            {
                ContactWindow contactWindow = Owner as ContactWindow;
                switch (contactWindow.CommunicationMean.SelectedIndex)
                {
                    case 0:
                        e.CanExecute = TextBox.IsMaskFull && !(Value != TextBox.Text && contactWindow.Contact.Phones.Any(x => x.Value == TextBox.Text));
                        break;
                    case 1:
                        e.CanExecute = TextBox.IsMaskFull && !(Value != TextBox.Text && contactWindow.Contact.Faxes.Any(x => x.Value == TextBox.Text));
                        break;
                    case 2:
                        string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
                        e.CanExecute = Regex.IsMatch(TextBox.Text, pattern, RegexOptions.IgnoreCase) && !(Value.ToUpper() != TextBox.Text.ToUpper() && contactWindow.Contact.Emails.Any(x => x.Value.ToUpper() == TextBox.Text.ToUpper()));
                        break;
                }
            }
        }

        private void Accept_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Value = TextBox.Text;
            DialogResult = true;
            SystemCommands.CloseWindow(this);
        }
    }
}