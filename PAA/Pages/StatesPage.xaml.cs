using PAA.Classes;
using PAA.Enums;
using PAA.Frames;
using PAA.Models;
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
    /// Interaction logic for StatesPage.xaml
    /// </summary>
    public partial class StatesPage : Page
    {
        List<Page> framesList;
        private bool isInitialized = false;
        public string? role = null;
        User? authorizedUser;

        SearchFrame searchStateFrame;
        SearchFrame searchProjectFrame;

        Classes.Project existingProject;

        public StatesPage(string role, User? authorizedUser)
        {
            InitializeComponent();
            this.role = role;
            this.authorizedUser = authorizedUser;

            framesList = new List<Page>();

            searchStateFrame = new SearchFrame("state");
            framesList.Add(searchStateFrame);
            frameState.Content = framesList[0];
            searchStateFrame.StateSelected += OnStateSelected;

            searchProjectFrame = new SearchFrame("project");
            framesList.Add(searchProjectFrame);
            frameProject.Content = framesList[1];

            frameState.IsEnabled = false;
            dataGridStates.ItemsSource = Storage.Instance.states;

            UpdateStateDataGridToHPE();

            isInitialized = true;
        }
        private void UpdateStateDataGridToHPE()
        {
            if (role == "employee" || role == "headProject")
            {
                ProjectsPage.UpdateComboBoxItemsVisibility(comboBoxStateOperationType, item =>
                    item.Content.ToString() == "Addition a state");

                if (role == "headProject")
                    dataGridStates.ItemsSource = Storage.Instance.states
                        .Where(s => s.project.headProject.Id == authorizedUser.Id)
                        .ToList();
                else if (role == "employee")
                    dataGridStates.ItemsSource = Storage.Instance.states
                        .Where(s => Storage.Instance.participations.Any(p => p.project.Id == s.project.Id && p.user.Id == authorizedUser.Id))
                        .ToList();
            }
        }
        private void OnStateSelected(Classes.State state)
        {
            if (state != null)
            {
                searchProjectFrame.enable = false;

                searchProjectFrame.textBoxSearch.Text = state.project.Id + " " + state.project.Name;
                textBoxDescription.Text = state.Description;

                searchProjectFrame.enable = true;

                dataGridStates.SelectedItem = state;
            }
            else
            {
                ClearStateFields();
                dataGridStates.ItemsSource = Storage.Instance.states;
            }
        }
        private void comboBoxStateOperationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                searchStateFrame.textBoxSearch.Clear();
                ClearStateFields();

                if (comboBoxStateOperationType.SelectedIndex == 0)
                {
                    frameState.IsEnabled = false;
                    EnableForStateDetails(true);
                }
                else if (comboBoxStateOperationType.SelectedIndex == 1)
                {
                    frameState.IsEnabled = true;
                    EnableForStateDetails(true);
                }
                else if (comboBoxStateOperationType.SelectedIndex == 2)
                {
                    frameState.IsEnabled = true;
                    EnableForStateDetails(false);
                }
            }
        }
        private void EnableForStateDetails(bool enable)
        {
            frameProject.IsEnabled = enable;
            textBoxDescription.IsEnabled = enable;
        }

        private void buttonPerformOperationOnState_Click(object sender, RoutedEventArgs e)
        {
            if (!Helper.IsValidInput(searchStateFrame.textBoxSearch.Text) ||
                !Helper.IsValidInput(searchProjectFrame.textBoxSearch.Text) ||
                !Helper.IsValidInput(textBoxDescription.Text))
            {
                Helper.ShowError("The entered text contains prohibited SQL statements.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(textBoxDescription.Text) &&
                !string.IsNullOrWhiteSpace(searchProjectFrame.textBoxSearch.Text))
            {
                Classes.State.OnValidationError += Helper.ShowError;

                if (comboBoxStateOperationType.SelectedIndex == 0)
                {
                    int newId = Storage.Instance.states.Count > 0
                        ? Storage.Instance.states.Last().Id + 1
                        : 0;

                    Classes.State? state = GetStateData();

                    if (state != null)
                    {
                        if (authorizedUser.Role == Role.headProject && 
                            state.project.headProject.Id != authorizedUser.Id)
                        {
                            Classes.State.OnValidationError -= Helper.ShowError;
                            Helper.ShowError("You can only add states from projects where you are the head.");
                            return;
                        }
                        else if (authorizedUser.Role == Role.employee)
                        {
                            var tempProjects = Storage.Instance.projects
                                .Where(s => Storage.Instance.participations.Any(p => p.project.Id == state.project.Id && p.user.Id == authorizedUser.Id))
                                .ToList();

                            if (tempProjects == null || tempProjects.Count == 0)
                            {
                                Classes.State.OnValidationError -= Helper.ShowError;
                                Helper.ShowError("You can only add states from projects where you are working on.");
                                return;
                            }
                        }

                        state.Id = newId;
                        state.Date = DateTime.Now;
                        state.user = authorizedUser;
                        Storage.Instance.states.Add(state);

                        var userPaa = Storage.Instance.context.UserPaas
                            .FirstOrDefault(p => p.UserId == state.user.Id);
                        var projectDB = Storage.Instance.context.Projects
                            .FirstOrDefault(p => p.ProjectId == state.project.Id);
                        Models.State stateDB = new Models.State
                        {
                            StateId = state.Id,
                            Description = state.Description,
                            Project = projectDB,
                            User = userPaa,
                            Date = state.Date ?? DateTime.Now
                        };
                        Storage.Instance.context.States.Add(stateDB);
                        Storage.Instance.context.SaveChanges();

                        Helper.ShowMessage("There is a state added.");
                        Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.addition, $"{authorizedUser.FullName} added a state\n\"{state.Id}, {state.ProjectData}, {state.Description}, {state.Date}\".", DateTime.Now, authorizedUser);
                        Storage.Instance.transactions.Add(transaction);

                        Helper.SaveTransactionDB(transaction);

                        ClearStateFields();

                        dataGridStates.ItemsSource = null;
                        dataGridStates.ItemsSource = Storage.Instance.states;
                        UpdateStateDataGridToHPE();
                    }

                    Classes.State.isCorrectValues = 0;
                }
                else if (comboBoxStateOperationType.SelectedIndex == 1 ||
                    comboBoxStateOperationType.SelectedIndex == 2)
                {
                    if (!string.IsNullOrWhiteSpace(searchStateFrame.textBoxSearch.Text))
                    {
                        string[] parts = searchStateFrame.textBoxSearch.Text.Split(' ');

                        if (parts.Length > 0 &&
                            int.TryParse(parts[0], out int index) &&
                            index >= 0 &&
                            index <= Storage.Instance.states.Last().Id)
                        {
                            var stateDB = Storage.Instance.context.States
                                .FirstOrDefault(item => item.StateId == index);

                            //Редагування
                            if (comboBoxStateOperationType.SelectedIndex == 1)
                            {
                                Classes.State? state = GetStateData();

                                if (state != null)
                                {
                                    state.Id = index;

                                    var existingState = Storage.Instance.states.FirstOrDefault(p => p.Id == state.Id);
                                    string str = $"{existingState.Id}, {existingState.Description}, {existingState.ProjectData}, {existingState.Date}";

                                    if (existingState != null)
                                    {
                                        bool hasChanges = existingState.Description != state.Description ||
                                            existingState.project != state.project;

                                        if (hasChanges)
                                        {
                                            existingState.Description = state.Description;
                                            existingState.project = state.project;

                                            var projectDB = Storage.Instance.context.Projects
                                                .FirstOrDefault(p => p.ProjectId == state.project.Id);
                                            stateDB.Description = state.Description;
                                            stateDB.Project = projectDB;
                                            Storage.Instance.context.SaveChanges();

                                            Helper.ShowMessage("The state has been edited.");
                                            Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.editing, $"{authorizedUser.FullName} edited a state\nfrom \"{str}\" to\n\"{state.Id}, {state.Description}, {state.ProjectData}, {state.Date}\".", DateTime.Now, authorizedUser);
                                            Storage.Instance.transactions.Add(transaction);

                                            Helper.SaveTransactionDB(transaction);

                                            dataGridStates.ItemsSource = null;
                                            dataGridStates.ItemsSource = Storage.Instance.states;
                                            
                                            searchStateFrame.textBoxSearch.Clear();
                                            ClearStateFields();
                                        }
                                        else
                                        {
                                            Helper.ShowMessage("No changes detected.");
                                        }
                                    }
                                }

                                Classes.State.isCorrectValues = 0;
                            }

                            //Видалення
                            else if (comboBoxStateOperationType.SelectedIndex == 2)
                            {
                                var tempState = Storage.Instance.states.FirstOrDefault(item => item.Id == index);
                                string str = $"{tempState.Id}, {tempState.Description}";

                                Storage.Instance.states.RemoveAll(item => item.Id == index);

                                Storage.Instance.context.States.Remove(stateDB);
                                Storage.Instance.context.SaveChanges();

                                Helper.ShowMessage("The state has been deleted.");

                                Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.removal, $"{authorizedUser.FullName} deleted a state\n\"{str}\".", DateTime.Now, authorizedUser);
                                Storage.Instance.transactions.Add(transaction);

                                Helper.SaveTransactionDB(transaction);

                                dataGridStates.ItemsSource = null;
                                dataGridStates.ItemsSource = Storage.Instance.states;

                                searchStateFrame.textBoxSearch.Clear();
                                ClearStateFields();
                            }
                        }
                        else
                            Helper.ShowError("No such state exists.");
                    }
                    else
                        Helper.ShowError("Select a state.");
                }

                Classes.State.OnValidationError -= Helper.ShowError;
            }
            else Helper.ShowError("Fill in the blanks.");
        }
        private Classes.State? GetStateData()
        {
            string input = searchProjectFrame.textBoxSearch.Text;
            Classes.Project? project = Helper.ParseToObject(input, typeof(Classes.Project)) as Classes.Project;
            if (project != null && Classes.Project.isCorrectValues == 1)
            {
                existingProject = Storage.Instance.projects.FirstOrDefault(p => p.Id == project.Id && p.Name == project.Name);
                Classes.Project.isCorrectValues = 0;
            }

            Classes.State? state = null;

            if (existingProject != null)
            {
                Classes.Project.OnValidationError += Helper.ShowError;

                if (existingProject.CheckProjectStatus(existingProject) == 0)
                {
                    Classes.Project.OnValidationError -= Helper.ShowError;
                    return null;
                }

                Classes.Project.OnValidationError -= Helper.ShowError;

                state = new();

                state.Description = textBoxDescription.Text;
                if (Classes.State.isCorrectValues == 1)
                {
                    state.project = existingProject;
                }
            }
            else
                Helper.ShowError("No such project exists.");

            if (Classes.State.isCorrectValues == 1)
                return state;
            else
                return null;
        }
        private void ClearStateFields()
        {
            textBoxDescription.Clear();
            searchProjectFrame.textBoxSearch.Clear();
        }

        private void buttonUpdateStateDataGrid_Click(object sender, RoutedEventArgs e)
        {
            dataGridStates.ItemsSource = null;
            dataGridStates.ItemsSource = Storage.Instance.states;
            UpdateStateDataGridToHPE();
            Helper.ShowMessage("State table has been updated.");
        }
    }
}
