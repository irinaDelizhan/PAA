using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using SWC = System.Windows.Controls;
using static MaterialDesignThemes.Wpf.Theme;
using PAA.Frames;
using PAA.UserInterface;
using PAA.Classes;
using PAA.Enums;
using PAA.Models;

namespace PAA.Pages
{
    /// <summary>
    /// Interaction logic for UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        List<Page> framesList;
        private bool isInitialized = false;
        public string? role = null;
        User? authorizedUser;

        DateFrame dateFrame = new DateFrame("search");
        SearchFrame searchSectionFrame;
        SearchFrame searchPositionFrame;
        SearchFrame searchUserFrame;

        Classes.Position existingPosition;
        _Section existingSection;

        public UsersPage(string role, User? authorizedUser)
        {
            InitializeComponent();

            this.role = role;
            this.authorizedUser = authorizedUser;

            framesList = new List<Page>();

            searchSectionFrame = new SearchFrame("section");
            framesList.Add(searchSectionFrame);
            frameSection.Content = framesList[0];

            searchPositionFrame = new SearchFrame("position");
            framesList.Add(searchPositionFrame);
            framePosition.Content = framesList[1];

            searchUserFrame = new SearchFrame("user");
            framesList.Add(searchUserFrame);
            frameUser.Content = framesList[2];
            searchUserFrame.UserSelected += OnUserSelected;

            framesList.Add(dateFrame);
            frameDate.Content = framesList[3];

            dataGridUsers.ItemsSource = Storage.Instance.users;

            frameUser.IsEnabled = false;
            dateFrame.enable = false;
            dateFrame.endDate.IsEnabled = false;

            if (role == "admin")
            {
                ProjectsPage.UpdateComboBoxItemsVisibility(comboBoxRoleType, item =>
                    item.Content.ToString() != "Administrator" &&
                    item.Content.ToString() != "The head of the company");
            }

            ProjectsPage.UpdateComboBoxItemsVisibility(comboBoxUserStatusType, item =>
                item.Content.ToString() == "Active");

            isInitialized = true;
        }
        private void OnUserSelected(User user)
        {
            if (user != null)
            {
                searchSectionFrame.enable = false;
                searchPositionFrame.enable = false;

                textBoxLogin.Text = user.Login;
                textBoxPassword.Password = user.Password;
                textBoxAddress.Text = user.Address;
                searchSectionFrame.textBoxSearch.Text = user.section.Id + " " + user.section.Name;
                searchPositionFrame.textBoxSearch.Text = user.position.Id + " " + user.position.Name;
                textBoxName.Text = user.Name;
                textBoxSurname.Text = user.Surname;
                textBoxPatronymic.Text = user.Patronymic;
                dateFrame.startDate.SelectedDate = user.StartDate;
                dateFrame.endDate.SelectedDate = user.EndDate;
                
                if (user.Status == Status.active)
                {
                    comboBoxUserStatusType.SelectedIndex = 0;
                }
                else if (user.Status == Status.passive)
                {
                    comboBoxUserStatusType.SelectedIndex = 1;
                }
                else
                {
                    comboBoxUserStatusType.SelectedIndex = 2;
                }

                if (user.Phone.StartsWith("+380 "))
                {
                    textBoxPhone.Text = user.Phone.Substring(5);
                }
                else
                {
                    textBoxPhone.Text = user.Phone.Substring(3);
                }

                if (user.Role == Role.headProject)
                {
                    comboBoxRoleType.SelectedIndex = 0;
                }
                else if (user.Role == Role.employee)
                {
                    comboBoxRoleType.SelectedIndex = 1;
                }
                else if (user.Role == Role.admin)
                {
                    comboBoxRoleType.SelectedIndex = 2;
                }
                else
                {
                    comboBoxRoleType.SelectedIndex = 3;
                }

                dataGridUsers.SelectedItem = user;

                searchSectionFrame.enable = true;
                searchPositionFrame.enable = true;
            }
            else
            {
                ClearUserFields();
                dataGridUsers.ItemsSource = Storage.Instance.users;
            }
        }
        private void textBoxPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true; // Блокуємо введення нецифрових символів
                return;
            }

            SWC.TextBox textBox = (SWC.TextBox)sender;
            string newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            newText = newText.Replace(" ", "");

            if (comboBoxPhoneType.SelectedIndex == 0)
            {
                if (textBoxPhone.Text.Length > 11)
                    return;
                else
                    newText = UkrainianFormatPhoneNumber(newText);
            }
            else
            {
                if (textBoxPhone.Text.Length > 12)
                    return;
                else
                    newText = USAFormatPhoneNumber(newText);
            }

            textBox.Text = newText;

            // Переміщуємо курсор в кінець
            textBox.CaretIndex = newText.Length;

            // Забороняємо стандартну обробку тексту
            e.Handled = true;
        }
        private string UkrainianFormatPhoneNumber(string text)
        {
            if (text.Length > 2)
            {
                text = text.Insert(2, " ");
            }
            if (text.Length > 6)
            {
                text = text.Insert(6, " ");
            }
            return text;
        }
        private string USAFormatPhoneNumber(string text)
        {
            if (text.Length > 3)
            {
                text = text.Insert(3, " ");
            }
            if (text.Length > 7)
            {
                text = text.Insert(7, " ");
            }
            return text;
        }

        private void comboBoxPhoneType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxPhoneType.SelectedIndex == 0)
                textBoxPhone.MaxLength = 11;
            else
                textBoxPhone.MaxLength = 12;
            
            textBoxPhone.Text = "";
        }

        private void buttonPerformOperationOnUser_Click(object sender, RoutedEventArgs e)
        {
            if (!Helper.IsValidInput(textBoxName.Text) ||
                !Helper.IsValidInput(textBoxSurname.Text) ||
                !Helper.IsValidInput(textBoxPatronymic.Text) ||
                !Helper.IsValidInput(textBoxLogin.Text) ||
                !Helper.IsValidInput(textBoxPassword.Password) ||
                !Helper.IsValidInput(textBoxPhone.Text) ||
                !Helper.IsValidInput(textBoxAddress.Text) ||
                !Helper.IsValidInput(searchSectionFrame.textBoxSearch.Text) ||
                !Helper.IsValidInput(searchPositionFrame.textBoxSearch.Text) ||
                !Helper.IsValidInput(searchUserFrame.textBoxSearch.Text) ||
                !Helper.IsValidInput(textBoxSurname.Text))
            {
                Helper.ShowError("The entered text contains prohibited SQL statements.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(textBoxName.Text) &&
                !string.IsNullOrWhiteSpace(textBoxSurname.Text) &&
                !string.IsNullOrWhiteSpace(textBoxPatronymic.Text) &&
                !string.IsNullOrWhiteSpace(textBoxLogin.Text) &&
                !string.IsNullOrWhiteSpace(textBoxPassword.Password) &&
                !string.IsNullOrWhiteSpace(textBoxPhone.Text) &&
                !string.IsNullOrWhiteSpace(textBoxAddress.Text) &&
                !string.IsNullOrWhiteSpace(searchSectionFrame.textBoxSearch.Text) &&
                !string.IsNullOrWhiteSpace(searchPositionFrame.textBoxSearch.Text) &&
                dateFrame.startDate.SelectedDate != null)
            {
                User.OnValidationError += Helper.ShowError;

                if (comboBoxUserOperationType.SelectedIndex == 0)
                {
                    int newId = Storage.Instance.users.Count > 0
                        ? Storage.Instance.users.Last().Id + 1
                        : 0;

                    User? user = GetUserData();

                    User? tempUser = Storage.Instance.users.FirstOrDefault(u => u.Login == user.Login);
                    if (tempUser != null)
                    {
                        User.OnValidationError -= Helper.ShowError;
                        Helper.ShowError("Login already exists.");
                        return;
                    }

                    if (user != null && User.isCorrectValues == 7)
                    {
                        user.Id = newId;

                        Storage.Instance.users.Add(user);

                        // Конвертація User -> UserPaa
                        var section = Storage.Instance.context.Sections
                            .FirstOrDefault(s => s.SectionId == user.section.Id);
                        var position = Storage.Instance.context.Positions
                            .FirstOrDefault(p => p.PositionId == user.position.Id);

                        UserPaa userPaa = new UserPaa
                        {
                            UserId = user.Id,
                            Name = user.Name ?? string.Empty,
                            Surname = user.Surname ?? string.Empty,
                            Patronymic = user.Patronymic ?? string.Empty,
                            Login = user.Login ?? string.Empty,
                            Password = user.EncryptedPassword ?? string.Empty,
                            Phone = user.Phone ?? string.Empty,
                            Address = user.Address ?? string.Empty,
                            Role = user.Role.ToString(), // Перетворення enum в string
                            Section = section,
                            Position = position,
                            StartDate = user.StartDate ?? DateTime.Now,
                            EndDate = user.EndDate,
                            Status = user.Status.ToString()
                        };
                        Storage.Instance.context.UserPaas.Add(userPaa);
                        Storage.Instance.context.SaveChanges();

                        Helper.ShowMessage("There is a user added.");
                        Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.addition, $"{authorizedUser.FullName} added a user\n\"{user.Id}, {user.Name}, {user.Surname}, {user.Patronymic}, {user.Login}, {user.EncryptedPassword} {user.Role},\n{user.Phone}, {user.Address}, {user.SectionData}, {user.PositionData}, {user.Status}, {user.StartDate}, {user.EndDate}\".", DateTime.Now, authorizedUser);
                        Storage.Instance.transactions.Add(transaction);

                        Helper.SaveTransactionDB(transaction);

                        ClearUserFields();

                        dataGridUsers.ItemsSource = null;
                        dataGridUsers.ItemsSource = Storage.Instance.users;
                    }

                    User.isCorrectValues = 0;
                }
                else if (comboBoxUserOperationType.SelectedIndex == 1 ||
                    comboBoxUserOperationType.SelectedIndex == 2)
                {
                    if (!string.IsNullOrWhiteSpace(searchUserFrame.textBoxSearch.Text))
                    {
                        string[] parts = searchUserFrame.textBoxSearch.Text.Split(' ');

                        if (parts.Length > 0 && 
                            int.TryParse(parts[0], out int index) && 
                            index >= 0 && 
                            index <= Storage.Instance.users.Last().Id)
                        {
                            var tempUser = Storage.Instance.users.FirstOrDefault(item => item.Id == index);
                            if ((tempUser.Role == Role.admin || tempUser.Role == Role.headCompany) && 
                                authorizedUser.Role == Role.admin &&
                                tempUser.FullName != authorizedUser.FullName)
                            {
                                User.OnValidationError -= Helper.ShowError;
                                Helper.ShowError("An administrator cannot edit or delete information about another administrator or head of the company.");
                                return;
                            }

                            var userPaa = Storage.Instance.context.UserPaas
                                .FirstOrDefault(u => u.UserId == index);

                            //Редагування
                            if (comboBoxUserOperationType.SelectedIndex == 1)
                            {
                                User? user = GetUserData();

                                if (authorizedUser.Id == index && user != null)
                                {
                                    if (user.Status == Status.passive || user.Status == Status.blocked ||
                                        user.EndDate != null)
                                    {
                                        User.OnValidationError -= Helper.ShowError;
                                        Helper.ShowError("You cannot change your status or end date yourself.");
                                        return;
                                    }
                                    else if (authorizedUser.Role != user.Role)
                                    {
                                        User.OnValidationError -= Helper.ShowError;
                                        Helper.ShowError("You cannot change your role yourself.");
                                        return;
                                    }
                                }

                                if (user != null && user.Status == Status.passive && user.EndDate == null)
                                {
                                    User.OnValidationError -= Helper.ShowError;
                                    Helper.ShowError("You indicated the status - passive. Choose an end date.");
                                    return;
                                }

                                if (user != null && User.isCorrectValues == 7)
                                {
                                    user.Id = index;

                                    var existingUser = Storage.Instance.users.FirstOrDefault(p => p.Id == user.Id);
                                    string str = $"{existingUser.Id}, {existingUser.Name}, {existingUser.Surname}, {existingUser.Patronymic}, {existingUser.Login}, {existingUser.EncryptedPassword}, {existingUser.Role},\n{existingUser.Phone}, {existingUser.Address}, {existingUser.SectionData}, {existingUser.PositionData}, {existingUser.Status}, {existingUser.StartDate}, {existingUser.EndDate}";

                                    if (existingUser != null)
                                    {
                                        if (user.EndDate != null &&
                                            user.Status == Status.active &&
                                            existingUser.Status == Status.passive)
                                        {
                                            user.EndDate = null;
                                        }
                                        else if (user.EndDate != null &&
                                            user.Status == Status.active &&
                                            existingUser.Status == Status.active)
                                        {
                                            user.Status = Status.passive;
                                        }

                                        bool hasChanges = existingUser.Name != user.Name ||
                                            existingUser.Surname != user.Surname ||
                                            existingUser.Patronymic != user.Patronymic ||
                                            existingUser.Login != user.Login ||
                                            existingUser.Password != user.Password ||
                                            existingUser.Phone != user.Phone ||
                                            existingUser.Address != user.Address ||
                                            existingUser.Role != user.Role ||
                                            existingUser.section != user.section ||
                                            existingUser.position != user.position ||
                                            existingUser.StartDate != user.StartDate ||
                                            existingUser.EndDate != user.EndDate ||
                                            existingUser.Status != user.Status;

                                        if (hasChanges)
                                        {
                                            existingUser.Name = user.Name;
                                            existingUser.Surname = user.Surname;
                                            existingUser.Patronymic = user.Patronymic;
                                            existingUser.Login = user.Login;
                                            existingUser.Password = user.Password;
                                            existingUser.EncryptedPassword = user.EncryptedPassword;
                                            existingUser.Phone = user.Phone;
                                            existingUser.Address = user.Address;
                                            existingUser.Role = user.Role;
                                            existingUser.section = user.section;
                                            existingUser.position = user.position;
                                            existingUser.StartDate = user.StartDate;
                                            existingUser.EndDate = user.EndDate;
                                            existingUser.Status = user.Status;

                                            var position = Storage.Instance.context.Positions
                                                .FirstOrDefault(p => p.PositionId == user.position.Id);
                                            var section = Storage.Instance.context.Sections
                                                .FirstOrDefault(s => s.SectionId == user.section.Id);

                                            userPaa.Name = user.Name ?? userPaa.Name;
                                            userPaa.Surname = user.Surname ?? userPaa.Surname;
                                            userPaa.Patronymic = user.Patronymic ?? userPaa.Patronymic;
                                            userPaa.Login = user.Login ?? userPaa.Login;
                                            userPaa.Password = user.EncryptedPassword;
                                            userPaa.Phone = user.Phone ?? userPaa.Phone;
                                            userPaa.Address = user.Address ?? userPaa.Address;
                                            userPaa.Role = user.Role.ToString();
                                            userPaa.StartDate = user.StartDate ?? userPaa.StartDate;
                                            userPaa.EndDate = user.EndDate;
                                            userPaa.Status = user.Status.ToString();
                                            userPaa.Position = position;
                                            userPaa.Section = section;
                                            Storage.Instance.context.SaveChanges();

                                            Helper.ShowMessage("The user has been edited.");
                                            Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.editing, $"{authorizedUser.FullName} edited a user\nfrom \"{str}\" to\n\"{user.Id}, {user.Name}, {user.Surname}, {user.Patronymic}, {user.Login}, {user.EncryptedPassword}, {user.Role}\n{user.Phone}, {user.Address}, {user.SectionData}, {user.PositionData}, {user.Status}, {user.StartDate}, {user.EndDate}\".", DateTime.Now, authorizedUser);
                                            Storage.Instance.transactions.Add(transaction);

                                            Helper.SaveTransactionDB(transaction);

                                            dataGridUsers.ItemsSource = null;
                                            dataGridUsers.ItemsSource = Storage.Instance.users;
                                            searchUserFrame.textBoxSearch.Clear();
                                            ClearUserFields();
                                        }
                                        else
                                        {
                                            Helper.ShowMessage("No changes detected.");
                                        }
                                    }
                                }

                                User.isCorrectValues = 0;
                            }

                            //Видалення
                            else if (comboBoxUserOperationType.SelectedIndex == 2)
                            {
                                if (authorizedUser.Id == index)
                                {
                                    User.OnValidationError -= Helper.ShowError;
                                    Helper.ShowError("You cannot delete yourself.");
                                    return;
                                }

                                foreach (var project in Storage.Instance.projects)
                                {
                                    if (project.headProject.Id == index)
                                    {
                                        User.OnValidationError -= Helper.ShowError;
                                        User user = Storage.Instance.users.FirstOrDefault(u => u.Id == index);
                                        Helper.ShowMessage($"Change the head of the project ({user.Id} {user.FullName}) in projects.");
                                        return;
                                    }
                                }

                                string str = $"{tempUser.Id}, {tempUser.FullName}";

                                Storage.Instance.participations.RemoveAll(item => item.user.Id == index);
                                var participationsDB = Storage.Instance.context.Participations
                                    .Where(p => p.UserId == index)
                                    .ToList();
                                foreach (var part in participationsDB)
                                {
                                    Transaction transactionP = new(Helper.GetIdTransaction(), Enums._Type.removal, $"{authorizedUser.FullName} deleted a participation\n\"{part.ParticipationId}\".", DateTime.Now, authorizedUser);
                                    Storage.Instance.transactions.Add(transactionP);
                                    Helper.SaveTransactionDB(transactionP);
                                }
                                Storage.Instance.context.Participations.RemoveRange(participationsDB);

                                var itemsToRemove = Storage.Instance.transactions
                                    .Where(item => item.user.Id == index)
                                    .ToList();
                                foreach (var item in itemsToRemove)
                                {
                                    Storage.Instance.transactions.Remove(item);

                                    var transactionDB = Storage.Instance.context.TransactionPaas
                                        .FirstOrDefault(t => t.TransactionId == item.Id);
                                    Storage.Instance.context.TransactionPaas.Remove(transactionDB);
                                }

                                Storage.Instance.states.RemoveAll(item => item.user.Id == index);
                                var statesDB = Storage.Instance.context.States
                                    .Where(s => s.UserId == index)
                                    .ToList();
                                foreach (var state in statesDB)
                                {
                                    Transaction transactionS = new(Helper.GetIdTransaction(), Enums._Type.removal, $"{authorizedUser.FullName} deleted a state\n\"{state.StateId}, {state.Description}\".", DateTime.Now, authorizedUser);
                                    Storage.Instance.transactions.Add(transactionS);
                                    Helper.SaveTransactionDB(transactionS);
                                }
                                Storage.Instance.context.States.RemoveRange(statesDB);

                                Storage.Instance.users.RemoveAll(item => item.Id == index);
                                Storage.Instance.context.UserPaas.Remove(userPaa);
                                Storage.Instance.context.SaveChanges();

                                Helper.ShowMessage("The user has been deleted.");

                                Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.removal, $"{authorizedUser.FullName} deleted a user\n\"{str}\".", DateTime.Now, authorizedUser);
                                Storage.Instance.transactions.Add(transaction);
                                Helper.SaveTransactionDB(transaction);

                                dataGridUsers.ItemsSource = null;
                                dataGridUsers.ItemsSource = Storage.Instance.users;

                                searchUserFrame.textBoxSearch.Clear();
                                ClearUserFields();
                            }
                        }
                        else
                            Helper.ShowError("No such user exists.");
                    }
                    else
                        Helper.ShowError("Select a user.");
                }

                User.OnValidationError -= Helper.ShowError;

            }
            else Helper.ShowError("Fill in the blanks.");
        }
        private User? GetUserData()
        {
            string newPhone = "";
            if (comboBoxPhoneType.SelectedIndex == 0)
                newPhone += "+380 ";
            else
                newPhone += "+1 ";
            newPhone += textBoxPhone.Text;

            Role newRole;
            if (comboBoxRoleType.SelectedIndex == 0)
                newRole = Role.headProject;
            else if (comboBoxRoleType.SelectedIndex == 1)
                newRole = Role.employee;
            else if (comboBoxRoleType.SelectedIndex == 2)
                newRole = Role.admin;
            else
                newRole = Role.headCompany;

            Status newStatus;
            if (comboBoxUserStatusType.SelectedIndex == 0)
                newStatus = Status.active;
            else if (comboBoxUserStatusType.SelectedIndex == 1)
                newStatus = Status.passive;
            else
                newStatus = Status.blocked;

            string input = searchSectionFrame.textBoxSearch.Text;
            _Section? section = Helper.ParseToObject(input, typeof(_Section)) as _Section;
            if (section != null && _Section.isCorrectValues == 1)
            {
                existingSection = Storage.Instance.sections.FirstOrDefault(p => p.Id == section.Id && p.Name == section.Name);
                _Section.isCorrectValues = 0;
            }

            if (section != null)
            {
                input = searchPositionFrame.textBoxSearch.Text;
                Classes.Position? position = Helper.ParseToObject(input, typeof(Classes.Position)) as Classes.Position;
                if (position != null && Classes.Position.isCorrectValues == 1)
                {
                    existingPosition = Storage.Instance.positions.FirstOrDefault(p => p.Id == position.Id && p.Name == position.Name);
                    Classes.Position.isCorrectValues = 0;
                }
            }

            User? user = null;

            if (existingSection != null && existingPosition != null)
            {
                user = new User();

                user.Name = textBoxName.Text; // 01
                if (User.isCorrectValues == 1)
                    user.Surname = textBoxSurname.Text; // 02
                if (User.isCorrectValues == 2)
                    user.Patronymic = textBoxPatronymic.Text; // 03
                if (User.isCorrectValues == 3)
                {
                    user.Login = textBoxLogin.Text; // 04
                }
                if (User.isCorrectValues == 4)
                {
                    user.Password = textBoxPassword.Password; // 05
                    if (User.isCorrectValues == 5)
                        user.EncryptedPassword = user.EncryptPassword(user.Password);
                }
                if (User.isCorrectValues == 5)
                    user.Phone = newPhone; // 06
                if (User.isCorrectValues == 6)
                    user.Address = textBoxAddress.Text; // 07
                user.Role = newRole;
                user.section = existingSection;
                user.position = existingPosition;
                user.StartDate = dateFrame.startDate.SelectedDate ?? DateTime.Now; // Якщо дата не вибрана, використовуємо поточну дату;
                user.EndDate = dateFrame.endDate.SelectedDate;
                user.Status = newStatus;
            }
            else
                Helper.ShowError("No such section or position exists.");

            return user;
        }

        private void comboBoxUserOperationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                searchUserFrame.textBoxSearch.Clear();
                ClearUserFields();

                if (comboBoxUserOperationType.SelectedIndex == 0)
                {
                    EnableForUserDetails(true);
                    frameUser.IsEnabled = false;
                    dateFrame.enable = false;
                    dateFrame.endDate.IsEnabled = false;

                    ProjectsPage.UpdateComboBoxItemsVisibility(comboBoxUserStatusType, item =>
                        item.Content.ToString() == "Active");
                    ProjectsPage.CheckNumberVisibleObjects(comboBoxUserStatusType);

                    if (role == "admin")
                    {
                        ProjectsPage.UpdateComboBoxItemsVisibility(comboBoxRoleType, item =>
                            item.Content.ToString() != "Administrator" &&
                            item.Content.ToString() != "The head of the company");
                        ProjectsPage.CheckNumberVisibleObjects(comboBoxRoleType);
                    }
                }
                else if (comboBoxUserOperationType.SelectedIndex == 1)
                {
                    EnableForUserDetails(true);
                    frameUser.IsEnabled = true;
                    dateFrame.enable = true;
                    dateFrame.endDate.IsEnabled = true;

                    ProjectsPage.UpdateComboBoxItemsVisibility(comboBoxRoleType, item => true);
                    ProjectsPage.UpdateComboBoxItemsVisibility(comboBoxUserStatusType, item => true);
                }
                else if (comboBoxUserOperationType.SelectedIndex == 2)
                {
                    EnableForUserDetails(false);
                    frameUser.IsEnabled = true;
                }
            }
        }
        private void EnableForUserDetails(bool enable)
        {
            textBoxLogin.IsEnabled = enable;
            textBoxPassword.IsEnabled = enable;
            textBoxPhone.IsEnabled = enable;
            comboBoxPhoneType.IsEnabled = enable;
            textBoxAddress.IsEnabled = enable;
            comboBoxRoleType.IsEnabled = enable;
            frameSection.IsEnabled = enable;
            framePosition.IsEnabled = enable;
            textBoxName.IsEnabled = enable;
            textBoxSurname.IsEnabled = enable;
            textBoxPatronymic.IsEnabled = enable;
            comboBoxUserStatusType.IsEnabled = enable;
            frameDate.IsEnabled = enable;
        }
        private void ClearUserFields()
        {
            textBoxLogin.Clear();
            textBoxPassword.Clear();
            textBoxPhone.Clear();
            textBoxAddress.Clear();
            searchSectionFrame.textBoxSearch.Clear();
            searchPositionFrame.textBoxSearch.Clear();
            textBoxName.Clear();
            textBoxSurname.Clear();
            textBoxPatronymic.Clear();
            dateFrame.startDate.SelectedDate = null;
            dateFrame.endDate.SelectedDate = null;
        }
    }
}
