using System.Media;
using System.Windows;

namespace DealerBase.Windows
{
    /// <summary>
    /// Логика взаимодействия для ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        private static readonly string[] ErrorMessages = new string[]
        {
            "Невозможно добавить запись.\nЗадано недостаточно констант.",
            "Невозможно изменить запись.\nЗадано недостаточно констант.",
            "Невозможно изменить запись.\nЗапись удалена из базы данных.",
            "Невозможно удалить запись.\nЗапись удалена из базы данных.",
            "Невозможно удалить запись.\nЗапись связана с другими данными.",
            "Невозможно напечатать документ.\nОтсутствуют данные для печати.",
            "Невозможно создать список email адресов.\nОтсутствуют данные для сохранения.",
            "Ошибка доступа к базе данных.\nРабота программы будет прекращена."
        };

        public ErrorWindow(byte errorCode)
        {
            InitializeComponent();
            MessageText.Text = ErrorMessages[errorCode];
            SystemSounds.Hand.Play();
            this.FixLayout();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }
    }
}