using PAA.Classes;
using PAA.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PAA.Pages
{
    /// <summary>
    /// Interaction logic for PositionsPage.xaml
    /// </summary>
    public partial class PositionsPage : Page
    {
        private bool isInitialized = false;
        SearchFrame searchFrame;
        User? authorizedUser;
        public PositionsPage(User? authorizedUser)
        {
            InitializeComponent();
            this.authorizedUser = authorizedUser;

            searchFrame = new SearchFrame("position");
            framePosition.Content = searchFrame;
            searchFrame.PositionSelected += OnPositionSelected;

            dataGridPositions.ItemsSource = Storage.Instance.positions;

            framePosition.IsEnabled = false;

            isInitialized = true;
        }
        private void OnPositionSelected(Position position)
        {
            if (position != null)
            {
                textBoxPositionName.Text = position.Name;
                textBoxSalary.Text = position.Salary.ToString();
                dataGridPositions.ItemsSource = new List<Position> { position };
            }
            else
            {
                ClearPositionFields();
                dataGridPositions.ItemsSource = Storage.Instance.positions;
            }
        }

        private void comboBoxPositionOperationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                searchFrame.textBoxSearch.Clear();
                ClearPositionFields();

                if (comboBoxPositionOperationType.SelectedIndex == 0)
                {
                    EnableForParticipationDetails(true);
                    framePosition.IsEnabled = false;
                }
                else if (comboBoxPositionOperationType.SelectedIndex == 1)
                {
                    EnableForParticipationDetails(true);
                    framePosition.IsEnabled = true;
                }
                else if (comboBoxPositionOperationType.SelectedIndex == 2)
                {
                    EnableForParticipationDetails(false);
                    framePosition.IsEnabled = true;
                }
            }
        }
        private void EnableForParticipationDetails(bool enable)
        {
            textBoxPositionName.IsEnabled = enable;
            textBoxSalary.IsEnabled = enable;
        }
        private void buttonPerformOperationOnPosition_Click(object sender, RoutedEventArgs e)
        {
            if (!Helper.IsValidInput(searchFrame.textBoxSearch.Text) ||
                !Helper.IsValidInput(textBoxPositionName.Text) ||
                !Helper.IsValidInput(textBoxSalary.Text))
            {
                Helper.ShowError("The entered text contains prohibited SQL statements.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(textBoxPositionName.Text) &&
                !string.IsNullOrWhiteSpace(textBoxSalary.Text))
            {
                Position.OnValidationError += Helper.ShowError;

                if (comboBoxPositionOperationType.SelectedIndex == 0)
                {
                    int newId = Storage.Instance.positions.Count > 0
                        ? Storage.Instance.positions.Last().Id + 1
                        : 0;

                    Position? position = GetPositionData();

                    if (position != null)
                    {
                        position.Id = newId;
                        Storage.Instance.positions.Add(position);

                        Models.Position positionDB = new Models.Position
                        {
                            PositionId = position.Id,
                            PositionName = position.Name,
                            Salary = (decimal)(position.Salary ?? 0)
                        };
                        Storage.Instance.context.Positions.Add(positionDB);
                        Storage.Instance.context.SaveChanges();

                        Helper.ShowMessage("The position has been added.");
                        Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.addition, $"{authorizedUser.FullName} added a position\n\"{position.Id}, {position.Name}, {position.Salary}\".", DateTime.Now, authorizedUser);
                        Storage.Instance.transactions.Add(transaction);

                        Helper.SaveTransactionDB(transaction);

                        ClearPositionFields();
                    }

                    dataGridPositions.ItemsSource = null;
                    dataGridPositions.ItemsSource = Storage.Instance.positions;

                    Position.isCorrectValues = 0;
                }
                else if (comboBoxPositionOperationType.SelectedIndex ==  1 ||
                    comboBoxPositionOperationType.SelectedIndex == 2)
                {
                    if (!string.IsNullOrWhiteSpace(searchFrame.textBoxSearch.Text))
                    {
                        string[] parts = searchFrame.textBoxSearch.Text.Split(' ');

                        if (parts.Length > 0 &&
                            int.TryParse(parts[0], out int index) &&
                            index >= 0 &&
                            index <= Storage.Instance.positions.Last().Id)
                        {
                            var positionDB = Storage.Instance.context.Positions
                                .FirstOrDefault(item => item.PositionId == index);

                            //Редагування
                            if (comboBoxPositionOperationType.SelectedIndex == 1)
                            {
                                Position? position = GetPositionData();

                                if (position != null)
                                {
                                    position.Id = index;
                                    var existingPosition = Storage.Instance.positions.FirstOrDefault(p => p.Id == position.Id);
                                    string str = $"{existingPosition.Id}, {existingPosition.Name}, {existingPosition.Salary}";

                                    if (existingPosition != null)
                                    {
                                        bool hasChanges = existingPosition.Name != position.Name || existingPosition.Salary != position.Salary;

                                        if (hasChanges)
                                        {
                                            existingPosition.Name = position.Name;
                                            existingPosition.Salary = position.Salary;

                                            positionDB.PositionName = position.Name ?? positionDB.PositionName;
                                            positionDB.Salary = position.Salary.HasValue ? (decimal)position.Salary.Value : positionDB.Salary;
                                            Storage.Instance.context.SaveChanges();

                                            Helper.ShowMessage("The position has been edited.");
                                            Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.editing, $"{authorizedUser.FullName} edited a position\nfrom \"{str}\" to\n\"{position.Id}, {position.Name}, {position.Salary}\".", DateTime.Now, authorizedUser);
                                            Storage.Instance.transactions.Add(transaction);

                                            Helper.SaveTransactionDB(transaction);

                                            dataGridPositions.ItemsSource = null;
                                            dataGridPositions.ItemsSource = Storage.Instance.positions;
                                            searchFrame.textBoxSearch.Clear();
                                            ClearPositionFields();
                                        }
                                        else
                                        {
                                            Helper.ShowMessage("No changes detected.");
                                        }
                                    }
                                }

                                Position.isCorrectValues = 0;
                            }

                            //Видалення
                            else if (comboBoxPositionOperationType.SelectedIndex == 2)
                            {
                                User user = Storage.Instance.users.FirstOrDefault(u => u.PositionData == $"{Storage.Instance.positions[index].Id} {Storage.Instance.positions[index].Name}");
                                if (user != null)
                                {
                                    Position.OnValidationError -= Helper.ShowError;
                                    Helper.ShowMessage($"Change the position ({Storage.Instance.positions[index].Id} {Storage.Instance.positions[index].Name}) in users.");
                                    return;
                                }

                                var tempPosition = Storage.Instance.positions.FirstOrDefault(item => item.Id == index);
                                string str = $"{tempPosition.Id}, {tempPosition.Name}";

                                Storage.Instance.positions.RemoveAll(item => item.Id == index);

                                Storage.Instance.context.Positions.Remove(positionDB);
                                Storage.Instance.context.SaveChanges();

                                Helper.ShowMessage("The position has been deleted.");

                                Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.removal, $"{authorizedUser.FullName} deleted a position\n\"{str}\".", DateTime.Now, authorizedUser);
                                Storage.Instance.transactions.Add(transaction);

                                Helper.SaveTransactionDB(transaction);

                                dataGridPositions.ItemsSource = null;
                                dataGridPositions.ItemsSource = Storage.Instance.positions;

                                searchFrame.textBoxSearch.Clear();
                                ClearPositionFields();  
                            }
                        }
                        else
                            Helper.ShowError("No such position exists.");
                    }
                    else
                        Helper.ShowError("Select a position.");
                }

                Position.OnValidationError -= Helper.ShowError;
            }
            else Helper.ShowError("Fill in the blanks.");
        }
        private Position? GetPositionData()
        {
            string salaryStr = textBoxSalary.Text.Replace('.', ',');
            if (salaryStr.Any(c => !char.IsDigit(c) && c != ',' && c != '.'))
            {
                Helper.ShowError("Salary can only contain digits, a comma, or a dot.");
                return null;
            }
            else if (salaryStr.StartsWith("0") && !salaryStr.StartsWith("0.") && !salaryStr.StartsWith("0,"))
            {
                Helper.ShowError("Salary must only contain digits and cannot start with 0 unless it's a decimal value.");
                return null;
            }
            else if (salaryStr.EndsWith(",") || salaryStr.EndsWith(".") ||
                salaryStr.StartsWith(",") || salaryStr.StartsWith("."))
            {
                Helper.ShowError("Salary cannot start or end with a comma or dot.");
                return null;
            }

            Position? position = new Position();
            position.Name = textBoxPositionName.Text;
            position.Salary = double.Parse(salaryStr);

            if (Position.isCorrectValues == 2)
                return position;
            else return null;
        }
        private void ClearPositionFields()
        {
            textBoxPositionName.Clear();
            textBoxSalary.Clear();
        }
    }
}
