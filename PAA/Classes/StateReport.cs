using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Documents;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace PAA.Classes
{
    public class StateReport: _Report
    {
        private List<State>? states;
        public override List<State>? States
        {
            get => states;
            set => states = value;
        }
        public StateReport(List<State>? states)
        {
            States = states;
        }
        public override PdfPTable AddDataToTable(DataGrid dataGridReports)
        {
            // Створення таблиці з потрібною кількістю колонок (без Time)
            var pdfTable = new iTextSharp.text.pdf.PdfPTable(dataGridReports.Columns.Count - 1);

            // Додаємо заголовки колонок (ігноруємо Time)
            foreach (DataGridColumn column in dataGridReports.Columns)
            {
                if (column.Header.ToString() != "Time")
                {
                    pdfTable.AddCell(new iTextSharp.text.Phrase(column.Header.ToString()));
                }
            }

            // Додаємо дані з рядків (без Time)
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
                }
            }

            return pdfTable;
        }
    }
}
