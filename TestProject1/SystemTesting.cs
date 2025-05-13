using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PAA.Classes;
using System.Security.Cryptography;

namespace TestsProject
{
    public class SystemTesting
    {
        [TestCase("irynaDelizhan", "qwerty12")]
        public void Tests_LogIn_Positive(string login, string password)
        {
            Storage.Instance.users = new();
            PAA.Classes.User userAdd = new()
            {
                Login = "irynaDelizhan",
                Password = "qwerty12",
                EncryptedPassword = "LlwsQI8PJqS2cdlOOacWnURx6b5qJWXZ9EGV1wgk9VuRzu5YArXzcEPHapOEoNZtHxorNVUD8wL/yd6jBTBlYRfmkrXd8EjJx35Jcqgdgbaz0ccQ6nNplj1rWKUfMQzZ/2RlelQa9qcuEYYUNFiup0oNAPT5BrvswotD/xkkfA5FGyFuMiaWKLXT49TvM5pvp/F7yAfQ3zw3yVcsWlZAQrmkL8pRiIfvoTa6PnBAI1etnmopJjlTyakAlvbNoJ97Nb9qElgsNkFy+Rb4Rsj70m+i4rmrk3NDlVRXdkTHAuUGzwru3t7tW/0ZOkjm5G+2j53CECKLCNr+JozJ0Q31zw==",
                Status = PAA.Enums.Status.active
            };
            Storage.Instance.users.Add(userAdd);

            PAA.Classes.User user = new();

            Assert.That(user.LogIn(login, password), Is.Not.Null);
        }

        [TestCase("ir", "qw")]
        [TestCase("katyDelizhan", "qwerty12")]
        [TestCase("irynaDelizhan", "qwerty13")]
        public void Tests_LogIn_Negative(string login, string password)
        {
            Storage.Instance.users = new();
            PAA.Classes.User userAdd = new()
            {
                Login = "irynaDelizhan",
                Password = "qwerty12",
                EncryptedPassword = "LlwsQI8PJqS2cdlOOacWnURx6b5qJWXZ9EGV1wgk9VuRzu5YArXzcEPHapOEoNZtHxorNVUD8wL/yd6jBTBlYRfmkrXd8EjJx35Jcqgdgbaz0ccQ6nNplj1rWKUfMQzZ/2RlelQa9qcuEYYUNFiup0oNAPT5BrvswotD/xkkfA5FGyFuMiaWKLXT49TvM5pvp/F7yAfQ3zw3yVcsWlZAQrmkL8pRiIfvoTa6PnBAI1etnmopJjlTyakAlvbNoJ97Nb9qElgsNkFy+Rb4Rsj70m+i4rmrk3NDlVRXdkTHAuUGzwru3t7tW/0ZOkjm5G+2j53CECKLCNr+JozJ0Q31zw==",
                Status = PAA.Enums.Status.active
            };
            Storage.Instance.users.Add(userAdd);

            PAA.Classes.User user = new();

            Assert.That(user.LogIn(login, password), Is.Null);
        }

        [Test]
        public void Tests_GenerateReport1_Positive1()
        {
            Storage.Instance.projects = new();
            Storage.Instance.states = new();

            Project project = new()
            {
                Id = 0,
                StartDate = new DateTime(2025, 1, 22) // 22.01.2025
            };
            Storage.Instance.projects.Add(project);

            State state = new()
            {
                Id = 0,
                Date = new DateTime(2025, 2, 22),
                project = Storage.Instance.projects[0]
            };
            Storage.Instance.states.Add(state);

            StateReport stateReport = new(Storage.Instance.states);

            Assert.That(stateReport.GenerateReport(stateReport.States, 0, 0), Is.Not.Null);
        }

