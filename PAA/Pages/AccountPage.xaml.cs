using PAA.Classes;
using PAA.Enums;
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
    public partial class AccountPage : Page
    {
        User? authorizedUser;
        public AccountPage(User? authorizedUser)
        {
            InitializeComponent();
            this.authorizedUser = authorizedUser;

            textBlockFullName.Text += $" {authorizedUser.FullName}";
            textBlockLogin.Text += $" {authorizedUser.Login}";
            textBlockPhone.Text += $" {authorizedUser.Phone}";
            textBlockAddress.Text += $" {authorizedUser.Address}";
            textBlockSection.Text += $" {authorizedUser.section.Name}";
            textBlockPosition.Text += $" {authorizedUser.position.Name}";
            textBlockStartDate.Text += $" {authorizedUser.StartDate.GetValueOrDefault().ToShortDateString():dd.MM.yyyy}";

            if (authorizedUser.Role == Role.headProject)
            {
                textBlockRole.Text += $" The head of the projects";
            }
            else if (authorizedUser.Role == Role.employee)
            {
                textBlockRole.Text += $" Employee";
            }
            else if (authorizedUser.Role == Role.admin)
            {
                textBlockRole.Text += $" Administrator";
            }
            else
            {
                textBlockRole.Text += $" The head of the company";
            }

            if (authorizedUser.Role == Role.headProject)
            {
                var openProjects = Storage.Instance.projects
                    .Where(p => p.headProject.Id == authorizedUser.Id && p.ExecutionStatus == ExecutionStatus.open)
                    .ToList();
                textBlockOpenProjects.Text += $" {openProjects.Count}";

                var closedProjects = Storage.Instance.projects
                    .Where(p => p.headProject.Id == authorizedUser.Id && p.ExecutionStatus == ExecutionStatus.closed)
                    .ToList();
                textBlockClosedProjects.Text += $" {closedProjects.Count}";
            }
            else
            {
                var openProjects = Storage.Instance.projects
                        .Where(s => Storage.Instance.participations.Any(p => p.user.Id == authorizedUser.Id && p.project.ExecutionStatus == ExecutionStatus.open))
                        .ToList();
                textBlockOpenProjects.Text += $" {openProjects.Count}";

                var closedProjects = Storage.Instance.projects
                        .Where(s => Storage.Instance.participations.Any(p => p.user.Id == authorizedUser.Id && p.project.ExecutionStatus == ExecutionStatus.closed))
                        .ToList();
                textBlockClosedProjects.Text += $" {closedProjects.Count}";
            }

            var totalOpenProjects = Storage.Instance.projects
                .Where(p => p.ExecutionStatus == ExecutionStatus.open)
                .ToList();
            textBlockTotalOpenProjects.Text += $" {totalOpenProjects.Count}";

            var totalClosedProjects = Storage.Instance.projects
                .Where(p => p.ExecutionStatus == ExecutionStatus.closed)
                .ToList();
            textBlockTotalClosedProjects.Text += $" {totalClosedProjects.Count}";

            textBlockTotalStates.Text += $" {Storage.Instance.states.ToList().Count}";
            textBlockTotalUsers.Text += $" {Storage.Instance.users.ToList().Count}";
            textBlockTotalSections.Text += $" {Storage.Instance.sections.ToList().Count}";
            textBlockTotalPositions.Text += $" {Storage.Instance.positions.ToList().Count}";
        }
        private void buttonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (!Helper.IsValidInput(textBoxCurrentPassword.Password) ||
                !Helper.IsValidInput(textBoxNewPassword.Password))
            {
                Helper.ShowError("The entered text contains prohibited SQL statements.");
                return;
            }

            User.OnValidationError += Helper.ShowError;

            User currentUser = new(textBoxCurrentPassword.Password);
            User newUser = new(textBoxNewPassword.Password);

            User.OnValidationError -= Helper.ShowError;

            if (User.isCorrectValues == 2)
            {
                var existingUser = Storage.Instance.users.FirstOrDefault(p => p.Id == authorizedUser.Id);
                string str = $"{existingUser.EncryptedPassword}";
                if (existingUser != null)
                {
                    if (existingUser.Password != currentUser.Password)
                    {
                        Helper.ShowError("The current password is incorrect.");
                        return;
                    }

                    existingUser.Password = newUser.Password;
                    existingUser.EncryptedPassword = existingUser.EncryptPassword(existingUser.Password);

                    var userPaa = Storage.Instance.context.UserPaas
                        .FirstOrDefault(u => u.UserId == authorizedUser.Id);
                    userPaa.Password = existingUser.EncryptedPassword;
                    Storage.Instance.context.SaveChanges();

                    Helper.ShowMessage("The password has been changed.");

                    Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.editing, $"{authorizedUser.FullName} changed password\nfrom \"{str}\" to\n\"{existingUser.EncryptedPassword}\".", DateTime.Now, authorizedUser);
                    Storage.Instance.transactions.Add(transaction);

                    Helper.SaveTransactionDB(transaction);

                    textBoxCurrentPassword.Clear();
                    textBoxNewPassword.Clear();
                }
            }

            User.isCorrectValues = 0;
        }
    }
}
