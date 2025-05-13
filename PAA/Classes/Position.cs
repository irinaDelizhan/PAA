using PAA.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAA.Classes
{
    public class Position: IGeneral
    {
        private int id;
        private string? name;
        private double? salary;

        public static short isCorrectValues = 0;

        public delegate void Handler(string message);
        public static event Handler? OnValidationError;

        public Position() { }
        public Position(int id, string? name)
        {
            Id = id;
            Name = name;
        }
        public Position(int id, string? name, double? salary)
        {
            Id = id;
            Name = name;
            Salary = salary;
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
                    if (value.Length < 3 || value.Length > 50)
                    {
                        OnValidationError?.Invoke("Name length must be between 3 and 50.");
                        return;
                    }
                    else if (!value.All(c => char.IsLetter(c) || c == ' '))
                    {
                        OnValidationError?.Invoke("Name can only contain letters or spaces.");
                        return;
                    }
                }

                name = value;
                isCorrectValues++;
            }
        }
        public double? Salary
        {
            get => salary;
            set
            {
                if (value <= 0)
                {
                    OnValidationError?.Invoke("Salary must be greater than 0.");
                    return;
                }

                salary = Math.Round(value.Value, 2);
                isCorrectValues++;
            }
        }
    }
}
