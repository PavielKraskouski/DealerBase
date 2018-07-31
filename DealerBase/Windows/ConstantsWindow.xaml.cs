using DealerBase.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DealerBase.Windows
{
    /// <summary>
    /// Логика взаимодействия для ConstantsWindow.xaml
    /// </summary>
    public partial class ConstantsWindow : Window
    {
        private bool ValueExists()
        {
            switch (Constant.SelectedIndex)
            {
                case 0:
                    return BusinessEntity.Exists((long)(Values.SelectedItem as TextBlock).Tag);
                case 1:
                    return Activity.Exists((long)(Values.SelectedItem as TextBlock).Tag);
                default:
                    return ActivityDirection.Exists((long)(Values.SelectedItem as TextBlock).Tag);
            }
        }

        private bool DealerExists()
        {
            switch (Constant.SelectedIndex)
            {
                case 0:
                    return Dealer.Count("BusinessEntity", (long)(Values.SelectedItem as TextBlock).Tag) != 0;
                case 1:
                    return Dealer.Count("Activity", (long)(Values.SelectedItem as TextBlock).Tag) != 0;
                default:
                    return Dealer.Count("ActivityDirection", (long)(Values.SelectedItem as TextBlock).Tag) != 0;
            }
        }

        private void UpdateValues()
        {
            Values.Items.Clear();
            switch (Constant.SelectedIndex)
            {
                case 0:
                    BusinessEntity.Select().ForEach(x => Values.Items.Add(BusinessEntity.ToTextBlock(x)));
                    break;
                case 1:
                    Activity.Select().ForEach(x => Values.Items.Add(Activity.ToTextBlock(x)));
                    break;
                case 2:
                    ActivityDirection.Select().ForEach(x => Values.Items.Add(ActivityDirection.ToTextBlock(x)));
                    break;
            }
        }

        private void ShowErrorWindow(byte errorCode, bool updateValues = true)
        {
            new ErrorWindow(errorCode).ShowDialog(this);
            if (updateValues)
            {
                UpdateValues();
                Values.SelectItem();
            }
        }

        public ConstantsWindow()
        {
            InitializeComponent();
            this.FixLayout();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateValues();
            Values.SelectItem();
        }

        private void Constant_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Constant.IsLoaded)
            {
                UpdateValues();
                Values.SelectItem();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ConstantWindow constantWindow = new ConstantWindow();
            if ((bool)constantWindow.ShowDialog(this))
            {
                long insertedValueId;
                switch (Constant.SelectedIndex)
                {
                    case 0:
                        insertedValueId = BusinessEntity.Insert(constantWindow.Value);
                        break;
                    case 1:
                        insertedValueId = Activity.Insert(constantWindow.Value);
                        break;
                    default:
                        insertedValueId = ActivityDirection.Insert(constantWindow.Value);
                        break;
                }
                UpdateValues();
                Values.SelectItem(Values.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == insertedValueId));
            }
        }

        private void Edit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Values.SelectedItem != null;
        }

        private void Edit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ValueExists())
            {
                ConstantWindow constantWindow = new ConstantWindow((Values.SelectedItem as TextBlock).Text);
                if ((bool)constantWindow.ShowDialog(this))
                {
                    if (ValueExists())
                    {
                        long selectedValueId = (long)(Values.SelectedItem as TextBlock).Tag;
                        switch (Constant.SelectedIndex)
                        {
                            case 0:
                                BusinessEntity.Update(selectedValueId, constantWindow.Value);
                                break;
                            case 1:
                                Activity.Update(selectedValueId, constantWindow.Value);
                                break;
                            case 2:
                                ActivityDirection.Update(selectedValueId, constantWindow.Value);
                                break;
                        }
                        UpdateValues();
                        Values.SelectItem(Values.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == selectedValueId));
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

        private void Value_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Edit.Command.Execute(null);
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Values.SelectedItem != null && !DealerExists();
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ValueExists())
            {
                if ((bool)new ConfirmationWindow().ShowDialog(this))
                {
                    if (ValueExists())
                    {
                        if (!DealerExists())
                        {
                            switch (Constant.SelectedIndex)
                            {
                                case 0:
                                    BusinessEntity.Delete((long)(Values.SelectedItem as TextBlock).Tag);
                                    break;
                                case 1:
                                    Activity.Delete((long)(Values.SelectedItem as TextBlock).Tag);
                                    break;
                                case 2:
                                    ActivityDirection.Delete((long)(Values.SelectedItem as TextBlock).Tag);
                                    break;
                            }
                            UpdateValues();
                            Values.SelectItem();
                        }
                        else
                        {
                            ShowErrorWindow(4, false);
                        }
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
    }
}