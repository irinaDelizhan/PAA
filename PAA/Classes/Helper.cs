using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace PAA.Classes
{
    public static class Helper
    {
        public static void SaveTransactionDB(Classes.Transaction transaction)
        {
            var userPaa = Storage.Instance.context.UserPaas
                .FirstOrDefault(item => item.UserId == transaction.user.Id);

            Models.TransactionPaa transactionPaa = new Models.TransactionPaa
            {
                TransactionId = transaction.Id,
                Description = transaction.Description,
                User = userPaa,
                Date = transaction.Date ?? DateTime.Now,
                Type = transaction.Type.ToString()
            };

            Storage.Instance.context.TransactionPaas.Add(transactionPaa);
            Storage.Instance.context.SaveChanges();
        }
        public static bool IsValidInput(string input)
        {
            string sqlPattern = @"\b(SELECT|INSERT|UPDATE|DELETE|DROP|ALTER|EXEC|UNION|--|')\b";
            return !Regex.IsMatch(input, sqlPattern, RegexOptions.IgnoreCase);
        }
        public static int BlockUser(string login)
        {
            int index = Storage.Instance.users.FindIndex(u => u.Login == login);
            if (index != -1)
            {
                Storage.Instance.users[index].NumberFailedAttempts++;

                if (Storage.Instance.users[index].NumberFailedAttempts == 5)
                {
                    Storage.Instance.users[index].Status = Enums.Status.blocked;
                    
                    var userPaa = Storage.Instance.context.UserPaas
                        .FirstOrDefault(u => u.UserId == index);
                    userPaa.Status = Storage.Instance.users[index].Status.ToString();
                    Storage.Instance.context.SaveChanges();

                    Helper.ShowError("Your account has been blocked. You have made more than 5 failed login attempts.");

                    Transaction transaction = new(GetIdTransaction(), Enums._Type.editing, $"System block the user {Storage.Instance.users[index].FullName}.", DateTime.Now, Storage.Instance.users[index]);
                    Storage.Instance.transactions.Add(transaction);

                    SaveTransactionDB(transaction);

                    User.isCorrectValues = 0;
                    return 0;
                }
            }

            return 1;
        }
        public static int GetIdTransaction()
        {
            int newId = Storage.Instance.transactions.Count > 0
                        ? Storage.Instance.transactions.Last().Id + 1
                        : 0;

            return newId;
        }
        public static void ShowError(string message) =>
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);

        public static void ShowMessage(string message) =>
            MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        public static object? ParseToObject(string input, Type type)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                ShowError("Input cannot be empty.");
                return null;
            }

            if (type == typeof(Position) || 
                type == typeof(_Section) || 
                type == typeof(Project))
            {
                string[] parts = input.Trim().Split(' ', 2); // Розділяємо на ID та Name

                if (parts.Length < 2)
                {
                    ShowError("Invalid format. Enter the ID and name, separated by a space.");
                    return null;
                }

                if (!int.TryParse(parts[0], out int id))
                {
                    ShowError("ID must be an integer.");
                    return null;
                }

                string name = parts[1].Trim();

                if (type == typeof(Position))
                {
                    return new Position(id, name);
                }
                else if (type == typeof(_Section))
                {
                    return new _Section(id, name);
                }
                else if (type == typeof(Project))
                {
                    return new Project(id, name);
                }
            }
            else if (type == typeof(User))
            {
                string[] parts = input.Trim().Split(' ', 4);

                if (parts.Length < 4)
                {
                    ShowError("Invalid format. Enter the ID, name, surname and patronymic, separated by a space.");
                    return null;
                }

                if (!int.TryParse(parts[0], out int id))
                {
                    ShowError("ID must be an integer.");
                    return null;
                }

                string name = parts[1].Trim();
                string surname = parts[2].Trim();
                string patronymic = parts[3].Trim();

                return new User(id, name, surname, patronymic);
            }

            ShowError("Invalid type specified.");
            return null;
        }
    }
}
