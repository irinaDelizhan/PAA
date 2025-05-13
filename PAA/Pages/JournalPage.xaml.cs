using PAA.Classes;
using PAA.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    /// Interaction logic for JournalPage.xaml
    /// </summary>
    public partial class JournalPage : Page
    {
        SearchFrame searchFrame;
        DateFrame dateFrame;
        private bool isInitialized = false;
        public JournalPage()
        {
            InitializeComponent();

            searchFrame = new SearchFrame("user");
            frameUser.Content = searchFrame;

            dateFrame = new DateFrame("search");
            frameDate.Content = dateFrame;

            EnableForJournalDetails(false);

            dataGridJournal.ItemsSource = Storage.Instance.users;

            isInitialized = true;
        }

        private void EnableForJournalDetails(bool enable)
        {
            frameDate.IsEnabled = enable;
            frameUser.IsEnabled = enable;
        }

        private void buttonPerformOperationOnJournal_Click(object sender, RoutedEventArgs e)
        {
            if (!Helper.IsValidInput(searchFrame.textBoxSearch.Text))
            {
                Helper.ShowError("The entered text contains prohibited SQL statements.");
                return;
            }

            switch (comboBoxSearchOperationOnJournal.SelectedIndex)
            {
                case 0:
                    dataGridJournal.ItemsSource = null;
                    dataGridJournal.ItemsSource = Storage.Instance.users;
                    break;
                case 1:
                    var filteredUsers = Storage.Instance.users.AsEnumerable();

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
                            filteredUsers = filteredUsers.Where(user => user.Id == searchId);
                        }
                        else
                        {
                            Helper.ShowError("The user is not selected correctly.");
                            return;
                        }
                    }

                    if (dateFrame.startDate.SelectedDate.HasValue)
                    {
                        DateTime startDate = dateFrame.startDate.SelectedDate.Value.Date;

                        filteredUsers = filteredUsers.Where(user =>
                            user.StartDate >= startDate);

                    }
                    if (dateFrame.endDate.SelectedDate.HasValue)
                    {
                        DateTime endDate = dateFrame.endDate.SelectedDate.Value.Date;
                        
                        filteredUsers = filteredUsers.Where(user =>
                            user.StartDate.GetValueOrDefault().Date <= endDate);

                        foreach (var item in filteredUsers)
                        {
                            if (item.EndDate != null && item.EndDate.GetValueOrDefault().Date > endDate)
                                filteredUsers = filteredUsers.Where(user =>
                                    user.Id != item.Id);
                        }
                    }

                    if (filteredUsers.ToList().Count == 0)
                        Helper.ShowMessage("No users found.");

                    dataGridJournal.ItemsSource = null;
                    dataGridJournal.ItemsSource = filteredUsers.ToList();
                    break;
            }
        }

        private void comboBoxSearchOperationOnJournal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                searchFrame.textBoxSearch.Clear();
                dateFrame.startDate.SelectedDate = null;
                dateFrame.endDate.SelectedDate = null;

                if (comboBoxSearchOperationOnJournal.SelectedIndex == 0)
                {
                    EnableForJournalDetails(false);
                }
                else
                {
                    EnableForJournalDetails(true);
                }
            }
        }
    }
}
