using PAA.Enums;
using PAA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PAA.Classes
{
    public class Transaction: IGeneral, INotice
    {
        private int id;
        private string? description;
        public User? user;
        private DateTime? date;
        private _Type type;

        public static short isCorrectValues = 0;

        public delegate void Handler(string message);
        public static event Handler? OnValidationError;

        public Transaction() { }
        public Transaction(int id, _Type type, string description, DateTime date, User user)
        {
            Id = id;
            Type = type;
            Description = description;
            Date = date;
            this.user = user;
        }
        public int Id
        {
            get => id;
            set => id = value;
        }

        public string? Description
        {
            get => description;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (value.Length < 3 || value.Length > 10000)
                    {
                        OnValidationError?.Invoke("Description length must be between 3 and 255.");
                        return;
                    }
                }

                description = value;
                isCorrectValues++;
            }
        }
        public DateTime? Date
        {
            get => date;
            set
            {
                date = value;
            }
        }
        public _Type Type
        {
            get => type;
            set => type = value;
        }
        public string UserData
        {
            get => $"{user.Id} {user.FullName}";
        }
    }
}
