using PAA.Enums;
using PAA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAA.Classes
{
    public class Project: IGeneral
    {
        private int id;
        private string? name;
        public User? headProject;
        private DateTime? startDate;
        private DateTime? expectedEndDate;
        private DateTime? actualEndDate;
        private ExecutionStatus executionStatus;

        public static short isCorrectValues = 0;

        public delegate void Handler(string message);
        public static event Handler? OnValidationError;

        public Project() { }
        public Project(int id, string? name)
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
                    if (value.Length < 3 || value.Length > 20)
                    {
                        OnValidationError?.Invoke("Name length must be between 3 and 20.");
                        return;
                    }
                }

                name = value;
                isCorrectValues++;
            }
        }
        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
            }
        }
        public DateTime? ExpectedEndDate
        {
            get => expectedEndDate;
            set
            {
                expectedEndDate = value;
            }
        }
        public DateTime? ActualEndDate
        {
            get => actualEndDate;
            set
            {
                actualEndDate = value;
            }
        }

        public ExecutionStatus ExecutionStatus
        {
            get => executionStatus;
            set => executionStatus = value;
        }
        public string HeadProjectData
        {
            get => $"{headProject.Id} {headProject.FullName}";
        }
        public int CheckProjectStatus(Project project)
        {
            if (project.ExecutionStatus == Enums.ExecutionStatus.closed)
            {
                OnValidationError?.Invoke($"The project has {project.ExecutionStatus} status.");
                return 0;
            }
            else return 1;
        }
    }
}
