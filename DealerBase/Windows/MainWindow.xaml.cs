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

        public void Update()
        {
            long selectedRegionId = (long)(Region.SelectedItem as TextBlock).Tag;
            long selectedActivityId = (long)(Activity.SelectedItem as TextBlock).Tag;
            long selectedActivityDirectionId = (long)(ActivityDirection.SelectedItem as TextBlock).Tag;
            bool regionExists() => selectedRegionId == 0 || Entities.Region.Exists(selectedRegionId);
            bool activityExists() => selectedActivityId == 0 || Entities.Activity.Exists(selectedActivityId);
            bool activityDirectionExists() => selectedActivityDirectionId == 0 || Entities.ActivityDirection.Exists(selectedActivityDirectionId);
            Region.Items.RemoveRange(1, Region.Items.Count - 1);
            Entities.Region.Select().ForEach(x => Region.Items.Add(Entities.Region.ToTextBlock(x)));
            Region.SelectedIndex = regionExists() ? Region.Items.IndexOf(Region.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == selectedRegionId)) : 0;
            Activity.Items.RemoveRange(1, Activity.Items.Count - 1);
            Entities.Activity.Select().ForEach(x => Activity.Items.Add(Entities.Activity.ToTextBlock(x)));
            Activity.SelectedIndex = activityExists() ? Activity.Items.IndexOf(Activity.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == selectedActivityId)) : 0;
            ActivityDirection.Items.RemoveRange(1, ActivityDirection.Items.Count - 1);
            Entities.ActivityDirection.Select().ForEach(x => ActivityDirection.Items.Add(Entities.ActivityDirection.ToTextBlock(x)));
            ActivityDirection.SelectedIndex = activityDirectionExists() ? ActivityDirection.Items.IndexOf(ActivityDirection.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == selectedActivityDirectionId)) : 0;
            Region.Tag = (Region.SelectedItem as TextBlock).Tag;
            Activity.Tag = (Activity.SelectedItem as TextBlock).Tag;
            ActivityDirection.Tag = (ActivityDirection.SelectedItem as TextBlock).Tag;
            Relevance.Tag = (long)Relevance.SelectedIndex;
            Sort.Tag = (long)Sort.SelectedIndex;
            long selectedDealerId = Dealers.SelectedItem != null ? (long)(Dealers.SelectedItem as TextBlock).Tag : 0;
            Dealers.Items.Clear();
            Dealer.Select(new Filter(true)).ForEach(x => Dealers.Items.Add(Dealer.ToTextBlock(x)));
            TextBlock selectedDealer = Dealers.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == selectedDealerId);
            Dealers.SelectedIndex = selectedDealer != null ? Dealers.Items.IndexOf(selectedDealer) : 0;
            Dealers.ScrollIntoView(Dealers.SelectedItem);
        }

        public MainWindow()
        {
            DBAccess.CreateDatabase();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
            Update();
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
                Update();
            }
        }

        private void Add_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = BusinessEntity.Count() != 0 && Entities.Activity.Count() != 0 && Entities.ActivityDirection.Count() != 0;
        }

        private void Add_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DealerWindow dealerWindow = new DealerWindow()
            {
                Owner = this
            };
            if ((bool)dealerWindow.ShowDialog())
            {
                if (BusinessEntity.Exists(dealerWindow.Dealer.BusinessEntityId) && Entities.Activity.Exists(dealerWindow.Dealer.ActivityId) && Entities.ActivityDirection.Exists(dealerWindow.Dealer.ActivityDirectionId))
                {
                    long insertedDealerId = dealerWindow.Dealer.Insert();
                    Update();
                    TextBlock insertedDealer = Dealers.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == insertedDealerId);
                    Dealers.SelectedIndex = insertedDealer != null ? Dealers.Items.IndexOf(insertedDealer) : Dealers.SelectedIndex;
                    Dealers.ScrollIntoView(Dealers.SelectedItem);
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

        private void Edit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded && Dealers.SelectedItem != null;
        }

        private void Edit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Dealer.Exists((long)(Dealers.SelectedItem as TextBlock).Tag))
            {
                DealerWindow dealerWindow = new DealerWindow()
                {
                    Owner = this,
                    Dealer = Dealer.FromDataRow(Dealer.Select((long)(Dealers.SelectedItem as TextBlock).Tag))
                };
                if ((bool)dealerWindow.ShowDialog())
                {
                    if (Dealer.Exists((long)(Dealers.SelectedItem as TextBlock).Tag) &&
                        BusinessEntity.Exists(dealerWindow.Dealer.BusinessEntityId) &&
                        Entities.Activity.Exists(dealerWindow.Dealer.ActivityId) &&
                        Entities.ActivityDirection.Exists(dealerWindow.Dealer.ActivityDirectionId))
                    {
                        dealerWindow.Dealer.Update();
                        Update();
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
                ConfirmationWindow confirmationWindow = new ConfirmationWindow()
                {
                    Owner = this
                };
                if ((bool)confirmationWindow.ShowDialog())
                {
                    if (Dealer.Exists((long)(Dealers.SelectedItem as TextBlock).Tag))
                    {
                        int selectedIndex = Dealers.SelectedIndex;
                        Dealer.Delete((long)(Dealers.SelectedItem as TextBlock).Tag);
                        Update();
                        Dealers.SelectedIndex = Math.Max(0, Math.Min(Dealers.Items.Count - 1, selectedIndex - 1));
                        Dealers.ScrollIntoView(Dealers.SelectedItem);
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
            e.CanExecute = IsLoaded && Email.Count(new Filter(false)) != 0;
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
                    ErrorWindow errorWindow = new ErrorWindow()
                    {
                        Owner = this
                    };
                    errorWindow.ShowDialog();
                }
            }
        }

        private void Print_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsLoaded && Dealer.Count(new Filter(false)) != 0;
        }

        private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PrintingWindow printingWindow = new PrintingWindow()
            {
                Owner = this
            };
            printingWindow.ShowDialog();
        }

        private void Notify_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Event.Count(DateTime.Now) != 0;
        }

        private void Notify_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NotificationWindow notificationWindow = new NotificationWindow()
            {
                Owner = this
            };
            notificationWindow.ShowDialog();
        }

        private void Constants_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ConstantsWindow constantsWindow = new ConstantsWindow()
            {
                Owner = this
            };
            constantsWindow.ShowDialog();
            Update();
        }

        private void Update_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Update();
        }
    }
}