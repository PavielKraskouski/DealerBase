using DealerBase.Entities;
using Microsoft.Win32;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DealerBase.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        public static Window ActiveWindow { get; set; }

        public void UpdateDealers()
        {
            long selectedRegionId = (long)(Region.SelectedItem as TextBlock).Tag;
            long selectedActivityId = (long)(Activity.SelectedItem as TextBlock).Tag;
            long selectedActivityDirectionId = (long)(ActivityDirection.SelectedItem as TextBlock).Tag;
            Region.Items.RemoveRange(1, Region.Items.Count - 1);
            Entities.Region.Select().ForEach(x => Region.Items.Add(Entities.Region.ToTextBlock(x)));
            Region.SelectItem(Region.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == selectedRegionId));
            Activity.Items.RemoveRange(1, Activity.Items.Count - 1);
            Entities.Activity.Select().ForEach(x => Activity.Items.Add(Entities.Activity.ToTextBlock(x)));
            Activity.SelectItem(Activity.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == selectedActivityId));
            ActivityDirection.Items.RemoveRange(1, ActivityDirection.Items.Count - 1);
            Entities.ActivityDirection.Select().ForEach(x => ActivityDirection.Items.Add(Entities.ActivityDirection.ToTextBlock(x)));
            ActivityDirection.SelectItem(ActivityDirection.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == selectedActivityDirectionId));
            Region.Tag = (Region.SelectedItem as TextBlock).Tag;
            Activity.Tag = (Activity.SelectedItem as TextBlock).Tag;
            ActivityDirection.Tag = (ActivityDirection.SelectedItem as TextBlock).Tag;
            Relevance.Tag = (long)Relevance.SelectedIndex;
            Sort.Tag = (long)Sort.SelectedIndex;
            Dealers.Items.Clear();
            Dealer.Select(new Filter(true)).ForEach(x => Dealers.Items.Add(Dealer.ToTextBlock(x)));
        }

        private void ShowErrorWindow(byte errorCode)
        {
            new ErrorWindow(errorCode).ShowDialog(this);
            UpdateDealers();
            Dealers.SelectItem();
        }

        public MainWindow()
        {
            DBAccess.CreateDatabase();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
            ActiveWindow = this;
            UpdateDealers();
            Dealers.SelectItem();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            DBAccess.BackupDatabase();
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if ((long)comboBox.Tag != (long)(comboBox.SelectedItem as TextBlock).Tag)
            {
                UpdateDealers();
                Dealers.SelectItem();
            }
        }

        private void Add_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !ErrorWindow.CriticalError && BusinessEntity.Count() != 0 && Entities.Activity.Count() != 0 && Entities.ActivityDirection.Count() != 0;
        }

        private void Add_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DealerWindow dealerWindow = new DealerWindow();
            if ((bool)dealerWindow.ShowDialog(this))
            {
                if (BusinessEntity.Exists(dealerWindow.Dealer.BusinessEntityId) &&
                    Entities.Activity.Exists(dealerWindow.Dealer.ActivityId) &&
                    Entities.ActivityDirection.Exists(dealerWindow.Dealer.ActivityDirectionId))
                {
                    long insertedDealerId = dealerWindow.Dealer.Insert();
                    UpdateDealers();
                    Dealers.SelectItem(Dealers.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == insertedDealerId));
                }
                else
                {
                    ShowErrorWindow(0);
                }
            }
        }

        private void Edit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded && Dealers.SelectedItem != null;
        }

        private void Edit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Dealer.Exists((long)(Dealers.SelectedItem as TextBlock).Tag))
            {
                DealerWindow dealerWindow = new DealerWindow(Dealer.FromDataRow(Dealer.Select((long)(Dealers.SelectedItem as TextBlock).Tag)));
                if ((bool)dealerWindow.ShowDialog(this))
                {
                    if (Dealer.Exists((long)(Dealers.SelectedItem as TextBlock).Tag))
                    {
                        if (BusinessEntity.Exists(dealerWindow.Dealer.BusinessEntityId) &&
                            Entities.Activity.Exists(dealerWindow.Dealer.ActivityId) &&
                            Entities.ActivityDirection.Exists(dealerWindow.Dealer.ActivityDirectionId))
                        {
                            long selectedDealerId = (long)(Dealers.SelectedItem as TextBlock).Tag;
                            dealerWindow.Dealer.Update();
                            UpdateDealers();
                            Dealers.SelectItem(Dealers.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == selectedDealerId));
                        }
                        else
                        {
                            ShowErrorWindow(1);
                        }
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

        private void Dealer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Edit.Command.Execute(null);
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded && Dealers.SelectedItem != null;
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Dealer.Exists((long)(Dealers.SelectedItem as TextBlock).Tag))
            {
                if ((bool)new ConfirmationWindow().ShowDialog(this))
                {
                    if (Dealer.Exists((long)(Dealers.SelectedItem as TextBlock).Tag))
                    {
                        Dealer.Delete((long)(Dealers.SelectedItem as TextBlock).Tag);
                        UpdateDealers();
                        Dealers.SelectItem();
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

        private void Backup_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Файл базы данных (*.db)|*.db",
                FileName = "DealerBase_Backup.db"
            };
            if ((bool)saveFileDialog.ShowDialog())
            {
                DBAccess.BackupDatabase(saveFileDialog.FileName);
            }
        }

        private void Emails_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded && !ErrorWindow.CriticalError && Email.Count(new Filter(false)) != 0;
        }

        private void Emails_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Текстовый файл (*.txt)|*.txt",
                FileName = "DealerBase.txt"
            };
            if ((bool)saveFileDialog.ShowDialog())
            {
                Filter filter = new Filter(false);
                if (Email.Count(filter) != 0)
                {
                    File.WriteAllText(saveFileDialog.FileName, String.Join(", ", Email.Select(filter).Select(x => x.Field<string>("Value"))));
                }
                else
                {
                    ShowErrorWindow(6);
                }
            }
        }

        private void Print_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded && !ErrorWindow.CriticalError && Dealer.Count(new Filter(false)) != 0;
        }

        private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new PrintingWindow().ShowDialog(this);
        }

        private void Notify_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !ErrorWindow.CriticalError && Event.Count(DateTime.Now) != 0;
        }

        private void Notify_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new NotificationWindow().ShowDialog(this);
        }

        private void Constants_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new ConstantsWindow().ShowDialog(this);
            UpdateDealers();
            Dealers.SelectItem();
        }

        private void Update_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UpdateDealers();
            Dealers.SelectItem();
        }
    }
}