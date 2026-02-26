using Layla.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace Layla.Desktop.Views
{
    /// <summary>
    /// Interaction logic for ProjectListView.xaml
    /// </summary>
    public partial class ProjectListView : Page
    {
        private readonly IProjectApiService _projectApiService;

        public ProjectListView()
        {
            InitializeComponent();
            _projectApiService = new ProjectApiService();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var projects = await _projectApiService.GetMyProjectsAsync();
                ProjectsListView.ItemsSource = projects;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading projects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProjectsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ProjectsListView.SelectedItem is Models.ProjectDto selectedProject)
            {
                NavigationService.Navigate(new MainView());
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SettingsView());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Services.SessionManager.ClearSession();
            NavigationService.Navigate(new LoginView());
        }

        private void OpenCreateProject_Click(object sender, RoutedEventArgs e)
        {
            NewProjectTitle.Text = string.Empty;
            NewProjectGenre.Text = string.Empty;
            NewProjectSynopsis.Text = string.Empty;
            CreateProjectError.Visibility = Visibility.Collapsed;
            CreateProjectModal.Visibility = Visibility.Visible;
        }

        private void CancelCreateProject_Click(object sender, RoutedEventArgs e)
        {
            CreateProjectModal.Visibility = Visibility.Collapsed;
        }

        private async void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewProjectTitle.Text) ||
                string.IsNullOrWhiteSpace(NewProjectGenre.Text) ||
                string.IsNullOrWhiteSpace(NewProjectSynopsis.Text))
            {
                CreateProjectError.Text = "Please fill in all fields.";
                CreateProjectError.Visibility = Visibility.Visible;
                return;
            }

            CreateProjectError.Visibility = Visibility.Collapsed;
            SaveProjectButton.IsEnabled = false;
            SaveProjectButton.Content = "Creating...";

            try
            {
                var request = new Models.CreateProjectRequest
                {
                    Title = NewProjectTitle.Text,
                    LiteraryGenre = NewProjectGenre.Text,
                    Synopsis = NewProjectSynopsis.Text
                };

                var newProject = await _projectApiService.CreateProjectAsync(request);

                if (newProject != null)
                {
                    CreateProjectModal.Visibility = Visibility.Collapsed;
                    await ReloadProjectsAsync();
                }
                else
                {
                    CreateProjectError.Text = "Failed to create project.";
                    CreateProjectError.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                CreateProjectError.Text = $"Error: {ex.Message}";
                CreateProjectError.Visibility = Visibility.Visible;
            }
            finally
            {
                SaveProjectButton.IsEnabled = true;
                SaveProjectButton.Content = "Create";
            }
        }

        private async Task ReloadProjectsAsync()
        {
            try
            {
                var projects = await _projectApiService.GetMyProjectsAsync();
                ProjectsListView.ItemsSource = projects;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading projects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
