using PAA.Enums;
using PAA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAA.Classes
{
    public class _Section: IGeneral
    {
        private int id;
        private string? name;

        public static short isCorrectValues = 0;

        public delegate void Handler(string message);
        public static event Handler? OnValidationError;

        public _Section() { }
        public _Section(int id, string? name)
        {
            Id = id;
            Name = name;
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
                        OnValidationError?.Invoke("Name can only contain letters and spaces.");
                        return;
                    }
                }

                name = value;
                isCorrectValues++;
            }
        }
    }
}
