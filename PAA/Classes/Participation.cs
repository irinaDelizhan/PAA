using PAA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAA.Classes
{
    public class Participation: IGeneral
    {
        private int id;
        public User? user;
        public Project? project;
        private DateTime? startDate;
        private DateTime? endDate;

        public static short isCorrectValues = 0;

        public delegate void Handler(string message);
        public static event Handler? OnValidationError;

        public int Id
        {
            get => id;
            set => id = value;
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
        public string ProjectData
        {
            get => $"{project.Id} {project.Name}";
        }
        public string UserData
        {
            get => $"{user.Id} {user.FullName}";
        }
        public string ProjectAndUserData // для виводу в listBox
        {
            get => $"{project.Name} {user.FullName}";
        }
    }
}
