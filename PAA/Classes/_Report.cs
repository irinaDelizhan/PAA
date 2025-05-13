using iTextSharp.text.pdf;
using iTextSharp.text;
using PAA.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PAA.Classes
{
    public abstract class _Report
    {
        public abstract List<State> States { get; set; }
        public abstract PdfPTable AddDataToTable(DataGrid dataGridReports);
        public virtual List<State>? GenerateReport(List<State> states, int number, int IdProject = -1, DateTime? startDate = null, DateTime? endDate = null)
        {
            switch(number)
            {
                case 0:
                    return states
                        .Where(s => s.project.Id == IdProject)
                        .ToList();

                case 1:
                    List<State> filteredStates = states
                        .Where(s => s.project.Id == IdProject)
                        .Where(s => !startDate.HasValue || s.Date.GetValueOrDefault() >= startDate.Value)
                        .Where(s => !endDate.HasValue || s.Date.GetValueOrDefault().Date <= endDate.Value.Date)
                        .ToList();
                    return filteredStates;

                case 2:
                    return states
                        .GroupBy(s => s.project?.Id) // Групуємо за ID проєкту
                        .Select(g => g.OrderByDescending(s => s.Date).FirstOrDefault()) // У кожній групі беремо запис з найпізнішою датою.
                        .Where(s => s != null) // Видаляємо можливі null
                        .ToList();

                default:
                    return null;
            }
        }
        public void DownloadReport(DataGrid dataGridReports, short number)
        {
            // Вибір шляху збереження файлу
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Збереження звіту",
                FileName = $"Report №{number + 1} {DateTime.Now.ToString("dd.MM.yyyy")}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                // Створення PDF-документу
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    var document = new iTextSharp.text.Document();
                    var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, stream);
                    document.Open();

                    // Додаємо заголовок
                    string phrase = "";
                    if (number == 0)
                        phrase = "Report \"Project statuses\"";
                    else if (number == 1)
                        phrase = "Report \"Project statuses according to dates\"";
                    else if (number == 2)
                        phrase = "Report \"The latest statuses of all projects\"";
                    else if (number == 3)
                        phrase = "Report \"Time spent on each status of the project\"";

                    var baseFont = iTextSharp.text.pdf.BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    var titleFont = new iTextSharp.text.Font(baseFont, 18, iTextSharp.text.Font.BOLD);
                    var title = new iTextSharp.text.Paragraph(phrase, titleFont)
                    {
                        Alignment = iTextSharp.text.Element.ALIGN_CENTER,
                        SpacingAfter = 20
                    };
                    document.Add(title);

                    var pdfTable = AddDataToTable(dataGridReports);

                    document.Add(pdfTable);

                    // Додаємо дату та час збереження звіту в нижньому лівому куті
                    string saveDateTime = $"Date: {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}";
                    PdfContentByte canvas = writer.DirectContent;
                    canvas.BeginText();
                    canvas.SetFontAndSize(baseFont, 10);
                    canvas.ShowTextAligned(Element.ALIGN_LEFT, saveDateTime, 40, 30, 0); // Координати (x, y) — лівий нижній кут
                    canvas.EndText();

                    document.Close();
                }
            }
        }
    }
}
