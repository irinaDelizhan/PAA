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
    /// Interaction logic for ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Page
    {
        List<Page> framesList;
        private bool isInitialized = false;
        public string? role = null;

        SearchFrame searchFrame = new SearchFrame("project");
        DateFrame dateFrame = new DateFrame("search");

        List<State> filteredStates;
        Project existingProject;

        StateReport stateReport;
        TimeReport timeReport;
        public ReportsPage(string role)
        {
            InitializeComponent();
            this.role = role;

            framesList = new List<Page>();

            framesList.Add(searchFrame);
            frameProject.Content = framesList[0];

            framesList.Add(dateFrame);
            frameDate.Content = framesList[1];

            frameDate.IsEnabled = false;
            buttonDownloadReport.IsEnabled = false;

            if (role == "headProject")
            {
                ProjectsPage.UpdateComboBoxItemsVisibility(comboBoxReportType, item =>
                    item.Content.ToString() != "Time spent on each status of the project");
            }

            isInitialized = true;
        }

        private void comboBoxReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                ClearReportFields();

                if (comboBoxReportType.SelectedIndex == 0 || 
                    comboBoxReportType.SelectedIndex == 3)
                {
                    frameProject.IsEnabled = true;
                    frameDate.IsEnabled = false;

                    buttonDownloadReport.IsEnabled = false;

                    Time.Visibility = Visibility.Collapsed;

                    if (comboBoxReportType.SelectedIndex == 3)
                    {
                        Time.Visibility = Visibility.Visible;
                    }
                }
                else if (comboBoxReportType.SelectedIndex == 1)
                {
                    EnableForReportDetails(true);
                    Time.Visibility = Visibility.Collapsed;
                    buttonDownloadReport.IsEnabled = false;
                }
                else if (comboBoxReportType.SelectedIndex == 2)
                {
                    EnableForReportDetails(false);
                    Time.Visibility = Visibility.Collapsed;
                    buttonDownloadReport.IsEnabled = false;
                }
            }
        }
        private void EnableForReportDetails(bool enable)
        {
            frameProject.IsEnabled = enable;
            frameDate.IsEnabled = enable;
        }

        private void buttonGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (!Helper.IsValidInput(searchFrame.textBoxSearch.Text))
            {
                Helper.ShowError("The entered text contains prohibited SQL statements.");
                return;
            }

            stateReport = new(Storage.Instance.states);

            if (comboBoxReportType.SelectedIndex == 0 ||
                comboBoxReportType.SelectedIndex == 1 ||
                comboBoxReportType.SelectedIndex == 3)
            {
                if (!string.IsNullOrWhiteSpace(searchFrame.textBoxSearch.Text))
                {
                    string input = searchFrame.textBoxSearch.Text;
                    Project? project = Helper.ParseToObject(input, typeof(Project)) as Project;
                    if (project != null && Project.isCorrectValues == 1)
                    {
                        existingProject = Storage.Instance.projects.FirstOrDefault(p => p.Id == project.Id && p.Name == project.Name);
                        Project.isCorrectValues = 0;
                    }

                    if (existingProject == null)
                    {
                        Helper.ShowError("No such project exists.");
                        return;
                    }

                    // project statuses
                    if (comboBoxReportType.SelectedIndex == 0)
                    {
                        filteredStates = stateReport.GenerateReport(stateReport.States, 0, existingProject.Id);
                    }

                    // time spent on each status
                    else if (comboBoxReportType.SelectedIndex == 3)
                    {
                        timeReport = new(Storage.Instance.states);
                        filteredStates = timeReport.GenerateReport(timeReport.States, 3, existingProject.Id);
                    }

                    // according to dates
                    else if (comboBoxReportType.SelectedIndex == 1)
                    {
                        if (dateFrame.startDate.SelectedDate != null)
                        {
                            if (dateFrame.endDate.SelectedDate != null)
                            {
                                filteredStates = stateReport.GenerateReport(stateReport.States, 1, existingProject.Id, dateFrame.startDate.SelectedDate, dateFrame.endDate.SelectedDate);
                            }
                            else
                            {
                                filteredStates = stateReport.GenerateReport(stateReport.States, 1, existingProject.Id, dateFrame.startDate.SelectedDate);
                            }
                        }
                        else
                            Helper.ShowError("Select a start date.");
                    }
                }
                else
                    Helper.ShowError("Select a project.");
            }

            // the latest statuses
            else if (comboBoxReportType.SelectedIndex == 2)
            {
                filteredStates = stateReport.GenerateReport(stateReport.States, 2);
            }

            if (filteredStates != null && filteredStates.Count == 0)
            {
                Helper.ShowMessage("No data found.");
                buttonDownloadReport.IsEnabled = false;
            }

            dataGridReports.ItemsSource = null;
            dataGridReports.ItemsSource = filteredStates;

            if (dataGridReports.ItemsSource != null && filteredStates.Count != 0)
                buttonDownloadReport.IsEnabled = true;
            else buttonDownloadReport.IsEnabled = false;
        }

        private void buttonDownloadReport_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxReportType.SelectedIndex == 0)
            {
                stateReport.DownloadReport(dataGridReports, 0);
            }
            else if (comboBoxReportType.SelectedIndex == 1)
            {
                stateReport.DownloadReport(dataGridReports, 1);
            }
            else if (comboBoxReportType.SelectedIndex == 2)
            {
                stateReport.DownloadReport(dataGridReports, 2);
            }
            else if (comboBoxReportType.SelectedIndex == 3)
            {
                timeReport.DownloadReport(dataGridReports, 3);
            }
        }
        private void ClearReportFields()
        {
            dateFrame.startDate.SelectedDate = null;
            dateFrame.endDate.SelectedDate = null;
            searchFrame.textBoxSearch.Clear();
        }
    }
}
