using DealerBase.Entities;
using System.Windows;
using System.Windows.Input;

namespace DealerBase.Windows
{
    /// <summary>
    /// Логика взаимодействия для ConstantWindow.xaml
    /// </summary>
    public partial class ConstantWindow : Window
    {
        public string Value { get; private set; }

        public ConstantWindow(string value = null, long valueId = 0)
        {
            InitializeComponent();
            Value = value;
            Tag = valueId;
            this.FixLayout();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Value == null)
            {
                Value = string.Empty;
                Title = "Добавление значения";
            }
            else
            {
                TextBox.Text = Value;
                Title = "Правка значения";
            }
            Keyboard.Focus(TextBox);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (TextBox.Text != string.Empty)
            {
                switch ((Owner as ConstantsWindow).Constant.SelectedIndex)
                {
                    case 0:
                        e.CanExecute = !ErrorWindow.CriticalError && !BusinessEntity.Exists(TextBox.Text, (long)Tag);
                        break;
                    case 1:
                        e.CanExecute = !ErrorWindow.CriticalError && !Activity.Exists(TextBox.Text, (long)Tag);
                        break;
                    case 2:
                        e.CanExecute = !ErrorWindow.CriticalError && !ActivityDirection.Exists(TextBox.Text, (long)Tag);
                        break;
                }
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Value = TextBox.Text;
            DialogResult = true;
            SystemCommands.CloseWindow(this);
        }
    }
}