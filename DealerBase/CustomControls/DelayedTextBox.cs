using DealerBase.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DealerBase.CustomControls
{
    public class DelayedTextBox : TextBox
    {
        private DispatcherTimer Timer { get; set; }

        public DelayedTextBox() : base()
        {
            Timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            Timer.Tick += new EventHandler((x, y) =>
            {
                Timer.Stop();
                MainWindow.Instance.UpdateDealers();
                MainWindow.Instance.Dealers.SelectItem();
                CommandManager.InvalidateRequerySuggested();
            });
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            Timer.Stop();
            Timer.Start();
        }
    }
}