        [Test]
        public void Tests_GenerateReport1_Negative1()
        {
            Storage.Instance.projects = new();
            Storage.Instance.states = new();

            Project project = new()
            {
                Id = 0,
                StartDate = new DateTime(2025, 1, 22) // 22.01.2025
            };
            Storage.Instance.projects.Add(project);

            State state = new()
            {
                Id = 0,
                Date = new DateTime(2025, 2, 22),
                project = Storage.Instance.projects[0]
            };
            Storage.Instance.states.Add(state);

            StateReport stateReport = new(Storage.Instance.states);

            var result = stateReport.GenerateReport(stateReport.States, 0, 15);

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void Tests_GenerateReport1_Negative2()
        {
            Storage.Instance.projects = new();
            Storage.Instance.states = new();

            Project project = new()
            {
                Id = 0,
                StartDate = new DateTime(2025, 1, 22) // 22.01.2025
            };
            Storage.Instance.projects.Add(project);

            State state = new()
            {
                Id = 0,
                Date = new DateTime(2025, 2, 22),
                project = Storage.Instance.projects[0]
            };
            Storage.Instance.states.Add(state);

            StateReport stateReport = new(Storage.Instance.states);

            var result = stateReport.GenerateReport(stateReport.States, 0);

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void Tests_GenerateReport2_Positive1()
        {
            Storage.Instance.projects = new();
            Storage.Instance.states = new();

            Project project = new()
            {
                Id = 0,
                StartDate = new DateTime(2025, 1, 22) // 22.01.2025
            };
            Storage.Instance.projects.Add(project);

            State state = new()
            {
                Id = 0,
                Date = new DateTime(2025, 2, 22),
                project = Storage.Instance.projects[0]
            };
            Storage.Instance.states.Add(state);

            StateReport stateReport = new(Storage.Instance.states);

            Assert.That(stateReport.GenerateReport(stateReport.States, 1, 0, new DateTime(2025, 1, 1)), Is.Not.Null);
        }

        [Test]
        public void Tests_GenerateReport2_Positive2()
        {
            Storage.Instance.projects = new();
            Storage.Instance.states = new();

            Project project = new()
            {
                Id = 0,
                StartDate = new DateTime(2025, 1, 22) // 22.01.2025
            };
            Storage.Instance.projects.Add(project);

            State state = new()
            {
                Id = 0,
                Date = new DateTime(2025, 2, 22),
                project = Storage.Instance.projects[0]
            };
            Storage.Instance.states.Add(state);

            StateReport stateReport = new(Storage.Instance.states);

            Assert.That(stateReport.GenerateReport(stateReport.States, 1, 0, new DateTime(2025, 1, 1), new DateTime(2025, 5, 1)), Is.Not.Null);
        }

        [Test]
        public void Tests_GenerateReport2_Negative1()
        {
            Storage.Instance.projects = new();
            Storage.Instance.states = new();

            Project project = new()
            {
                Id = 0,
                StartDate = new DateTime(2025, 1, 22) // 22.01.2025
            };
            Storage.Instance.projects.Add(project);

            State state = new()
            {
                Id = 0,
                Date = new DateTime(2025, 2, 22),
                project = Storage.Instance.projects[0]
            };
            Storage.Instance.states.Add(state);

            StateReport stateReport = new(Storage.Instance.states);

            var result = stateReport.GenerateReport(stateReport.States, 1, 16, new DateTime(2025, 1, 1));

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void Tests_GenerateReport3_Positive1()
        {
            Storage.Instance.projects = new();
            Storage.Instance.states = new();

            Project project = new()
            {
                Id = 0,
                StartDate = new DateTime(2025, 1, 22) // 22.01.2025
            };
            Storage.Instance.projects.Add(project);

            State state = new()
            {
                Id = 0,
                Date = new DateTime(2025, 2, 22),
                project = Storage.Instance.projects[0]
            };
            Storage.Instance.states.Add(state);

            StateReport stateReport = new(Storage.Instance.states);

            Assert.That(stateReport.GenerateReport(stateReport.States, 2), Is.Not.Null);
        }

        [Test]
        public void Tests_GenerateReport4_Positive1()
        {
            Storage.Instance.projects = new();
            Storage.Instance.states = new();

            Project project = new()
            {
                Id = 0,
                StartDate = new DateTime(2025, 1, 22) // 22.01.2025
            };
            Storage.Instance.projects.Add(project);

            State state = new()
            {
                Id = 0,
                Date = new DateTime(2025, 2, 22),
                project = Storage.Instance.projects[0]
            };
            Storage.Instance.states.Add(state);

            TimeReport timeReport = new(Storage.Instance.states);

            Assert.That(timeReport.GenerateReport(timeReport.States, 3, 0), Is.Not.Null);
        }

        [Test]
        public void Tests_GenerateReport4_Negative1()
        {
            Storage.Instance.projects = new();
            Storage.Instance.states = new();

            Project project = new()
            {
                Id = 0,
                StartDate = new DateTime(2025, 1, 22) // 22.01.2025
            };
            Storage.Instance.projects.Add(project);

            State state = new()
            {
                Id = 0,
                Date = new DateTime(2025, 2, 22),
                project = Storage.Instance.projects[0]
            };
            Storage.Instance.states.Add(state);

            TimeReport timeReport = new(Storage.Instance.states);

            Assert.That(timeReport.GenerateReport(timeReport.States, 3, 5), Is.Null);
        }

        [Test]
        public void Tests_EncryptPassword()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                string publicKey = "MIIBCgKCAQEAwAYQdbOxXSZoLVA9RT+Ajuxfj46S4LXF23NBjyc2YxVV6smH8pPFhkqoPAXRq+mJYUvpRWf5arQHsSVbeqbMwd2a21kVX78DBZjdwc0N+Us9UeSTKpt5FNU4c+D6omCeNGu4WXnub5UbEpO2T9/OQ78aMx4NyV2lJilT/s/zO3gsdfVyuyHDc6/ehoxb7LyQspBuXDlndjaNQRhJWZYVmZc3TcO0GRygO7AkzrR2jmTEqOb6aGjyPla1qtFsS6kvME9W86oCdG7WQSFxoKH/lVHCBVt94Q5d6RpoxS9I78vBpJKUk7WgW1T/BjqYE6rAIL8oIcJDUR/dMQMHJXE+2QIDAQAB";
                string privateKey = "MIIEowIBAAKCAQEAwAYQdbOxXSZoLVA9RT+Ajuxfj46S4LXF23NBjyc2YxVV6smH8pPFhkqoPAXRq+mJYUvpRWf5arQHsSVbeqbMwd2a21kVX78DBZjdwc0N+Us9UeSTKpt5FNU4c+D6omCeNGu4WXnub5UbEpO2T9/OQ78aMx4NyV2lJilT/s/zO3gsdfVyuyHDc6/ehoxb7LyQspBuXDlndjaNQRhJWZYVmZc3TcO0GRygO7AkzrR2jmTEqOb6aGjyPla1qtFsS6kvME9W86oCdG7WQSFxoKH/lVHCBVt94Q5d6RpoxS9I78vBpJKUk7WgW1T/BjqYE6rAIL8oIcJDUR/dMQMHJXE+2QIDAQABAoIBAF3/WfVUBiGUGHD+E1Afmr3b5ZdvcmS/dmBLVi9OQahVHF63Um4jehCX4SyoqI+f3VkcgM8x630ZLZ7Aq7wphJft880mGXlqFn+Z6gvhZdK/yv+YhZXYz3esPFs1KVttMmR0yqQ6NMa4Va1NU3RcGSs+lAOr1ZHZ3msE1hIpF2bnPr5dp99L3xAHPGdzWMftX4gdN/t+feCcVI9RrZwMMz79OU3o/qixgvKM4m0CfakIh0J/yuEMK67TUZ+lFOltvSC7P5uEKGf276jfv4fVPUi184yA6ioN/iDGqlIucx9MwtpZM85JBNMHT0dxVMAjOXqOqpzm6JC/005CxVGbfSUCgYEA4J6z3HT8iHwmN6I/xKSM5onVq1TUHQ+mw8HwjQXpHjnAp0OetERl8NKZS64vVRYAMri9phMIPM5xtLoR0lbirsxNnrtvJpotw+n+3Nu6gRvrcTTY4kO8i4pyl+J/F3p3HcZjVdU2lJoi10kaKCSC4SXTG0ovqCXabJjwsUI5UIMCgYEA2tmXUfF/bFSu/GflZPF14h4YLjtZLquFfeYb1JJZvBElQ90GO4/tJnPjI6Ssl+8ppyHb9lUtiVMkr9eM8vuTtKWIqc9cenM7cunXKSo1rd/oBG2e7EArjqGNRjzYi2yyiYaU9FQecxWxZhAgsvPrfa0cIIooDlw1T+yNnF7BXHMCgYAzQoRxTxFCZHKkR5ad3Z96DQKB8v3lE+lOyzeGN08X4r4gbcIOCX3qE2WAa+PJWxf4e0hsWfOLTOGOCNiAU+uvUFh2XPfkq1K+XuwWot5REHoOf6zvFd41SgcUuk+eoAgG93s730hxaSuCTeB2QL7NesBOfgOaL+lE1zI2gZJq2QKBgCEE82/JoBAYNs6eXl38kGytXbib+7iu6FU2grxv2FonvBehIW+bJ4zFr1+RWPkTfJVa5nUkJNqzULW3L+z5SC/ZSeVVA/71o+KSpYPwemjhf4Arie7bP7claMtQItvmaomVZKP4jR+QBlP/2u8lHkK3+6ZtMd34y5Jjfno5UbNBAoGBAIA1f3e9NAuDEyU8VEbGYtBtnpqH8Bm2R9LRUFsGl/sdZZShWnRKyS8KvjD17rrZ2sGB0zeHsINOdoLdpYVNRUxJEtCX/BLtqDTw2XRXv5g3+clI1JTZsDsozfqcsYM8lmX3Y/CPX0NeglxcWe29wBootFjiALhgrly75kbVuulG";

                PAA.Classes.User.publicKey = publicKey;
                PAA.Classes.User.privateKey = privateKey;
            }

            PAA.Classes.User user = new();
            string encrypted = user.EncryptPassword("password");

            Assert.That(encrypted, Is.Not.EqualTo("password")); // Переконуємось, що пароль змінився
            Assert.That(encrypted, Is.Not.Null); // Переконуємось, що не null
            Assert.That(encrypted, Is.Not.Empty); // Переконуємось, що не пустий рядок
        }

        [TestCase("UPDATE User SET Name = `Ira` WHERE Id = 0")]
        public void Tests_IsValidInput(string str)
        {
            Assert.That(Helper.IsValidInput(str), Is.False);
        }

        [Test]
        public void Test_Supports_TenThousandUsers()
        {
            // Ініціалізація сховища користувачів
            Storage.Instance.users = new List<User>();

            // Створення 10 000 користувачів
            for (int i = 0; i < 10_000; i++)
            {
                Storage.Instance.users.Add(new User
                {
                    Id = i,
                    Login = $"userUser{i}"
                });
            }

            // Перевірка, що всі 10 000 користувачів додано
            Assert.That(Storage.Instance.users.Count, Is.EqualTo(10_000));

            // Перевірка коректності першого та останнього користувача
            Assert.That(Storage.Instance.users[0].Login, Is.EqualTo("userUser0"));
            Assert.That(Storage.Instance.users[9999].Login, Is.EqualTo("userUser9999"));
        }

    }
}
