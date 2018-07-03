using DealerBase.Entities;
using System;
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

        public ConstantsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateValues();
            Values.SelectedIndex = 0;
        }

        private void Constant_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Constant.IsLoaded)
            {
                UpdateValues();
                Values.SelectedIndex = 0;
                Values.ScrollIntoView(Values.SelectedItem);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ConstantWindow constantWindow = new ConstantWindow()
            {
                Owner = this
            };
            if ((bool)constantWindow.ShowDialog())
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
                Values.SelectedItem = Values.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == insertedValueId);
                Values.ScrollIntoView(Values.SelectedItem);
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
                ConstantWindow constantWindow = new ConstantWindow()
                {
                    Owner = this,
                    Value = (Values.SelectedItem as TextBlock).Text
                };
                if ((bool)constantWindow.ShowDialog())
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
                        Values.SelectedItem = Values.Items.FirstOrDefault<TextBlock>(x => (long)x.Tag == selectedValueId);
                        Values.ScrollIntoView(Values.SelectedItem);
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
                ConfirmationWindow confirmationWindow = new ConfirmationWindow()
                {
                    Owner = this
                };
                if ((bool)confirmationWindow.ShowDialog())
                {
                    if (ValueExists() && !DealerExists())
                    {
                        int selectedIndex = Values.SelectedIndex;
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
                        Values.SelectedIndex = Math.Max(0, Math.Min(Values.Items.Count - 1, selectedIndex - 1));
                        Values.ScrollIntoView(Values.SelectedItem);
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
    }
}