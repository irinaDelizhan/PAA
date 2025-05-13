using iTextSharp.text;
using PAA.Classes;
using PAA.Enums;
using PAA.Frames;
using PAA.Models;
using PAA.UserInterface;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for ParticipationsPage.xaml
    /// </summary>
    public partial class ParticipationsPage : Page
    {
        List<Page> framesList;
        private bool isInitialized = false;
        User? authorizedUser;

        DateFrame dateFrame = new DateFrame();
        SearchFrame searchParticipationFrame = new SearchFrame("participation");
        SearchFrame searchUserFrame1 = new SearchFrame("user");
        SearchFrame searchUserFrame2 = new SearchFrame("user");
        SearchFrame searchUserFrame3 = new SearchFrame("user");
        SearchFrame searchUserFrame4 = new SearchFrame("user");
        SearchFrame searchProjectFrame = new SearchFrame("project");

        Classes.Project existingProject;
        User existingUser;

        public ParticipationsPage(User? authorizedUser)
        {
            InitializeComponent();
            this.authorizedUser = authorizedUser;

            framesList = new List<Page>();

            framesList.Add(searchParticipationFrame);
            frameParticipation.Content = framesList[0];
            searchParticipationFrame.ParticipationSelected += OnParticipationSelected;

            framesList.Add(searchProjectFrame);
            frameProject.Content = framesList[1];
            
            framesList.Add(dateFrame);
            frameDate.Content = framesList[2];

            framesList.Add(searchUserFrame1);
            framesList.Add(searchUserFrame2);
            framesList.Add(searchUserFrame3);
            framesList.Add(searchUserFrame4);

            frameUser1.Content = framesList[3];
            frameUser2.Content = framesList[4];
            frameUser3.Content = framesList[5];
            frameUser4.Content = framesList[6];

            dataGridParticipations.ItemsSource = Storage.Instance.participations;

            frameParticipation.IsEnabled = false;
            dateFrame.enable = false;
            buttonSubtractExecutor.IsEnabled = false;
            VisibilityParticipationDetails(Visibility.Hidden);

            isInitialized = true;
        }
        private void OnParticipationSelected(Classes.Participation participation)
        {
            if (participation != null)
            {
                searchProjectFrame.enable = false;
                searchUserFrame1.enable = false;

                searchProjectFrame.textBoxSearch.Text = participation.ProjectData;
                searchUserFrame1.textBoxSearch.Text = participation.UserData;
                dateFrame.startDate.SelectedDate = participation.StartDate;
                dateFrame.endDate.SelectedDate = participation.EndDate;

                searchProjectFrame.enable = true;
                searchUserFrame1.enable = true;

                dataGridParticipations.SelectedItem = participation;
            }
            else
            {
                ClearParticipationFields();
                dataGridParticipations.ItemsSource = Storage.Instance.participations;
            }
        }
        private void comboBoxParticipationOperationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            { 
                searchParticipationFrame.textBoxSearch.Clear();
                ClearParticipationFields();

                if (comboBoxParticipationOperationType.SelectedIndex == 0)
                {
                    frameParticipation.IsEnabled = false;
                    EnableForParticipationDetails(true);
                    dateFrame.enable = false;
                    dateFrame.endDate.IsEnabled = false;
                    VisibilityParticipationButtons(Visibility.Visible);
                    VisibilityParticipationDetails(Visibility.Hidden);
                    buttonSubtractExecutor.IsEnabled = false;
                    buttonAddExecutor.IsEnabled = true;
                }
                else if (comboBoxParticipationOperationType.SelectedIndex == 1)
                {
                    frameParticipation.IsEnabled = true;
                    EnableForParticipationDetails(true);
                    dateFrame.enable = true;
                    dateFrame.endDate.IsEnabled = true;
                    VisibilityParticipationButtons(Visibility.Hidden);
                    VisibilityParticipationDetails(Visibility.Hidden);
                }
                else if (comboBoxParticipationOperationType.SelectedIndex == 2)
                {
                    frameParticipation.IsEnabled = true;
                    EnableForParticipationDetails(false);
                    VisibilityParticipationButtons(Visibility.Hidden);
                    VisibilityParticipationDetails(Visibility.Hidden);
                }
            }
        }
        private void EnableForParticipationDetails(bool enable)
        {
            frameUser1.IsEnabled = enable;
            frameProject.IsEnabled = enable;
            frameDate.IsEnabled = enable;
        }
        private void VisibilityParticipationButtons(Visibility visibility)
        {
            buttonAddExecutor.Visibility = visibility;
            buttonSubtractExecutor.Visibility = visibility;
        }
        private void VisibilityParticipationDetails(Visibility visibility)
        {
            frameUser2.Visibility = visibility;
            frameUser3.Visibility = visibility;
            frameUser4.Visibility = visibility;
        }
        private void buttonPerformOperationOnParticipation_Click(object sender, RoutedEventArgs e)
        {
            if (!Helper.IsValidInput(searchParticipationFrame.textBoxSearch.Text) ||
                !Helper.IsValidInput(searchProjectFrame.textBoxSearch.Text) ||
                !Helper.IsValidInput(searchUserFrame1.textBoxSearch.Text) ||
                !Helper.IsValidInput(searchUserFrame2.textBoxSearch.Text) ||
                !Helper.IsValidInput(searchUserFrame3.textBoxSearch.Text) ||
                !Helper.IsValidInput(searchUserFrame4.textBoxSearch.Text))
            {
                Helper.ShowError("The entered text contains prohibited SQL statements.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(searchProjectFrame.textBoxSearch.Text) &&
                dateFrame.startDate.SelectedDate != null)
            {
                Classes.Participation.OnValidationError += Helper.ShowError;

                if (comboBoxParticipationOperationType.SelectedIndex == 0)
                {
                    if (string.IsNullOrWhiteSpace(searchUserFrame1.textBoxSearch.Text) &&
                        string.IsNullOrWhiteSpace(searchUserFrame2.textBoxSearch.Text) &&
                        string.IsNullOrWhiteSpace(searchUserFrame3.textBoxSearch.Text) &&
                        string.IsNullOrWhiteSpace(searchUserFrame4.textBoxSearch.Text))
                    {
                        Classes.Participation.OnValidationError -= Helper.ShowError;
                        Helper.ShowError("Fill in the blanks.");
                        return;
                    }

                    string input = searchProjectFrame.textBoxSearch.Text;
                    Classes.Project? project = Helper.ParseToObject(input, typeof(Classes.Project)) as Classes.Project;
                    if (project != null && Classes.Project.isCorrectValues == 1)
                    {
                        existingProject = Storage.Instance.projects.FirstOrDefault(p => p.Id == project.Id && p.Name == project.Name);
                        Classes.Project.isCorrectValues = 0;
                    }

                    if (existingProject != null)
                    {
                        Classes.Project.OnValidationError += Helper.ShowError;

                        if (existingProject.CheckProjectStatus(existingProject) == 0)
                        {
                            Classes.Participation.OnValidationError -= Helper.ShowError;
                            Classes.Project.OnValidationError -= Helper.ShowError;
                            return;
                        }

                        if (existingProject.headProject.Id != authorizedUser.Id && authorizedUser.Role == Role.headProject)
                        {
                            Classes.Participation.OnValidationError -= Helper.ShowError;
                            Classes.Project.OnValidationError -= Helper.ShowError;
                            Helper.ShowError("You can only add participation from projects where you are the head.");
                            return;
                        }

                        Classes.Project.OnValidationError -= Helper.ShowError;
                    }
                    else if (existingProject == null)
                    {
                        Classes.Participation.OnValidationError -= Helper.ShowError;
                        Helper.ShowError($"No such project exists.");
                        return;
                    }

                    int newId;
                    int countUser = 0;

                    var searchUserFrames = new[] { searchUserFrame1, searchUserFrame2, searchUserFrame3, searchUserFrame4 }
                        .DistinctBy(frame => frame.textBoxSearch.Text)
                        .ToArray();

                    foreach (var frame in searchUserFrames)
                    {
                        if (!string.IsNullOrWhiteSpace(frame.textBoxSearch.Text))
                        {
                            newId = Storage.Instance.participations.Count > 0
                                ? Storage.Instance.participations.Last().Id + 1
                                : 0;

                            Classes.Participation? participation = GetParticipationData(frame.textBoxSearch.Text);

                            if (participation != null)
                            {
                                participation.Id = newId;
                                Storage.Instance.participations.Add(participation);

                                var projectDB = Storage.Instance.context.Projects
                                    .FirstOrDefault(s => s.ProjectId == participation.project.Id);
                                var userDB = Storage.Instance.context.UserPaas
                                    .FirstOrDefault(p => p.UserId == participation.user.Id);
                                Models.Participation participationDB = new Models.Participation
                                {
                                    ParticipationId = participation.Id,
                                    Project = projectDB,
                                    User = userDB,
                                    StartDate = participation.StartDate ?? DateTime.Now
                                };
                                Storage.Instance.context.Participations.Add(participationDB);
                                Storage.Instance.context.SaveChanges();

                                Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.addition, $"{authorizedUser.FullName} added a participation\n\"{participation.Id}, {participation.ProjectAndUserData}, {participation.StartDate}, {participation.EndDate}\".", DateTime.Now, authorizedUser);
                                Storage.Instance.transactions.Add(transaction);

                                Helper.SaveTransactionDB(transaction);

                                countUser++;
                            }
                        }
                    }

                    if (countUser > 0)
                    {
                        Helper.ShowMessage("There is a participations added.");

                        ClearParticipationFields();

                        dataGridParticipations.ItemsSource = null;
                        dataGridParticipations.ItemsSource = Storage.Instance.participations;
                    }
                }
                else if (comboBoxParticipationOperationType.SelectedIndex == 1 ||
                    comboBoxParticipationOperationType.SelectedIndex == 2)
                {
                    if (!string.IsNullOrWhiteSpace(searchParticipationFrame.textBoxSearch.Text))
                    {
                        string[] parts = searchParticipationFrame.textBoxSearch.Text.Split(' ');

                        if (parts.Length > 0 &&
                            int.TryParse(parts[0], out int index) &&
                            index >= 0 &&
                            index <= Storage.Instance.participations.Last().Id)
                        {
                            var tempParticipation = Storage.Instance.participations.FirstOrDefault(p => p.Id == index);
                            if (tempParticipation.project.headProject.Id != authorizedUser.Id && authorizedUser.Role == Role.headProject)
                            {
                                Classes.Participation.OnValidationError -= Helper.ShowError;
                                Helper.ShowError("You can only edit or delete participation from projects where you are the head.");
                                return;
                            }

                            var participationDB = Storage.Instance.context.Participations
                                .FirstOrDefault(item => item.ParticipationId == index);

                            //Редагування
                            if (comboBoxParticipationOperationType.SelectedIndex == 1)
                            {
                                Classes.Participation? participation = GetParticipationData(searchUserFrame1.textBoxSearch.Text);

                                if (participation != null)
                                {
                                    participation.Id = index;

                                    var existingParticipation = Storage.Instance.participations.FirstOrDefault(p => p.Id == participation.Id);

                                    string str = $"{existingParticipation.Id}, {existingParticipation.ProjectAndUserData}, {existingParticipation.StartDate}, {existingParticipation.EndDate}";

                                    if (existingParticipation != null)
                                    {
                                        bool hasChanges = existingParticipation.project != participation.project ||
                                            existingParticipation.user != participation.user ||
                                            existingParticipation.StartDate != participation.StartDate ||
                                            existingParticipation.EndDate != participation.EndDate;

                                        if (hasChanges)
                                        {
                                            existingParticipation.project = participation.project;
                                            existingParticipation.user = participation.user;
                                            existingParticipation.StartDate = participation.StartDate;
                                            existingParticipation.EndDate = participation.EndDate;

                                            var userPaa = Storage.Instance.context.UserPaas
                                                .FirstOrDefault(u => u.UserId == participation.user.Id);
                                            var project = Storage.Instance.context.Projects
                                                .FirstOrDefault(p => p.ProjectId == participation.project.Id);
                                            participationDB.Project = project ?? participationDB.Project;
                                            participationDB.User = userPaa ?? participationDB.User;
                                            participationDB.StartDate = participation.StartDate ?? participationDB.StartDate;
                                            participationDB.EndDate = participation.EndDate;
                                            Storage.Instance.context.SaveChanges();

                                            Helper.ShowMessage("The participation has been edited.");
                                            Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.editing, $"{authorizedUser.FullName} edited a participation\nfrom \"{str}\" to\n\"{participation.Id}, {participation.ProjectAndUserData}, {participation.StartDate}, {participation.EndDate}\".", DateTime.Now, authorizedUser);
                                            Storage.Instance.transactions.Add(transaction);

                                            Helper.SaveTransactionDB(transaction);

                                            dataGridParticipations.ItemsSource = null;
                                            dataGridParticipations.ItemsSource = Storage.Instance.participations;
                                            searchParticipationFrame.textBoxSearch.Clear();
                                            ClearParticipationFields();
                                        }
                                        else
                                        {
                                            Helper.ShowMessage("No changes detected.");
                                        }
                                    }
                                }
                            }

                            //Видалення
                            else if (comboBoxParticipationOperationType.SelectedIndex == 2)
                            {
                                string str = $"{tempParticipation.Id}, {tempParticipation.ProjectAndUserData}";

                                Storage.Instance.participations.RemoveAll(item => item.Id == index);

                                Storage.Instance.context.Participations.Remove(participationDB);
                                Storage.Instance.context.SaveChanges();

                                Helper.ShowMessage("The participation has been deleted.");
                                
                                Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.removal, $"{authorizedUser.FullName} deleted a participation\n\"{str}\".", DateTime.Now, authorizedUser);
                                Storage.Instance.transactions.Add(transaction);

                                Helper.SaveTransactionDB(transaction);

                                dataGridParticipations.ItemsSource = null;
                                dataGridParticipations.ItemsSource = Storage.Instance.participations;

                                searchParticipationFrame.textBoxSearch.Clear();
                                ClearParticipationFields();
                            }
                        }
                        else
                            Helper.ShowError("No such participation exists.");
                    }
                    else
                        Helper.ShowError("Select a participation.");
                }

                Classes.Participation.OnValidationError += Helper.ShowError;
            }
            else Helper.ShowError("Fill in the blanks.");
        }
        private Classes.Participation? GetParticipationData(string inputUser)
        {
            string input = searchProjectFrame.textBoxSearch.Text;
            Classes.Project? project = Helper.ParseToObject(input, typeof(Classes.Project)) as Classes.Project;
            if (project != null && Classes.Project.isCorrectValues == 1)
            {
                existingProject = Storage.Instance.projects.FirstOrDefault(p => p.Id == project.Id && p.Name == project.Name);
                Classes.Project.isCorrectValues = 0;
            }

            input = inputUser;
            User? user = Helper.ParseToObject(input, typeof(User)) as User;
            if (user != null && User.isCorrectValues == 3)
            {
                existingUser = Storage.Instance.users.FirstOrDefault(p => p.Id == user.Id && p.FullName == user.FullName);
                User.isCorrectValues = 0;
            }

            Classes.Participation? participation = null;

            if (existingProject != null && existingUser != null)
            {
                User.OnValidationError += Helper.ShowError;

                if (existingUser.CheckUserStatus(existingUser) == 0)
                {
                    User.OnValidationError -= Helper.ShowError;
                    return null;
                }
                if (existingUser.Role != Role.employee)
                {
                    User.OnValidationError -= Helper.ShowError;
                    Helper.ShowError("The contractor must be an employee.");
                    return null;
                }

                User.OnValidationError -= Helper.ShowError;

                Classes.Project.OnValidationError += Helper.ShowError;

                if (existingProject.CheckProjectStatus(existingProject) == 0)
                {
                    Classes.Project.OnValidationError -= Helper.ShowError;
                    return null;
                }

                Classes.Project.OnValidationError -= Helper.ShowError;

                participation = new();

                participation.project = existingProject;
                participation.user = existingUser;
                participation.StartDate = dateFrame.startDate.SelectedDate ?? DateTime.Now;
                participation.EndDate = dateFrame.endDate.SelectedDate;
            }
            else if (existingProject == null)
                Helper.ShowError($"No such project exists.");
            else if (existingUser == null)
                Helper.ShowError($"No such user \"{inputUser}\" exists.");

            return participation;
        }
        private void buttonSubtractExecutor_Click(object sender, RoutedEventArgs e)
        {
            if (frameUser4.Visibility == Visibility.Visible)
            {
                frameUser4.Visibility = Visibility.Hidden;
                searchUserFrame4.textBoxSearch.Clear();
                buttonAddExecutor.IsEnabled = true;
            }
            else if (frameUser3.Visibility == Visibility.Visible)
            {
                frameUser3.Visibility = Visibility.Hidden;
                searchUserFrame3.textBoxSearch.Clear();
            }
            else if (frameUser2.Visibility == Visibility.Visible)
            {
                frameUser2.Visibility = Visibility.Hidden;
                searchUserFrame2.textBoxSearch.Clear();
                buttonSubtractExecutor.IsEnabled = false;
            }
        }
        private void buttonAddExecutor_Click(object sender, RoutedEventArgs e)
        {
            if (frameUser2.Visibility == Visibility.Hidden)
            {
                frameUser2.Visibility = Visibility.Visible;
                buttonSubtractExecutor.IsEnabled = true;
            }
            else if (frameUser2.Visibility == Visibility.Visible && frameUser3.Visibility == Visibility.Hidden)
            {
                frameUser3.Visibility = Visibility.Visible;
            }
            else if (frameUser3.Visibility == Visibility.Visible && frameUser4.Visibility == Visibility.Hidden)
            {
                frameUser4.Visibility = Visibility.Visible;
                buttonAddExecutor.IsEnabled = false;
            }
        }
        private void ClearParticipationFields()
        {
            searchProjectFrame.textBoxSearch.Clear();
            dateFrame.startDate.SelectedDate = null;
            dateFrame.endDate.SelectedDate = null;
            searchUserFrame1.textBoxSearch.Clear();
            searchUserFrame2.textBoxSearch.Clear();
            searchUserFrame3.textBoxSearch.Clear();
            searchUserFrame4.textBoxSearch.Clear();
        }

        private void buttonUpdateParticipationDataGrid_Click(object sender, RoutedEventArgs e)
        {
            dataGridParticipations.ItemsSource = null;
            dataGridParticipations.ItemsSource = Storage.Instance.participations;
            Helper.ShowMessage("Participation table has been updated.");
        }
    }
}
