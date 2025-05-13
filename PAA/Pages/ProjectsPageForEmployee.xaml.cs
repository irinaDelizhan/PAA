using PAA.Classes;
using PAA.Enums;
using PAA.Frames;
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
    /// Interaction logic for ProjectsPageForEmployee.xaml
    /// </summary>
    public partial class ProjectsPageForEmployee : Page
    {
        SearchFrame searchFrame;
        DateFrame dateFrame;
        private bool isInitialized = false;
        public ProjectsPageForEmployee()
        {
            InitializeComponent();

            searchFrame = new SearchFrame("project");
            frameProject.Content = searchFrame;

            dateFrame = new DateFrame("search");
            frameDate.Content = dateFrame;

            EnableForProjectsDetails(false);

            dataGridOpenProjects.ItemsSource = Storage.Instance.projects
                    .Where(project => project.ExecutionStatus == ExecutionStatus.open)
                    .ToList();
            dataGridClosedProjects.ItemsSource = Storage.Instance.projects
                    .Where(project => project.ExecutionStatus == ExecutionStatus.closed)
                    .ToList();

            isInitialized = true;
        }
        private void comboBoxSearchOperationOnProjectsForEmployeeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                searchFrame.textBoxSearch.Clear();
                dateFrame.startDate.SelectedDate = null;
                dateFrame.endDate.SelectedDate = null;

                if (comboBoxSearchOperationOnProjectsForEmployeeType.SelectedIndex == 0)
                {
                    EnableForProjectsDetails(false);
                }
                else
                {
                    EnableForProjectsDetails(true);
                }
            }
        }
        private void EnableForProjectsDetails(bool enable)
        {
            frameDate.IsEnabled = enable;
            frameProject.IsEnabled = enable;
        }
        private void buttonSearchOperationOnProjectsForEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (!Helper.IsValidInput(searchFrame.textBoxSearch.Text))
            {
                Helper.ShowError("The entered text contains prohibited SQL statements.");
                return;
            }

            switch (comboBoxSearchOperationOnProjectsForEmployeeType.SelectedIndex)
            {
                case 0:
                    dataGridOpenProjects.ItemsSource = null;
                    dataGridClosedProjects.ItemsSource = null;

                    dataGridOpenProjects.ItemsSource = Storage.Instance.projects
                            .Where(project => project.ExecutionStatus == ExecutionStatus.open)
                            .ToList();
                    dataGridClosedProjects.ItemsSource = Storage.Instance.projects
                            .Where(project => project.ExecutionStatus == ExecutionStatus.closed)
                            .ToList();
                    break;
                case 1:
                    var filteredProjects = Storage.Instance.projects.AsEnumerable();

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
                            filteredProjects = filteredProjects.Where(p => p.Id == searchId);
                        }
                        else
                        {
                            Helper.ShowError("The project is not selected correctly.");
                            return;
                        }
                    }

                    if (dateFrame.startDate.SelectedDate.HasValue)
                    {
                        DateTime startDate = dateFrame.startDate.SelectedDate.Value.Date;

                        filteredProjects = filteredProjects.Where(p =>
                            p.StartDate >= startDate);

                    }
                    if (dateFrame.endDate.SelectedDate.HasValue)
                    {
                        DateTime endDate = dateFrame.endDate.SelectedDate.Value.Date;

                        filteredProjects = filteredProjects.Where(p =>
                            p.StartDate.GetValueOrDefault().Date <= endDate);

                        foreach (var item in filteredProjects)
                        {
                            if (item.ExpectedEndDate != null && item.ExpectedEndDate.GetValueOrDefault().Date > endDate)
                                filteredProjects = filteredProjects.Where(p =>
                                    p.Id != item.Id);
                        }

                        foreach (var item in filteredProjects)
                        {
                            if (item.ActualEndDate != null && item.ActualEndDate.GetValueOrDefault().Date > endDate)
                                filteredProjects = filteredProjects.Where(p =>
                                    p.Id != item.Id);
                        }
                    }

                    if (filteredProjects.ToList().Count == 0)
                        Helper.ShowMessage("No projects found.");

                    dataGridOpenProjects.ItemsSource = null;
                    dataGridClosedProjects.ItemsSource = null;

                    dataGridOpenProjects.ItemsSource = filteredProjects
                            .Where(project => project.ExecutionStatus == ExecutionStatus.open)
                            .ToList();
                    dataGridClosedProjects.ItemsSource = filteredProjects
                            .Where(project => project.ExecutionStatus == ExecutionStatus.closed)
                            .ToList();
                    break;
            }
        }
    }
}
