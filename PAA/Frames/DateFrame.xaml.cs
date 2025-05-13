using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PAA.Frames
{
    /// <summary>
    /// Interaction logic for DateFrame.xaml
    /// </summary>
    public partial class DateFrame : Page
    {
        public bool enable = true;
        public DateFrame(string page = "")
        {
            InitializeComponent();
            if (page != "search")
                startDate.DisplayDateStart = DateTime.Now;
        }
        public static void UpdateEndDate(DatePicker startDate, DatePicker endDate)
        {
            if (startDate.SelectedDate.HasValue)
            {
                endDate.IsEnabled = true;

                // Встановлюємо мінімальну дозволену дату для endDate, виходячи з вибраної дати початку
                endDate.DisplayDateStart = startDate.SelectedDate.Value;

                // Якщо дата кінця вибрана і менша за дату початку, скидаємо її
                if (endDate.SelectedDate.HasValue && endDate.SelectedDate < startDate.SelectedDate)
                {
                    endDate.SelectedDate = startDate.SelectedDate.Value;
                }
            }
        }
        private void startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEndDate(startDate, endDate);
            if (!enable)
                endDate.IsEnabled = false;
        }
    }
}
