using System.Windows;
using System.Windows.Controls;

namespace DealerBase.Styles
{
    public class ListBoxStyleSelector : StyleSelector
    {
        public Style SeparatorStyle { get; set; }
        public Style TextBoxStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            return item is Separator ? SeparatorStyle : TextBoxStyle;
        }
    }
}