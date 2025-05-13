using PAA.Classes;
using PAA.Frames;
using PAA.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PAA.Pages
{
    /// <summary>
    /// Interaction logic for TransactionsPage.xaml
    /// </summary>
    public partial class TransactionsPage : Page
    {
        List<Page> framesList;
        private bool isInitialized = false;

        SearchFrame searchFrame = new SearchFrame("user");
        DateFrame dateFrame = new DateFrame("search");
        public TransactionsPage()
        {
            InitializeComponent();
            framesList = new List<Page>();

            framesList.Add(searchFrame);
            frameUser.Content = framesList[0];
            framesList.Add(dateFrame);
            frameDate.Content = framesList[1];

            EnableForTransactionDetails(false);

            dataGridTransactions.ItemsSource = Storage.Instance.transactions;

            isInitialized = true;
        }

        private void comboBoxSearchOperationOnTransactionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                ClearTransactionFields();

                if (comboBoxSearchOperationOnTransactionType.SelectedIndex == 0)
                {
                    EnableForTransactionDetails(false);
                }
                else
                {
                    EnableForTransactionDetails(true);
                }
            }
        }
        private void EnableForTransactionDetails(bool enable)
        {
            frameUser.IsEnabled = enable;
            comboBoxType.IsEnabled = enable;
            frameDate.IsEnabled = enable;
        }

        private void SearchOperationOnTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (!Helper.IsValidInput(searchFrame.textBoxSearch.Text))
            {
                Helper.ShowError("The entered text contains prohibited SQL statements.");
                return;
            }

            switch (comboBoxSearchOperationOnTransactionType.SelectedIndex)
            {
                case 0:
                    dataGridTransactions.ItemsSource = null;
                    dataGridTransactions.ItemsSource = Storage.Instance.transactions;
                    break;
                case 1:
                    var filteredTransaction = Storage.Instance.transactions.AsEnumerable();

                    if (string.IsNullOrWhiteSpace(searchFrame.textBoxSearch.Text) &&
                        !dateFrame.startDate.SelectedDate.HasValue &&
                        !dateFrame.endDate.SelectedDate.HasValue)
                    {
                        Helper.ShowError("Select search parameters.");
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(searchFrame.textBoxSearch.Text))
                    {
                        string[] parts = searchFrame.textBoxSearch.Text.Split(' ');

                        if (parts.Length > 0 && int.TryParse(parts[0], out int searchId))
                        {
                            filteredTransaction = filteredTransaction.Where(t => t.user.Id == searchId);
                        }
                        else
                        {
                            Helper.ShowError("The user is not selected correctly.");
                            return;
                        }
                    }

                    if (comboBoxType.SelectedIndex == 1)
                        filteredTransaction = filteredTransaction.Where(t => t.Type == Enums._Type.addition);
                    else if (comboBoxType.SelectedIndex == 2)
                        filteredTransaction = filteredTransaction.Where(t => t.Type == Enums._Type.editing);
                    else if (comboBoxType.SelectedIndex == 3)
                        filteredTransaction = filteredTransaction.Where(t => t.Type == Enums._Type.removal);

                    if (dateFrame.startDate.SelectedDate.HasValue)
                    {
                        DateTime startDate = dateFrame.startDate.SelectedDate.Value.Date;

                        filteredTransaction = filteredTransaction.Where(t =>
                            t.Date >= startDate);
                    }
                    if (dateFrame.endDate.SelectedDate.HasValue)
                    {
                        DateTime endDate = dateFrame.endDate.SelectedDate.Value.Date;

                        filteredTransaction = filteredTransaction.Where(t =>
                            t.Date.GetValueOrDefault().Date <= endDate);

                        foreach (var item in filteredTransaction)
                        {
                            if (item.Date != null && item.Date.GetValueOrDefault().Date > endDate)
                                filteredTransaction = filteredTransaction.Where(t =>
                                    t.Id != item.Id);
                        }
                    }

                    if (filteredTransaction == null || filteredTransaction.ToList().Count == 0)
                        Helper.ShowMessage("No transactions found.");

                    dataGridTransactions.ItemsSource = null;
                    dataGridTransactions.ItemsSource = filteredTransaction.ToList();
                    break;
            }
        }
        private void ClearTransactionFields()
        {
            searchFrame.textBoxSearch.Clear();
            comboBoxType.SelectedIndex = 0;
            dateFrame.startDate.SelectedDate = null;
            dateFrame.endDate.SelectedDate = null;
        }

        private void buttonUpdateTransactionDataGrid_Click(object sender, RoutedEventArgs e)
        {
            dataGridTransactions.ItemsSource = Storage.Instance.transactions;
            Helper.ShowMessage("Transaction table has been updated.");
        }
    }
}
