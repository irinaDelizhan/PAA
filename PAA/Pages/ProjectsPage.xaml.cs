using PAA.Classes;
using PAA.Enums;
using PAA.Frames;
using PAA.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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
    /// Interaction logic for ProjectsPage.xaml
    /// </summary>
    public partial class ProjectsPage : Page
    {
        List<Page> framesList;
        public static string? role = null;
        User? authorizedUser;
        public string? page = null;
        private bool isInitialized = false;

        SearchFrame searchHeadProjectFrame;
        SearchFrame searchProjectFrame;

        User existingHeadProject;
        public ProjectsPage(string? transmittedPage, User? authorizedUser)
        {
            InitializeComponent();
            this.authorizedUser = authorizedUser;

            framesList = new List<Page>();

            searchProjectFrame = new SearchFrame("project");
            framesList.Add(searchProjectFrame);
            frameProject.Content = framesList[0];
            searchProjectFrame.ProjectSelected += OnProjectSelected;
            
            searchHeadProjectFrame = new SearchFrame("user");
            framesList.Add(searchHeadProjectFrame);
            frameHeadProject.Content = framesList[1];


            page = transmittedPage;
            UpdateDataGridProjects();

            if (page == "openProjects")
            {
                UpdateComboBoxItemsVisibility(comboBoxProjectOperationType, item => true);
                UpdateComboBoxItemsVisibility(comboBoxExecutionStatusType, item =>
                    item.Content.ToString() == "Open");

                frameProject.IsEnabled = false;
                EnableForDates(false);
                startDate.IsEnabled = true;
            }
            else if (page == "closedProjects")
            {
                UpdateComboBoxItemsVisibility(comboBoxProjectOperationType, item =>
                    item.Content.ToString() == "Editing the project" ||
                    item.Content.ToString() == "Search for projects by head of the project");
                UpdateComboBoxItemsVisibility(comboBoxExecutionStatusType, item => true);
            }

            if (page == "openProjects" && role == "headProject")
            {
                UpdateComboBoxItemsVisibility(comboBoxProjectOperationType, item =>
                    item.Content.ToString() != "Search for projects by head of the project");
            }
            else if (page == "closedProjects" && role == "headProject")
            {
                UpdateComboBoxItemsVisibility(comboBoxProjectOperationType, item =>
                    item.Content.ToString() == "Editing the project");
            }

            CheckNumberVisibleObjects(comboBoxProjectOperationType);
            CheckNumberVisibleObjects(comboBoxExecutionStatusType);

            isInitialized = true;
        }
        private void OnProjectSelected(Classes.Project project)
        {
            if (project != null)
            {
                searchHeadProjectFrame.enable = false;

                textBoxProjectName.Text = project.Name;
                startDate.SelectedDate = project.StartDate;
                expectedEndDate.SelectedDate = project.ExpectedEndDate;
                actualEndDate.SelectedDate = project.ActualEndDate;
                searchHeadProjectFrame.textBoxSearch.Text = project.headProject.Id + " " + project.headProject.FullName;
                
                if (project.ExecutionStatus == ExecutionStatus.open)
                    comboBoxExecutionStatusType.SelectedIndex = 0;
                else
                    comboBoxExecutionStatusType.SelectedIndex = 1;

                searchHeadProjectFrame.enable = true;

                dataGridProjects.SelectedItem = project;
            }
            else
            {
                ClearProjectFields();
                UpdateDataGridProjects();
            }
        }
        public void UpdateDataGridProjects()
        {
            if (page == "openProjects")
                dataGridProjects.ItemsSource = Storage.Instance.projects
                    .Where(project => project.ExecutionStatus == ExecutionStatus.open)
                    .ToList();
            else
                dataGridProjects.ItemsSource = Storage.Instance.projects
                    .Where(project => project.ExecutionStatus == ExecutionStatus.closed)
                    .ToList();
        }
        public static void UpdateComboBoxItemsVisibility(ComboBox comboBox, Func<ComboBoxItem, bool> visibilityCondition)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                item.Visibility = visibilityCondition(item) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public static void CheckNumberVisibleObjects(ComboBox comboBox)
        {
            // Перевірка кількості видимих елементів
            var visibleItems = comboBox.Items
                .Cast<ComboBoxItem>()
                .Where(item => item.Visibility == Visibility.Visible)
                .ToList();

            // Якщо є лише один видимий елемент, встановити його як вибраний
            if (visibleItems.Count >= 1)
            {
                comboBox.SelectedItem = visibleItems.First();
            }
        }
        private void comboBoxProjectOperationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                searchProjectFrame.textBoxSearch.Clear();
                ClearProjectFields();

                if (comboBoxProjectOperationType.SelectedIndex == 0)
                {
                    EnableForProjectDetails(true);

                    frameProject.IsEnabled = false;
                    
                    EnableForDates(false);
                    startDate.IsEnabled = true;

                    UpdateComboBoxItemsVisibility(comboBoxExecutionStatusType, item =>
                        item.Content.ToString() == "Open");
                    CheckNumberVisibleObjects(comboBoxProjectOperationType);
                    CheckNumberVisibleObjects(comboBoxExecutionStatusType);

                    if (authorizedUser.Role == Role.headProject)
                        searchHeadProjectFrame.IsEnabled = true;
                }
                else if (comboBoxProjectOperationType.SelectedIndex == 1)
                {
                    EnableForProjectDetails(true);
                    
                    frameProject.IsEnabled = true;

                    EnableForDates(true);

                    UpdateComboBoxItemsVisibility(comboBoxExecutionStatusType, item => true);

                    if (authorizedUser.Role == Role.headProject)
                        searchHeadProjectFrame.IsEnabled = false;
                }
                else if (comboBoxProjectOperationType.SelectedIndex == 2)
                {
                    EnableForProjectDetails(false);
                    
                    frameProject.IsEnabled = true;

                    EnableForDates(false);

                    UpdateComboBoxItemsVisibility(comboBoxExecutionStatusType, item =>
                        item.Content.ToString() == "Open");
                }
                else if (comboBoxProjectOperationType.SelectedIndex == 3)
                {
                    EnableForProjectDetails(false);

                    frameProject.IsEnabled = false;

                    EnableForDates(false);

                    frameHeadProject.IsEnabled = true;

                    UpdateComboBoxItemsVisibility(comboBoxExecutionStatusType, item =>
                        item.Content.ToString() == "Open");
                }
            }
        }
        private void EnableForProjectDetails(bool enable)
        {
            textBoxProjectName.IsEnabled = enable;
            comboBoxExecutionStatusType.IsEnabled = enable;
            frameHeadProject.IsEnabled = enable;
        }
        private void EnableForDates(bool enable)
        {
            startDate.IsEnabled = enable;
            expectedEndDate.IsEnabled = enable;
            actualEndDate.IsEnabled = enable;        
        }
        private void startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateFrame.UpdateEndDate(startDate, expectedEndDate);
        }

        private void buttonPerformOperationOnProject_Click(object sender, RoutedEventArgs e)
        {
            if (!Helper.IsValidInput(searchProjectFrame.textBoxSearch.Text) ||
                !Helper.IsValidInput(searchHeadProjectFrame.textBoxSearch.Text) ||
                !Helper.IsValidInput(textBoxProjectName.Text))
            {
                Helper.ShowError("The entered text contains prohibited SQL statements.");
                return;
            }

            if (comboBoxProjectOperationType.SelectedIndex == 3)
            {
                if (string.IsNullOrWhiteSpace(searchHeadProjectFrame.textBoxSearch.Text))
                {
                    Helper.ShowError("Select a head of the project.");
                    return;
                }

                string input = searchHeadProjectFrame.textBoxSearch.Text;
                User? headProject = Helper.ParseToObject(input, typeof(User)) as User;
                if (headProject != null && User.isCorrectValues == 3)
                {
                    existingHeadProject = Storage.Instance.users.FirstOrDefault(p => p.Id == headProject.Id && p.FullName == headProject.FullName);
                    User.isCorrectValues = 0;
                }

                if (existingHeadProject == null)
                {
                    Helper.ShowError("No such head project exists.");
                    return;
                }

                dataGridProjects.ItemsSource = null;

                if (page == "openProjects")
                    dataGridProjects.ItemsSource = Storage.Instance.projects
                        .Where(project => project.ExecutionStatus == ExecutionStatus.open &&
                        project.headProject == existingHeadProject)
                        .ToList();
                else
                    dataGridProjects.ItemsSource = Storage.Instance.projects
                        .Where(project => project.ExecutionStatus == ExecutionStatus.closed &&
                        project.headProject == existingHeadProject)
                        .ToList();

                if(!dataGridProjects.Items.Cast<object>().Any()) // перевіряє, чи колекція містить хоча б один елемент
                {
                    Helper.ShowMessage("No projects with such a head of the project were found.");
                }

                return;
            }

            if (!string.IsNullOrWhiteSpace(textBoxProjectName.Text) &&
                startDate.SelectedDate != null &&
                expectedEndDate.SelectedDate != null &&
                !string.IsNullOrWhiteSpace(searchHeadProjectFrame.textBoxSearch.Text))
            {
                Classes.Project.OnValidationError += Helper.ShowError;

                if(comboBoxProjectOperationType.SelectedIndex == 0)
                {
                    int newId = Storage.Instance.projects.Count > 0
                        ? Storage.Instance.projects.Last().Id + 1
                        : 0;

                    if (authorizedUser.Role == Role.headProject)
                    {
                        searchHeadProjectFrame.textBoxSearch.Text = authorizedUser.Id + " " + authorizedUser.FullName;
                    }

                    Classes.Project? project = GetProjectData();

                    if (project != null && Classes.Project.isCorrectValues == 1)
                    {
                        project.Id = newId;

                        Storage.Instance.projects.Add(project);

                        var user = Storage.Instance.context.UserPaas
                            .FirstOrDefault(p => p.UserId == project.headProject.Id);
                        Models.Project projectDB = new Models.Project
                        {
                            ProjectId = project.Id,
                            ProjectName = project.Name,
                            Head = user,
                            StartDate = project.StartDate ?? DateTime.Now,
                            ExpectedEndDate = project.ExpectedEndDate ?? DateTime.Now,
                            ExecutionStatus = project.ExecutionStatus.ToString()
                        };
                        Storage.Instance.context.Projects.Add(projectDB);
                        Storage.Instance.context.SaveChanges();

                        Helper.ShowMessage("There is a project added.");
                        Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.addition, $"{authorizedUser.FullName} added a project\n\"{project.Id}, {project.Name}, {project.HeadProjectData}, {project.ExecutionStatus}, {project.StartDate}, {project.ExpectedEndDate}, {project.ActualEndDate}\".", DateTime.Now, authorizedUser);
                        Storage.Instance.transactions.Add(transaction);

                        Helper.SaveTransactionDB(transaction);

                        ClearProjectFields();

                        dataGridProjects.ItemsSource = null;
                        UpdateDataGridProjects();
                    }

                    Classes.Project.isCorrectValues = 0;
                }
                else if (comboBoxProjectOperationType.SelectedIndex == 1 ||
                    comboBoxProjectOperationType.SelectedIndex == 2)
                {
                    if (!string.IsNullOrWhiteSpace(searchProjectFrame.textBoxSearch.Text))
                    {
                        string[] parts = searchProjectFrame.textBoxSearch.Text.Split(' ');

                        if (parts.Length > 0 &&
                            int.TryParse(parts[0], out int index) &&
                            index >= 0 &&
                            index <= Storage.Instance.projects.Last().Id)
                        {
                            var checkProject = Storage.Instance.projects.FirstOrDefault(p => p.Id == index);
                            if (checkProject != null)
                            {
                                if (page == "openProjects" && checkProject.ExecutionStatus == ExecutionStatus.closed)
                                {
                                    Classes.Project.OnValidationError -= Helper.ShowError;
                                    Helper.ShowError("Select a project that is not closed.");
                                    return;
                                }
                                else if (page == "closedProjects" && checkProject.ExecutionStatus == ExecutionStatus.open)
                                {
                                    Classes.Project.OnValidationError -= Helper.ShowError;
                                    Helper.ShowError("Select a project that is not open.");
                                    return;
                                }

                                if (checkProject.headProject.Id != authorizedUser.Id && authorizedUser.Role == Role.headProject)
                                {
                                    Classes.Project.OnValidationError -= Helper.ShowError;
                                    Helper.ShowError("You can only edit and delete projects where you are the head.");
                                    return;
                                }
                            }

                            var projectDB = Storage.Instance.context.Projects
                                .FirstOrDefault(item => item.ProjectId == index);

                            //Редагування
                            if (comboBoxProjectOperationType.SelectedIndex == 1)
                            {
                                Classes.Project? project = GetProjectData();

                                if (project != null && project.ExecutionStatus == ExecutionStatus.closed && project.ActualEndDate == null)
                                {
                                    Classes.Project.OnValidationError -= Helper.ShowError;
                                    Helper.ShowError("You indicated the execution status - closed. Choose an actual end date.");
                                    return;
                                }

                                if (project != null && Classes.Project.isCorrectValues == 1)
                                {
                                    project.Id = index;

                                    var existingProject = Storage.Instance.projects.FirstOrDefault(p => p.Id == project.Id);
                                    string str = $"{existingProject.Id}, {existingProject.Name}, {existingProject.StartDate}, {existingProject.ExpectedEndDate}, {existingProject.ActualEndDate}, {existingProject.ExecutionStatus}, {existingProject.HeadProjectData}";

                                    if (existingProject != null)
                                    {
                                        if (project.ActualEndDate != null && 
                                            project.ExecutionStatus == ExecutionStatus.open && 
                                            existingProject.ExecutionStatus == ExecutionStatus.closed)
                                        {
                                            project.ActualEndDate = null;
                                        }
                                        else if (project.ActualEndDate != null &&
                                            project.ExecutionStatus == ExecutionStatus.open &&
                                            existingProject.ExecutionStatus == ExecutionStatus.open)
                                        {
                                            project.ExecutionStatus = ExecutionStatus.closed;
                                        }
                                    
                                        bool hasChanges = existingProject.Name != project.Name ||
                                            existingProject.StartDate != project.StartDate ||
                                            existingProject.ExpectedEndDate != project.ExpectedEndDate ||
                                            existingProject.ActualEndDate != project.ActualEndDate ||
                                            existingProject.ExecutionStatus != project.ExecutionStatus ||
                                            existingProject.headProject != project.headProject;

                                        if (hasChanges)
                                        {
                                            if (existingProject.ExecutionStatus == ExecutionStatus.open &&
                                                project.ExecutionStatus == ExecutionStatus.closed)
                                            {
                                                foreach(var part in Storage.Instance.participations)
                                                {
                                                    if (part.project.Id == existingProject.Id)
                                                    {
                                                        Storage.Instance.participations[part.Id].EndDate = project.ActualEndDate;

                                                        var participationDB = Storage.Instance.context.Participations
                                                            .FirstOrDefault(item => item.ParticipationId == part.Id);
                                                        participationDB.EndDate = project.ActualEndDate;
                                                        Storage.Instance.context.SaveChanges();

                                                        Transaction transactionP = new(Helper.GetIdTransaction(), Enums._Type.editing, $"{authorizedUser.FullName} edited a participation end date \nfrom \"NULL\" to\n\"{project.ActualEndDate}\".", DateTime.Now, authorizedUser);
                                                        Storage.Instance.transactions.Add(transactionP);

                                                        Helper.SaveTransactionDB(transactionP);
                                                    }
                                                }
                                            }

                                            existingProject.Name = project.Name;
                                            existingProject.StartDate = project.StartDate;
                                            existingProject.ExpectedEndDate = project.ExpectedEndDate;
                                            existingProject.ActualEndDate = project.ActualEndDate;
                                            existingProject.ExecutionStatus = project.ExecutionStatus;
                                            existingProject.headProject = project.headProject;

                                            var userPaa = Storage.Instance.context.UserPaas
                                                .FirstOrDefault(u => u.UserId == project.headProject.Id);

                                            projectDB.ProjectName = project.Name ?? projectDB.ProjectName;
                                            projectDB.StartDate = project.StartDate ?? projectDB.StartDate;
                                            projectDB.ExpectedEndDate = project.ExpectedEndDate ?? projectDB.ExpectedEndDate;
                                            projectDB.ActualEndDate = project.ActualEndDate;
                                            projectDB.ExecutionStatus = project.ExecutionStatus.ToString();
                                            projectDB.Head = userPaa;
                                            Storage.Instance.context.SaveChanges();

                                            Helper.ShowMessage("The project has been edited.");
                                            Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.editing, $"{authorizedUser.FullName} edited a project\nfrom \"{str}\" to\n\"{project.Id}, {project.Name}, {project.StartDate}, {project.ExpectedEndDate}, {project.ActualEndDate}, {project.ExecutionStatus}, {project.HeadProjectData}\".", DateTime.Now, authorizedUser);
                                            Storage.Instance.transactions.Add(transaction);

                                            Helper.SaveTransactionDB(transaction);

                                            dataGridProjects.ItemsSource = null;
                                            UpdateDataGridProjects();

                                            searchProjectFrame.textBoxSearch.Clear();
                                            ClearProjectFields();
                                        }
                                        else
                                        {
                                            Helper.ShowMessage("No changes detected.");
                                        }
                                    }
                                }

                                Classes.Project.isCorrectValues = 0;
                            }

                            //Видалення
                            else if (comboBoxProjectOperationType.SelectedIndex == 2)
                            {
                                var tempProject = Storage.Instance.projects.FirstOrDefault(item => item.Id == index);
                                string str = $"{tempProject.Id}, {tempProject.Name}";

                                Storage.Instance.participations.RemoveAll(item => item.project.Id == index);
                                Storage.Instance.states.RemoveAll(item => item.project.Id == index);

                                var participationsDB = Storage.Instance.context.Participations
                                    .Where(p => p.ProjectId == index)
                                    .ToList();
                                var statesDB = Storage.Instance.context.States
                                    .Where(s => s.ProjectId == index)
                                    .ToList();

                                foreach(var part in participationsDB)
                                {
                                    Transaction transactionP = new(Helper.GetIdTransaction(), Enums._Type.removal, $"{authorizedUser.FullName} deleted a participation\n\"{part.ParticipationId}\".", DateTime.Now, authorizedUser);
                                    Storage.Instance.transactions.Add(transactionP);
                                    Helper.SaveTransactionDB(transactionP);
                                }
                                foreach(var state in statesDB)
                                {
                                    Transaction transactionS = new(Helper.GetIdTransaction(), Enums._Type.removal, $"{authorizedUser.FullName} deleted a state\n\"{state.StateId}, {state.Description}\".", DateTime.Now, authorizedUser);
                                    Storage.Instance.transactions.Add(transactionS);
                                    Helper.SaveTransactionDB(transactionS);
                                }

                                Storage.Instance.context.Participations.RemoveRange(participationsDB);
                                Storage.Instance.context.States.RemoveRange(statesDB);

                                Storage.Instance.projects.RemoveAll(item => item.Id == index);

                                Storage.Instance.context.Projects.Remove(projectDB);
                                Storage.Instance.context.SaveChanges();

                                Helper.ShowMessage("The project has been deleted.");

                                Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.removal, $"{authorizedUser.FullName} deleted a project\n\"{str}\".", DateTime.Now, authorizedUser);
                                Storage.Instance.transactions.Add(transaction);

                                Helper.SaveTransactionDB(transaction);

                                dataGridProjects.ItemsSource = null;
                                UpdateDataGridProjects();

                                searchProjectFrame.textBoxSearch.Clear();
                                ClearProjectFields();
                            }
                        }
                        else
                            Helper.ShowError("No such project exists.");
                    }
                    else
                        Helper.ShowError("Select a project.");
                }

                Classes.Project.OnValidationError -= Helper.ShowError;
            }
            else Helper.ShowError("Fill in the blanks.");
        }
        private Classes.Project? GetProjectData()
        {
            ExecutionStatus newStatus;
            if (comboBoxExecutionStatusType.SelectedIndex == 0)
                newStatus = ExecutionStatus.open;
            else
                newStatus = ExecutionStatus.closed;

            string input = searchHeadProjectFrame.textBoxSearch.Text;
            User? headProject = Helper.ParseToObject(input, typeof(User)) as User;
            if (headProject != null && User.isCorrectValues == 3)
            {
                existingHeadProject = Storage.Instance.users.FirstOrDefault(p => p.Id == headProject.Id && p.FullName == headProject.FullName);
                User.isCorrectValues = 0;
            }

            Classes.Project? project = null;

            if (existingHeadProject != null)
            {
                User.OnValidationError += Helper.ShowError;

                if (existingHeadProject.CheckUserStatus(existingHeadProject) == 0)
                {
                    User.OnValidationError += Helper.ShowError;
                    return null;
                }
                if (existingHeadProject.Role != Role.headProject)
                {
                    User.OnValidationError += Helper.ShowError;
                    Helper.ShowError("The selected user is not the head of the projects.");
                    return null;
                }

                User.OnValidationError += Helper.ShowError;

                project = new();

                project.Name = textBoxProjectName.Text;
                project.headProject = existingHeadProject;
                project.StartDate = startDate.SelectedDate ?? DateTime.Now; // Якщо дата не вибрана, використовуємо поточну дату;
                project.ExpectedEndDate = expectedEndDate.SelectedDate ?? DateTime.Now;
                project.ActualEndDate = actualEndDate.SelectedDate;
                project.ExecutionStatus = newStatus;
            }
            else
                Helper.ShowError("No such head project exists.");

            return project;
        }
        private void ClearProjectFields()
        {
            textBoxProjectName.Clear();
            startDate.SelectedDate = null;
            expectedEndDate.SelectedDate = null;
            actualEndDate.SelectedDate = null;
            searchHeadProjectFrame.textBoxSearch.Clear();
        }
        
        // Використовується у тому випадку, якщо змінили стан на open/closed
        // Перейшли на іншу вкладку
        // dataGrid автоматично не оновився, то оновлюємо його вручну
        private void buttonUpdateProjectDataGrid_Click(object sender, RoutedEventArgs e)
        {
            dataGridProjects.ItemsSource = null;
            UpdateDataGridProjects();
            Helper.ShowMessage("Project table has been updated.");
        }
    }
}
