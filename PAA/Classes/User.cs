using Org.BouncyCastle.Crypto;
using PAA.Enums;
using PAA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace PAA.Classes
{
    public class User : IGeneral
    {
        private int id;
        private string? name;
        private string? surname;
        private string? patronymic;
        private string? login;
        private string? password;
        private string? encryptedPassword;
        private string? phone;
        private string? address;
        private Role role;
        public _Section? section;
        public Position? position;
        private DateTime? startDate;
        private DateTime? endDate;
        private Status status;

        public static short isCorrectValues = 0;

        public delegate void Handler(string message);
        public static event Handler? OnValidationError;

        public static string? publicKey;
        public static string? privateKey;
        public User() { }
        public User(string? password)
        {
            Password = password;
        }
        public User(int id, string? name, string? surname, string? patronymic)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
        }
        public int Id
        {
            get => id;
            set => id = value;
        }

        public string? Name
        {
            get => name;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (value.Length < 3 || value.Length > 20)
                    {
                        OnValidationError?.Invoke("Name length must be between 3 and 20.");
                        return;
                    }
                    else if (!value.All(char.IsLetter))
                    {
                        OnValidationError?.Invoke("Name can only contain letters.");
                        return;
                    }
                }

                name = value;
                isCorrectValues++;
            }
        }
        public string? Surname
        {
            get => surname;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && (value.Length < 3 || value.Length > 20))
                {
                    OnValidationError?.Invoke("Surname length must be between 3 and 20.");
                    return;
                }
                else if (!value.All(char.IsLetter))
                {
                    OnValidationError?.Invoke("Surname can only contain letters.");
                    return;
                }

                surname = value;
                isCorrectValues++;
            }
        }
        public string? Patronymic
        {
            get => patronymic;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && (value.Length < 3 || value.Length > 20))
                {
                    OnValidationError?.Invoke("Patronymic length must be between 3 and 20.");
                    return;
                }
                else if (!value.All(char.IsLetter))
                {
                    OnValidationError?.Invoke("Patronymic can only contain letters.");
                    return;
                }

                patronymic = value;
                isCorrectValues++;
            }
        }
        public string? Login
        {
            get => login;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && (value.Length < 8 || value.Length > 20))
                {
                    OnValidationError?.Invoke("Login length must be between 8 and 20.");
                    return;
                }

                login = value;
                isCorrectValues++;
            }
        }
        public string? Password
        {
            get => password;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && (value.Length < 8 || value.Length > 16))
                {
                    OnValidationError?.Invoke("Password length must be between 8 and 16.");
                    return;
                }

                password = value;
                isCorrectValues++;
            }
        }
        public string? EncryptedPassword
        {
            get => encryptedPassword;
            set => encryptedPassword = value;
        }
        public string? Phone
        {
            get => phone;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\d\s\+]+$"))
                    {
                        OnValidationError?.Invoke("Phone number can only contain digits, spaces, or the plus sign.");
                        return;
                    }

                    string digitsOnly = new string(value.Where(c => char.IsDigit(c)).ToArray());

                    if (value[1] == '3')
                    {
                        if (value.Length <= 17)
                        {
                            if (digitsOnly.Length != 13)
                            {
                                OnValidationError?.Invoke("The phone number must contain no more than 10 digits.");
                                return;
                            }
                        }
                        else
                        {
                            OnValidationError?.Invoke("Incorrect phone number input format.");
                            return;
                        }
                    }
                    else if (value[1] == '1')
                    {
                        if (value.Length <= 16)
                        {
                            if (digitsOnly.Length != 12)
                            {
                                OnValidationError?.Invoke("The phone number must contain no more than 11 digits.");
                                return;
                            }
                        }
                        else
                        {
                            OnValidationError?.Invoke("Incorrect phone number input format.");
                            return;
                        }
                    }
                }

                phone = value;
                isCorrectValues++;
            }
        }
        public string? Address
        {
            get => address;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && (value.Length < 3 || value.Length > 50))
                {
                    OnValidationError?.Invoke("Address length must be between 3 and 50.");
                    return;
                }

                address = value;
                isCorrectValues++;
            }
        }
        public Role Role
        {
            get => role;
            set => role = value;
        }
        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
            }
        }
        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
            }
        }
        public Status Status
        {
            get => status;
            set => status = value;
        }
        public string FullName
        {
            get => $"{Name} {Surname} {Patronymic}";
        }
        public string PositionData // використовується при виведенні даних у datagrid
        {
            get => $"{position.Id} {position.Name}";
        }
        public string SectionData
        {
            get => $"{section.Id} {section.Name}";
        }
        public short NumberFailedAttempts { get; set; } = 0;

        public string EncryptPassword(string password)
        {
            using (RSA rsaEncryptor = RSA.Create())
            {
                rsaEncryptor.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
                byte[] encryptedData = rsaEncryptor.Encrypt(Encoding.UTF8.GetBytes(password), RSAEncryptionPadding.OaepSHA256);
                return Convert.ToBase64String(encryptedData);
            }
        }
        public string DecryptPassword(string encryptedPassword)
        {
            using (RSA rsaDecryptor = RSA.Create())
            {
                rsaDecryptor.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                byte[] decryptedData = rsaDecryptor.Decrypt(Convert.FromBase64String(encryptedPassword), RSAEncryptionPadding.OaepSHA256);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }
        public int CheckUserStatus(User user)
        {
            if (user.Status == Enums.Status.passive || user.Status == Enums.Status.blocked)
            {
                OnValidationError?.Invoke($"The user has {user.Status} status.");
                return 0;
            }
            else return 1;
        }
        public User? LogIn(string login, string password)
        {
            // Перевіряємо, чи існує користувач із введеним логіном
            User? user = Storage.Instance.users.FirstOrDefault(u => u.Login == login);

            if (user == null)
            {
                OnValidationError?.Invoke("User with this login does not exist.");
                return null;
            }
            else if (CheckUserStatus(user) == 0)
            {
                return null;
            }

            // Розшифровуємо збережений пароль
            string decryptedPassword = user.DecryptPassword(user.EncryptedPassword);

            // Порівнюємо введений пароль із розшифрованим
            if (password == decryptedPassword)
            {
                return user; // Успішний вхід
            }
            else
            {
                OnValidationError?.Invoke("Invalid password.");
                return null;
            }
        }
    }
}
