using PAA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PAA.Classes
{
    public class State: IGeneral, INotice
    {
        private int id;
        private string? description;
        public Project? project;
        private DateTime? date;
        public User? user;

        public static short isCorrectValues = 0;

        public delegate void Handler(string message);
        public static event Handler? OnValidationError;

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
                    if (value.Length < 3 || value.Length > 255)
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
        public string ProjectData
        {
            get => $"{project.Id} {project.Name}";
        }
        public string UserData
        {
            get => $"{user.Id} {user.FullName}";
        }
        public int? Time { get; set; }
    }
}
