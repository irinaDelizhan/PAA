using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PAA.Classes
{
    public class TimeReport: _Report
    {
        private List<State>? states;
        public override List<State>? States
        {
            get => states;
            set => states = value;
        }

        public TimeReport(List<State>? states)
        {
            States = states;
        }
        public override List<State>? GenerateReport(List<State> states, int number, int IdProject = -1, DateTime? startDate = null, DateTime? endDate = null)
        {
            var tempProject = Storage.Instance.projects
                .FirstOrDefault(item => item.Id == IdProject);
            if (tempProject == null)
                return null;

            var projectStates = states
                        .Where(s => s.project.Id == IdProject)
                        .OrderBy(s => s.Date)
                        .ToList();

            for (int i = 0; i < projectStates.Count; i++)
            {
                if (i == 0)
                {
                    if (projectStates[i].Date.HasValue && projectStates[i].project?.StartDate.HasValue == true)
                    {
                        double days = (projectStates[i].Date.Value - projectStates[i].project.StartDate.Value).TotalDays;
                        projectStates[i].Time = days > 0 ? (int)days : (int?)null;
                    }
                    else
                    {
                        projectStates[i].Time = null;
                    }
                }
                else
                {
                    if (projectStates[i].Date.HasValue && projectStates[i - 1].Date.HasValue)
                    {
                        double days = (projectStates[i].Date.Value - projectStates[i - 1].Date.Value).TotalDays;
                        projectStates[i].Time = days > 0 ? (int)days : (int?)null;
                    }
                    else
                    {
                        projectStates[i].Time = null;
                    }
                }
            }

            return projectStates;
        }
        public override PdfPTable AddDataToTable(DataGrid dataGridReports)
        {
            // Створення таблиці з потрібною кількістю колонок
            var pdfTable = new iTextSharp.text.pdf.PdfPTable(dataGridReports.Columns.Count);

            // Додаємо заголовки колонок (ігноруємо Time)
            foreach (DataGridColumn column in dataGridReports.Columns)
            {
                pdfTable.AddCell(new iTextSharp.text.Phrase(column.Header.ToString()));
            }

            // Додаємо дані з рядків
            foreach (var item in dataGridReports.Items)
            {
                if (item is State row) // Перевірка на тип State
                {
                    pdfTable.AddCell(row.Id.ToString());
                    pdfTable.AddCell(row.Description);
                    pdfTable.AddCell(row.ProjectData);
                    pdfTable.AddCell(row.UserData);
                    if (row.Date.HasValue)
                        pdfTable.AddCell(row.Date.Value.ToString("dd.MM.yyyy"));
                    else
                        pdfTable.AddCell("");
                    pdfTable.AddCell(row.Time.ToString());
                }
            }

            return pdfTable;
        }
    }
}
