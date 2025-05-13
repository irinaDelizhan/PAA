using PAA.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PAA.Frames
{
    /// <summary>
    /// Interaction logic for SearchFrame.xaml
    /// </summary>
    public partial class SearchFrame : Page
    {       
        private string page;
        public ObservableCollection<_Section> FilteredSections { get; set; } = new();
        public ObservableCollection<Position> FilteredPositions { get; set; } = new();
        public ObservableCollection<User> FilteredUsers { get; set; } = new();
        public ObservableCollection<Project> FilteredProjects { get; set; } = new();
        public ObservableCollection<State> FilteredStates { get; set; } = new();
        public ObservableCollection<Participation> FilteredParticipations { get; set; } = new();

        public event Action<_Section>? SectionSelected;
        public event Action<Position>? PositionSelected;
        public event Action<User>? UserSelected;
        public event Action<Project>? ProjectSelected;
        public event Action<State>? StateSelected;
        public event Action<Participation>? ParticipationSelected;

        public bool enable = true;
        public SearchFrame(string page)
        {
            InitializeComponent();
            DataContext = this;
            this.page = page;
            if (page == "section")
                listBoxSearch.SetBinding(ListBox.ItemsSourceProperty, new Binding("FilteredSections"));
            else if (page == "position")
                listBoxSearch.SetBinding(ListBox.ItemsSourceProperty, new Binding("FilteredPositions"));
            else if (page == "user")
                listBoxSearch.SetBinding(ListBox.ItemsSourceProperty, new Binding("FilteredUsers"));
            else if (page == "project")
                listBoxSearch.SetBinding(ListBox.ItemsSourceProperty, new Binding("FilteredProjects"));
            else if (page == "state")
                listBoxSearch.SetBinding(ListBox.ItemsSourceProperty, new Binding("FilteredStates"));
            else if (page == "participation")
                listBoxSearch.SetBinding(ListBox.ItemsSourceProperty, new Binding("FilteredParticipations"));

            SetItemTemplate(page);
        }
        private void SetItemTemplate(string page)
        {
            DataTemplate dataTemplate = new DataTemplate();
            FrameworkElementFactory stackPanel = new FrameworkElementFactory(typeof(StackPanel));
            stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock));

            Binding binding = new Binding("Id")
            {
                StringFormat = "{0} {1}",
            };

            MultiBinding multiBinding = new MultiBinding();
            multiBinding.StringFormat = "{0} {1}";
            multiBinding.Bindings.Add(new Binding("Id")); // для participation тільки це поле

            if (page == "user")
            {
                multiBinding.Bindings.Add(new Binding("FullName"));
            }
            else if (page == "position" || page == "section" || page == "project")
            {
                multiBinding.Bindings.Add(new Binding("Name"));
            }
            else if (page == "state")
            {
                multiBinding.Bindings.Add(new Binding("Description"));
            }
            else if (page == "participation")
            {
                multiBinding.Bindings.Add(new Binding("ProjectAndUserData"));
            }

            textBlock.SetBinding(TextBlock.TextProperty, multiBinding);

            stackPanel.AppendChild(textBlock);
            dataTemplate.VisualTree = stackPanel;

            listBoxSearch.ItemTemplate = dataTemplate;
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBoxSearch.Text))
            {
                PerformSearch(textBoxSearch.Text.ToLower());
            }
            else
            {
                if (page == "section")
                {
                    FilterResults(Storage.Instance.sections, FilteredSections);
                }
                else if (page == "position")
                {
                    FilterResults(Storage.Instance.positions, FilteredPositions);
                }
                else if (page == "user")
                {
                    FilterResults(Storage.Instance.users, FilteredUsers);
                }
                else if (page == "project")
                {
                    FilterResults(Storage.Instance.projects, FilteredProjects);
                }
                else if (page == "state")
                {
                    FilterResults(Storage.Instance.states, FilteredStates);
                }
                else if (page == "participation")
                {
                    FilterResults(Storage.Instance.participations, FilteredParticipations);
                }
            }
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!enable)
                return;

            string searchText = (sender as TextBox).Text.ToLower();
            PerformSearch(searchText);

            if (string.IsNullOrWhiteSpace(searchText))
            {
                if (page == "section")
                    SectionSelected?.Invoke(null); // Очистити вибір
                else if (page == "position")
                    PositionSelected?.Invoke(null);
                else if (page == "user")
                    UserSelected?.Invoke(null);
                else if (page == "project")
                    ProjectSelected?.Invoke(null);
                else if (page == "state")
                    StateSelected?.Invoke(null);
                else if (page == "participation")
                    ParticipationSelected?.Invoke(null);
            }
        }
        private void PerformSearch(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                popupResults.IsOpen = false;
                return;
            }

            if (page == "section")
            {
                FilterResults(searchText, Storage.Instance.sections, FilteredSections);
            }
            else if (page == "position")
            {
                FilterResults(searchText, Storage.Instance.positions, FilteredPositions);
            }
            else if (page == "user")
            {
                FilterResults(searchText, Storage.Instance.users, FilteredUsers);
            }
            else if (page == "project")
            {
                FilterResults(searchText, Storage.Instance.projects, FilteredProjects);
            }
            else if (page == "state")
            {
                FilterResults(searchText, Storage.Instance.states, FilteredStates);
            }
            else if (page == "participation")
            {
                FilterResults(searchText, Storage.Instance.participations, FilteredParticipations);
            }

            if (listBoxSearch.Items.Count > 0 && listBoxSearch.SelectedItem == null)
            {
                popupResults.IsOpen = true;
            }
            else
            {
                popupResults.IsOpen = false;
            }
        }

        private void FilterResults<T>(string searchText, List<T> source, ObservableCollection<T> target) where T : class
        {
            target.Clear();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var searchParts = searchText.Split(' ').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();

                var filtered = source.Where(s =>
                {
                    var idProperty = s.GetType().GetProperty("Id")?.GetValue(s)?.ToString().ToLower();
                    var nameProperty = page == "user"
                        ? s.GetType().GetProperty("FullName")?.GetValue(s)?.ToString().ToLower()
                        : page == "state"
                            ? s.GetType().GetProperty("Description")?.GetValue(s)?.ToString().ToLower()
                            : s.GetType().GetProperty("Name")?.GetValue(s)?.ToString().ToLower();

                    string combined = page == "participation" ? idProperty + "" : (idProperty + " " + nameProperty).Trim();
                    return searchParts.All(part => combined.Contains(part));
                }).ToList();

                foreach (var item in filtered)
                {
                    target.Add(item);
                }
            }
            popupResults.IsOpen = target.Count > 0;
        }
        private void FilterResults<T>(List<T> source, ObservableCollection<T> target) where T : class
        {
            target.Clear();

            foreach (var item in source)
            {
                target.Add(item);
            }

            popupResults.IsOpen = target.Count > 0;
        }
        private void listBoxSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxSearch.SelectedItem is _Section selectedSection)
            {
                textBoxSearch.Text = selectedSection.Id + " " + selectedSection.Name;

                // Викликаємо подію, щоб передати вибраний розділ
                SectionSelected?.Invoke(selectedSection);
            }
            else if (listBoxSearch.SelectedItem is Position selectedPosition)
            {
                textBoxSearch.Text = selectedPosition.Id + " " + selectedPosition.Name;

                PositionSelected?.Invoke(selectedPosition);
            }
            else if (listBoxSearch.SelectedItem is User selectedUser)
            {
                textBoxSearch.Text = selectedUser.Id + " " + selectedUser.FullName;

                UserSelected?.Invoke(selectedUser);
            }
            else if (listBoxSearch.SelectedItem is Project selectedProject)
            {
                textBoxSearch.Text = selectedProject.Id + " " + selectedProject.Name;

                ProjectSelected?.Invoke(selectedProject);
            }
            else if (listBoxSearch.SelectedItem is State selectedState)
            {
                textBoxSearch.Text = selectedState.Id + " " + selectedState.Description;
                StateSelected?.Invoke(selectedState);
            }
            else if (listBoxSearch.SelectedItem is Participation selectedParticipation)
            {
                textBoxSearch.Text = selectedParticipation.Id.ToString();
                ParticipationSelected?.Invoke(selectedParticipation);
            }

            // Закриваємо список після вибору
            popupResults.IsOpen = false;
            listBoxSearch.SelectedItem = null; // Запобігає повторному виклику SelectionChanged
        }
    }
}
