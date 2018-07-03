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
        public string Value { get; set; }

        public ConstantWindow()
        {
            InitializeComponent();
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
                ConstantsWindow constantsWindow = this.Owner as ConstantsWindow;
                switch (constantsWindow.Constant.SelectedIndex)
                {
                    case 0:
                        e.CanExecute = !(Value.ToUpper() != TextBox.Text.ToUpper() && BusinessEntity.Exists(TextBox.Text));
                        break;
                    case 1:
                        e.CanExecute = !(Value.ToUpper() != TextBox.Text.ToUpper() && Activity.Exists(TextBox.Text));
                        break;
                    case 2:
                        e.CanExecute = !(Value.ToUpper() != TextBox.Text.ToUpper() && ActivityDirection.Exists(TextBox.Text));
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