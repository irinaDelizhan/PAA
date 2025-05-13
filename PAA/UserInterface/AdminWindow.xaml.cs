using PAA.Classes;
using PAA.Models;
using PAA.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PAA.UserInterface
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public string? role = null;
        User? authorizedUser;
        public List<Page> pagesList;

        public AdminWindow(string role, User? authorizedUser)
        {
            InitializeComponent();

            this.role = role;
            this.authorizedUser = authorizedUser;

            textBlockFullName.Text = authorizedUser.Name + " " + authorizedUser.Surname;

            this.Closed += (s, e) => Application.Current.Shutdown();

            pagesList = new List<Page>();

            pagesList.Add(new UsersPage(role, authorizedUser)); // 00
            pagesList.Add(new SectionsPage(authorizedUser)); // 01
            pagesList.Add(new PositionsPage(authorizedUser)); // 02
            pagesList.Add(new JournalPage()); // 03

            ProjectsPage.role = role;

            ProjectsPage openProjectsPage = new ProjectsPage("openProjects", authorizedUser);
            pagesList.Add(openProjectsPage); // 04
            ProjectsPage closedProjectsPage = new ProjectsPage("closedProjects", authorizedUser);
            pagesList.Add(closedProjectsPage); // 05

            pagesList.Add(new StatesPage(role, authorizedUser)); // 06
            pagesList.Add(new ParticipationsPage(authorizedUser)); // 07

            pagesList.Add(new TransactionsPage()); // 08
            pagesList.Add(new ReportsPage(role)); // 09
            pagesList.Add(new ProjectsPageForEmployee()); // 10

            pagesList.Add(new AccountPage(authorizedUser)); // 11


            if (role == "admin" || role == "employee")
            {
                textBlockRole.Text = char.ToUpper(role[0]) + role.Substring(1);

                if (role == "admin")
                {
                    textBlockRole.Text += "istrator";
                    VisibileForAdmin();
                    lstVI_ProjectsForEmployee.Visibility = Visibility.Collapsed;
                }
                else
                {
                    VisibileForEmployee();
                    VisibileForAdmin();
                    
                    lstVI_Participations.Visibility = Visibility.Collapsed;
                    lstVI_OpenProjects.Visibility = Visibility.Collapsed;
                    lstVI_ClosedProjects.Visibility = Visibility.Collapsed;
                }
            }
            else if (role == "headCompany")
            {
                textBlockRole.Text = "The head of the company";
                
                lstVI_ProjectsForEmployee.Visibility = Visibility.Collapsed;
            }
            else
            {
                textBlockRole.Text = "The head of projects";

                VisibileForEmployee();
                lstVI_ProjectsForEmployee.Visibility = Visibility.Collapsed;
            }

            if (role == "admin" || role == "headCompany")
                frameMenu.Content = pagesList[0];
            else if (role == "employee")
                frameMenu.Content = pagesList[10];
            else
                frameMenu.Content = pagesList[4];
        }
        private void VisibileForAdmin()
        {
            lstVI_Reports.Visibility = Visibility.Collapsed;
            lstVI_Transactions.Visibility = Visibility.Collapsed;
        }
        private void VisibileForEmployee()
        {
            lstVI_Users.Visibility = Visibility.Collapsed;
            lstVI_Sections.Visibility = Visibility.Collapsed;
            lstVI_Positions.Visibility = Visibility.Collapsed;
            lstVI_Journal.Visibility = Visibility.Collapsed;
            lstVI_Transactions.Visibility = Visibility.Collapsed;
        }

        private void buttonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            buttonOpenMenu.Visibility = Visibility.Visible;
            buttonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void buttonLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Storage.Instance.mainWindow = new MainWindow();
            Storage.Instance.mainWindow.Show();
        }

        private void buttonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            buttonOpenMenu.Visibility = Visibility.Collapsed;
            buttonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ListViewItem_MouseLeftButtonUp_Users(object sender, MouseButtonEventArgs e)
        {
            frameMenu.Content = pagesList[0];
        }

        private void ListViewItem_MouseLeftButtonUp_Sections(object sender, MouseButtonEventArgs e)
        {
            frameMenu.Content = pagesList[1];
        }

        private void ListViewItem_MouseLeftButtonUp_Positions(object sender, MouseButtonEventArgs e)
        {
            frameMenu.Content = pagesList[2];
        }

        private void ListViewItem_MouseLeftButtonUp_Journal(object sender, MouseButtonEventArgs e)
        {
            frameMenu.Content = pagesList[3];
        }

        private void ListViewItem_MouseLeftButtonUp_OpenProjects(object sender, MouseButtonEventArgs e)
        {
            frameMenu.Content = pagesList[4];
        }

        private void ListViewItem_MouseLeftButtonUp_ClosedProjects(object sender, MouseButtonEventArgs e)
        {
            frameMenu.Content = pagesList[5];
        }

        private void ListViewItem_MouseLeftButtonUp_States(object sender, MouseButtonEventArgs e)
        {
            frameMenu.Content = pagesList[6];
        }

        private void ListViewItem_MouseLeftButtonUp_Participations(object sender, MouseButtonEventArgs e)
        {
            frameMenu.Content = pagesList[7];
        }

        private void ListViewItem_MouseLeftButtonUp_Transactions(object sender, MouseButtonEventArgs e)
        {
            frameMenu.Content = pagesList[8];
        }

        private void ListViewItem_MouseLeftButtonUp_Reports(object sender, MouseButtonEventArgs e)
        {
            frameMenu.Content = pagesList[9];
        }

        private void ListViewItem_MouseLeftButtonUp_ProjectsForEmployee(object sender, MouseButtonEventArgs e)
        {
            frameMenu.Content = pagesList[10];
        }
        private void buttonAccount_Click(object sender, RoutedEventArgs e)
        {
            frameMenu.Content = pagesList[11];
        }
    }
}
