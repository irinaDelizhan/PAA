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
using static System.Collections.Specialized.BitVector32;

namespace PAA.Pages
{
    /// <summary>
    /// Interaction logic for SectionsPage.xaml
    /// </summary>
    public partial class SectionsPage : Page
    {
        private bool isInitialized = false;
        User? authorizedUser;
        SearchFrame searchFrame;

        public SectionsPage(User? authorizedUser)
        {
            InitializeComponent();
            this.authorizedUser = authorizedUser;
            
            searchFrame = new SearchFrame("section");
            frameSection.Content = searchFrame;
            searchFrame.SectionSelected += OnSectionSelected;

            dataGridSections.ItemsSource = Storage.Instance.sections;

            frameSection.IsEnabled = false;

            isInitialized = true;
        }
        private void OnSectionSelected(_Section section)
        {
            if (section != null)
            {
                textBoxSectionName.Text = section.Name;
                dataGridSections.ItemsSource = new List<_Section> { section };
            }
            else
            {
                textBoxSectionName.Clear();
                dataGridSections.ItemsSource = Storage.Instance.sections;
            }
        }
        private void buttonPerformOperationOnSection_Click(object sender, RoutedEventArgs e)
        {
            if (!Helper.IsValidInput(textBoxSectionName.Text) ||
                !Helper.IsValidInput(searchFrame.textBoxSearch.Text))
            {
                Helper.ShowError("The entered text contains prohibited SQL statements.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(textBoxSectionName.Text))
            {
                _Section.OnValidationError += Helper.ShowError;

                if (comboBoxSectionOperationType.SelectedIndex == 0)
                {
                    int newId = Storage.Instance.sections.Count > 0
                        ? Storage.Instance.sections.Last().Id + 1
                        : 0;

                    _Section section = new _Section(
                        id: newId,
                        name: textBoxSectionName.Text);

                    if (_Section.isCorrectValues == 1)
                    {
                        Storage.Instance.sections.Add(section);

                        Models.Section sectionDB = new Models.Section
                        {
                            SectionId = section.Id,
                            SectionName = section.Name
                        };
                        Storage.Instance.context.Sections.Add(sectionDB);
                        Storage.Instance.context.SaveChanges();

                        Helper.ShowMessage("The section has been added.");
                        Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.addition, $"{authorizedUser.FullName} added a section\n\"{section.Id}, {section.Name}\".", DateTime.Now, authorizedUser);
                        Storage.Instance.transactions.Add(transaction);
                        
                        Helper.SaveTransactionDB(transaction);

                        textBoxSectionName.Text = "";
                    }

                    dataGridSections.ItemsSource = null;
                    dataGridSections.ItemsSource = Storage.Instance.sections;

                    _Section.isCorrectValues = 0;
                }
                else if (comboBoxSectionOperationType.SelectedIndex == 1 ||
                    comboBoxSectionOperationType.SelectedIndex == 2)
                {
                    if (!string.IsNullOrWhiteSpace(searchFrame.textBoxSearch.Text))
                    {
                        string[] parts = searchFrame.textBoxSearch.Text.Split(' ');

                        if (parts.Length > 0 &&
                            int.TryParse(parts[0], out int index) &&
                            index >= 0 &&
                            index <= Storage.Instance.sections.Last().Id)
                        {
                            var sectionDB = Storage.Instance.context.Sections
                                .FirstOrDefault(item => item.SectionId == index);

                            //Редагування
                            if (comboBoxSectionOperationType.SelectedIndex == 1)
                            {
                                if (textBoxSectionName.Text != Storage.Instance.sections[index].Name)
                                {
                                    string str = $"{Storage.Instance.sections[index].Id}, {Storage.Instance.sections[index].Name}";
                                    Storage.Instance.sections[index].Name = textBoxSectionName.Text;

                                    if (_Section.isCorrectValues == 1)
                                    {
                                        sectionDB.SectionName = textBoxSectionName.Text ?? sectionDB.SectionName;
                                        Storage.Instance.context.SaveChanges();

                                        Helper.ShowMessage("The section has been edited.");
                                        Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.editing, $"{authorizedUser.FullName} edited a section\nfrom \"{str}\" to\n\"{Storage.Instance.sections[index].Id}, {Storage.Instance.sections[index].Name}\".", DateTime.Now, authorizedUser);
                                        Storage.Instance.transactions.Add(transaction);

                                        Helper.SaveTransactionDB(transaction);

                                        dataGridSections.ItemsSource = null;
                                        dataGridSections.ItemsSource = Storage.Instance.sections;

                                        searchFrame.textBoxSearch.Clear();
                                        textBoxSectionName.Clear();

                                        _Section.isCorrectValues = 0;
                                    }
                                }
                                else
                                    Helper.ShowMessage("The section has not been edited.");
                            }

                            //Видалення
                            else if (comboBoxSectionOperationType.SelectedIndex == 2)
                            {
                                User user = Storage.Instance.users.FirstOrDefault(u => u.SectionData == $"{Storage.Instance.sections[index].Id} {Storage.Instance.sections[index].Name}");
                                if (user != null)
                                {
                                    _Section.OnValidationError -= Helper.ShowError;
                                    Helper.ShowMessage($"Change the section ({Storage.Instance.sections[index].Id} {Storage.Instance.sections[index].Name}) in users.");
                                    return;
                                }

                                var tempSection = Storage.Instance.sections.FirstOrDefault(item => item.Id == index);
                                string str = $"{tempSection.Id}, {tempSection.Name}";

                                Storage.Instance.sections.RemoveAll(item => item.Id == index);

                                Storage.Instance.context.Sections.Remove(sectionDB);
                                Storage.Instance.context.SaveChanges();

                                Helper.ShowMessage("The section has been deleted.");

                                Transaction transaction = new(Helper.GetIdTransaction(), Enums._Type.removal, $"{authorizedUser.FullName} deleted a section\n\"{str}\".", DateTime.Now, authorizedUser);
                                Storage.Instance.transactions.Add(transaction);

                                Helper.SaveTransactionDB(transaction);

                                dataGridSections.ItemsSource = null;
                                dataGridSections.ItemsSource = Storage.Instance.sections;

                                searchFrame.textBoxSearch.Clear();
                                textBoxSectionName.Clear();
                            }
                        }
                        else
                            Helper.ShowError("No such section exists.");
                    }
                    else
                        Helper.ShowError("Select a section.");
                }

                _Section.OnValidationError -= Helper.ShowError;
            }
            else Helper.ShowError("Fill in the blanks.");
        }
        private void comboBoxSectionOperationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                textBoxSectionName.Clear();
                searchFrame.textBoxSearch.Clear();
                
                if (comboBoxSectionOperationType.SelectedIndex == 0)
                {
                    textBoxSectionName.IsEnabled = true;
                    frameSection.IsEnabled = false;
                }
                else if (comboBoxSectionOperationType.SelectedIndex == 1)
                {
                    textBoxSectionName.IsEnabled = true;
                    frameSection.IsEnabled = true;
                }
                else if (comboBoxSectionOperationType.SelectedIndex == 2)
                {
                    textBoxSectionName.IsEnabled = false;
                    frameSection.IsEnabled = true;
                }
            }
        }
    }
}
