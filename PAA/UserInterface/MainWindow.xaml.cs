using PAA.Classes;
using PAA.Enums;
using PAA.Models;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PAA.UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Page> pagesList;
        User? authorizedUser;

        public MainWindow()
        {
            InitializeComponent();

            Storage.Instance.context = new();

            using (RSA rsa = RSA.Create(2048))
            {
                string publicKey = "MIIBCgKCAQEAwAYQdbOxXSZoLVA9RT+Ajuxfj46S4LXF23NBjyc2YxVV6smH8pPFhkqoPAXRq+mJYUvpRWf5arQHsSVbeqbMwd2a21kVX78DBZjdwc0N+Us9UeSTKpt5FNU4c+D6omCeNGu4WXnub5UbEpO2T9/OQ78aMx4NyV2lJilT/s/zO3gsdfVyuyHDc6/ehoxb7LyQspBuXDlndjaNQRhJWZYVmZc3TcO0GRygO7AkzrR2jmTEqOb6aGjyPla1qtFsS6kvME9W86oCdG7WQSFxoKH/lVHCBVt94Q5d6RpoxS9I78vBpJKUk7WgW1T/BjqYE6rAIL8oIcJDUR/dMQMHJXE+2QIDAQAB";
                string privateKey = "MIIEowIBAAKCAQEAwAYQdbOxXSZoLVA9RT+Ajuxfj46S4LXF23NBjyc2YxVV6smH8pPFhkqoPAXRq+mJYUvpRWf5arQHsSVbeqbMwd2a21kVX78DBZjdwc0N+Us9UeSTKpt5FNU4c+D6omCeNGu4WXnub5UbEpO2T9/OQ78aMx4NyV2lJilT/s/zO3gsdfVyuyHDc6/ehoxb7LyQspBuXDlndjaNQRhJWZYVmZc3TcO0GRygO7AkzrR2jmTEqOb6aGjyPla1qtFsS6kvME9W86oCdG7WQSFxoKH/lVHCBVt94Q5d6RpoxS9I78vBpJKUk7WgW1T/BjqYE6rAIL8oIcJDUR/dMQMHJXE+2QIDAQABAoIBAF3/WfVUBiGUGHD+E1Afmr3b5ZdvcmS/dmBLVi9OQahVHF63Um4jehCX4SyoqI+f3VkcgM8x630ZLZ7Aq7wphJft880mGXlqFn+Z6gvhZdK/yv+YhZXYz3esPFs1KVttMmR0yqQ6NMa4Va1NU3RcGSs+lAOr1ZHZ3msE1hIpF2bnPr5dp99L3xAHPGdzWMftX4gdN/t+feCcVI9RrZwMMz79OU3o/qixgvKM4m0CfakIh0J/yuEMK67TUZ+lFOltvSC7P5uEKGf276jfv4fVPUi184yA6ioN/iDGqlIucx9MwtpZM85JBNMHT0dxVMAjOXqOqpzm6JC/005CxVGbfSUCgYEA4J6z3HT8iHwmN6I/xKSM5onVq1TUHQ+mw8HwjQXpHjnAp0OetERl8NKZS64vVRYAMri9phMIPM5xtLoR0lbirsxNnrtvJpotw+n+3Nu6gRvrcTTY4kO8i4pyl+J/F3p3HcZjVdU2lJoi10kaKCSC4SXTG0ovqCXabJjwsUI5UIMCgYEA2tmXUfF/bFSu/GflZPF14h4YLjtZLquFfeYb1JJZvBElQ90GO4/tJnPjI6Ssl+8ppyHb9lUtiVMkr9eM8vuTtKWIqc9cenM7cunXKSo1rd/oBG2e7EArjqGNRjzYi2yyiYaU9FQecxWxZhAgsvPrfa0cIIooDlw1T+yNnF7BXHMCgYAzQoRxTxFCZHKkR5ad3Z96DQKB8v3lE+lOyzeGN08X4r4gbcIOCX3qE2WAa+PJWxf4e0hsWfOLTOGOCNiAU+uvUFh2XPfkq1K+XuwWot5REHoOf6zvFd41SgcUuk+eoAgG93s730hxaSuCTeB2QL7NesBOfgOaL+lE1zI2gZJq2QKBgCEE82/JoBAYNs6eXl38kGytXbib+7iu6FU2grxv2FonvBehIW+bJ4zFr1+RWPkTfJVa5nUkJNqzULW3L+z5SC/ZSeVVA/71o+KSpYPwemjhf4Arie7bP7claMtQItvmaomVZKP4jR+QBlP/2u8lHkK3+6ZtMd34y5Jjfno5UbNBAoGBAIA1f3e9NAuDEyU8VEbGYtBtnpqH8Bm2R9LRUFsGl/sdZZShWnRKyS8KvjD17rrZ2sGB0zeHsINOdoLdpYVNRUxJEtCX/BLtqDTw2XRXv5g3+clI1JTZsDsozfqcsYM8lmX3Y/CPX0NeglxcWe29wBootFjiALhgrly75kbVuulG";

                User.publicKey = publicKey;
                User.privateKey = privateKey;
            }

            Storage.Instance.users = new();
            Storage.Instance.sections = new();
            Storage.Instance.positions = new();
            Storage.Instance.projects = new();
            Storage.Instance.states = new();
            Storage.Instance.participations = new();
            Storage.Instance.transactions = new();

            Storage.Instance.sections = Storage.Instance.context.Sections
                .Select(s => new _Section
                {
                    Id = s.SectionId,
                    Name = s.SectionName
                })
                .ToList();

            Storage.Instance.positions = Storage.Instance.context.Positions
                .Select(p => new Classes.Position
                {
                    Id = p.PositionId,
                    Name = p.PositionName,
                    Salary = (double?)p.Salary
                })
                .ToList();

            User user = new User();

            var sections = Storage.Instance.sections;
            var positions = Storage.Instance.positions;

            Storage.Instance.users = Storage.Instance.context.UserPaas
                .AsEnumerable() // Переводимо запит у пам'ять
                .Select(u => new User
                {
                    Id = u.UserId,
                    Name = u.Name,
                    Surname = u.Surname,
                    Patronymic = u.Patronymic,
                    Login = u.Login,
                    EncryptedPassword = u.Password,
                    Password = user.DecryptPassword(u.Password),
                    Address = u.Address,
                    Phone = u.Phone,
                    section = sections.FirstOrDefault(item => item.Id == u.SectionId), // Пошук у пам'яті
                    position = positions.FirstOrDefault(item => item.Id == u.PositionId), // Пошук у пам'яті
                    StartDate = u.StartDate,
                    EndDate = u.EndDate,
                    Role = (Role)Enum.Parse(typeof(Role), u.Role),
                    Status = (Status)Enum.Parse(typeof(Status), u.Status)
                })
                .ToList();

            var users = Storage.Instance.users;

            Storage.Instance.transactions = new ObservableCollection<Transaction>(
                Storage.Instance.context.TransactionPaas
                    .AsEnumerable()
                    .Select(t => new Transaction
                    {
                        Id = t.TransactionId,
                        Description = t.Description,
                        Date = t.Date,
                        Type = (_Type)Enum.Parse(typeof(_Type), t.Type),
                        user = users.FirstOrDefault(item => item.Id == t.User.UserId)
                    })
                    .ToList()
            );

            Storage.Instance.projects = Storage.Instance.context.Projects
                .AsEnumerable()
                .Select(p => new Classes.Project
                {
                    Id = p.ProjectId,
                    Name = p.ProjectName,
                    StartDate = p.StartDate,
                    ExpectedEndDate = p.ExpectedEndDate,
                    ActualEndDate = p.ActualEndDate,
                    headProject = users.FirstOrDefault(item => item.Id == p.Head.UserId),
                    ExecutionStatus = (ExecutionStatus)Enum.Parse(typeof(ExecutionStatus), p.ExecutionStatus)
                })
                .ToList();

            var projects = Storage.Instance.projects;

            Storage.Instance.participations = Storage.Instance.context.Participations
                .AsEnumerable()
                .Select(p => new Classes.Participation
                {
                    Id = p.ParticipationId,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    user = users.FirstOrDefault(item => item.Id == p.User.UserId),
                    project = projects.FirstOrDefault(item => item.Id == p.Project.ProjectId)
                })
                .ToList();

            Storage.Instance.states = Storage.Instance.context.States
                .AsEnumerable()
                .Select(st => new Classes.State
                {
                    Id = st.StateId,
                    Description = st.Description,
                    Date = st.Date,
                    user = users.FirstOrDefault(item => item.Id == st.User.UserId),
                    project = projects.FirstOrDefault(item => item.Id == st.Project.ProjectId)
                })
                .ToList();

            User.isCorrectValues = 0;
            _Section.isCorrectValues = 0;
            Classes.Position.isCorrectValues = 0;
            Classes.Project.isCorrectValues = 0;
            Classes.Participation.isCorrectValues = 0;
            Classes.State.isCorrectValues = 0;
            Transaction.isCorrectValues = 0;

            this.Closed += (s, e) => Application.Current.Shutdown();
            Storage.Instance.mainWindow = this;
        }

        private void buttonEnter_Click(object sender, RoutedEventArgs e)
        {
            if (!Helper.IsValidInput(textBoxLogin.Text) ||
                !Helper.IsValidInput(textBoxPassword.Password))
            {
                Helper.ShowError("The entered text contains prohibited SQL statements.");
                return;
            }

            User.OnValidationError += Helper.ShowError;

            authorizedUser = new User();
            authorizedUser.Login = textBoxLogin.Text;
            if (User.isCorrectValues == 1)
            {
                User? user = Storage.Instance.users.FirstOrDefault(u => u.Login == textBoxLogin.Text);

                if (user != null && user.CheckUserStatus(user) == 0)
                {
                    User.isCorrectValues = 0;
                    User.OnValidationError -= Helper.ShowError;
                    return;
                }

                authorizedUser.Password = textBoxPassword.Password;

                if (User.isCorrectValues != 2)
                {
                    if (Helper.BlockUser(textBoxLogin.Text) == 0)
                    {
                        User.isCorrectValues = 0;
                        User.OnValidationError -= Helper.ShowError;
                        return;
                    }
                }
            }
            if (User.isCorrectValues == 2)
            {
                authorizedUser = authorizedUser.LogIn(authorizedUser.Login, authorizedUser.Password);

                if (authorizedUser == null)
                {
                    User? user = Storage.Instance.users.FirstOrDefault(u => u.Login == textBoxLogin.Text);

                    if (Helper.BlockUser(textBoxLogin.Text) == 0)
                    {
                        User.isCorrectValues = 0;
                        User.OnValidationError -= Helper.ShowError;
                        return;
                    }
                }
            }

            if (User.isCorrectValues == 2 && authorizedUser != null)
            {
                Storage.Instance.adminWindow = new AdminWindow(authorizedUser.Role.ToString(), authorizedUser);
                Storage.Instance.adminWindow.Show();
                Storage.Instance.mainWindow.textBoxLogin.Text = null;
                Storage.Instance.mainWindow.textBoxPassword.Password = null;
                this.Hide();
            }

            User.isCorrectValues = 0;

            User.OnValidationError -= Helper.ShowError;
        }
        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}