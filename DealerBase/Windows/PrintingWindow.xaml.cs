using DealerBase.Entities;
using System;
using System.Data;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DealerBase.Windows
{
    /// <summary>
    /// Логика взаимодействия для PrintingWindow.xaml
    /// </summary>
    public partial class PrintingWindow : Window
    {
        private FlowDocument GenerateFlowDocument()
        {
            Table table = new Table()
            {
                FontFamily = new FontFamily("Times New Roman"),
                FontSize = 12,
                Foreground = Brushes.Black,
                TextAlignment = TextAlignment.Center
            };
            Style tableCellStyle = new Style(typeof(TableCell));
            tableCellStyle.Setters.Add(new Setter(TableCell.BorderBrushProperty, Brushes.Black));
            tableCellStyle.Setters.Add(new Setter(TableCell.BorderThicknessProperty, new Thickness(1)));
            tableCellStyle.Setters.Add(new Setter(TableCell.PaddingProperty, new Thickness(2)));
            Dealer.Select(new Filter(true)).ForEach(x =>
            {
                Dealer dealer = Dealer.FromDataRow(x);
                Contact contact = dealer.Contacts.FirstOrDefault();
                Phone phone = contact?.Phones.FirstOrDefault();
                TableCell nameCell = new TableCell(new Paragraph(new Run($"{BusinessEntity.Select(dealer.BusinessEntityId).Field<string>("Name")} \"{dealer.Name}\""))) { Style = tableCellStyle };
                TableCell regionCell = new TableCell(new Paragraph(new Run(Region.Select(dealer.RegionId).Field<string>("Name")))) { Style = tableCellStyle };
                TableCell activityCell = new TableCell(new Paragraph(new Run(Activity.Select(dealer.ActivityId).Field<string>("Name")))) { Style = tableCellStyle };
                TableCell activityDirectionCell = new TableCell(new Paragraph(new Run(ActivityDirection.Select(dealer.ActivityDirectionId).Field<string>("Name")))) { Style = tableCellStyle };
                TableCell ratingCell = new TableCell(new Paragraph(new Run(new string('★', (int)dealer.Rating)))) { Style = tableCellStyle };
                TableCell contactCell = new TableCell(new Paragraph(new Run(contact == null ? "–" : $"{contact.Surname} {contact.Name} {contact.Patronymic}".Trim()))) { Style = tableCellStyle };
                TableCell positionCell = new TableCell(new Paragraph(new Run(contact == null || contact.Position == string.Empty ? "–" : contact.Position))) { Style = tableCellStyle };
                TableCell phoneCell = new TableCell(new Paragraph(new Run(phone == null ? "–" : phone.Value))) { Style = tableCellStyle };
                TableRow tableRow = new TableRow();
                tableRow.Cells.Add(nameCell);
                tableRow.Cells.Add(regionCell);
                tableRow.Cells.Add(activityCell);
                tableRow.Cells.Add(activityDirectionCell);
                tableRow.Cells.Add(ratingCell);
                tableRow.Cells.Add(contactCell);
                tableRow.Cells.Add(positionCell);
                tableRow.Cells.Add(phoneCell);
                TableRowGroup tableRowGroup = new TableRowGroup();
                tableRowGroup.Rows.Add(tableRow);
                table.RowGroups.Add(tableRowGroup);
            });
            PrintDialog printDialog = new PrintDialog();
            return new FlowDocument(table)
            {
                Background = Brushes.White,
                PageWidth = printDialog.PrintableAreaHeight,
                PageHeight = printDialog.PrintableAreaWidth,
                ColumnWidth = printDialog.PrintableAreaHeight
            };
        }

        public PrintingWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Preview.Document = GenerateFlowDocument();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (Dealer.Count(new Filter(false)) != 0)
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
                if ((bool)printDialog.ShowDialog())
                {
                    if (Dealer.Count(new Filter(false)) != 0)
                    {
                        printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
                        printDialog.PrintDocument((GenerateFlowDocument() as IDocumentPaginatorSource).DocumentPaginator, "DealerBase");
                    }
                    else
                    {
                        new ErrorWindow(5).ShowDialog(this);
                    }
                }
            }
            else
            {
                new ErrorWindow(5).ShowDialog(this);
            }
        }
    }
}