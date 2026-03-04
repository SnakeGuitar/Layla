using Layla.Desktop.Services;
using System.Diagnostics;
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
            if (ProjectsListView.SelectedItem is Models.Project selectedProject)
            {
                NavigationService.Navigate(new WorkspaceView(selectedProject));
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
                Debug.WriteLine(ex);
            }
            finally
            {
                SaveProjectButton.IsEnabled = true;
                SaveProjectButton.Content = "Create";
            }
        }

        private Guid _editingProjectId;

        private void EditProject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Models.Project project)
            {
                _editingProjectId = project.Id;
                EditProjectTitle.Text = project.Title;
                EditProjectGenre.Text = project.LiteraryGenre;
                EditProjectSynopsis.Text = project.Synopsis;
                EditProjectError.Visibility = Visibility.Collapsed;
                EditProjectModal.Visibility = Visibility.Visible;
            }
        }

        private async void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Models.Project project)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the project '{project.Title}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    bool deleted = await _projectApiService.DeleteProjectAsync(project.Id);
                    if (deleted)
                    {
                        await ReloadProjectsAsync();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete project.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CancelEditProject_Click(object sender, RoutedEventArgs e)
        {
            EditProjectModal.Visibility = Visibility.Collapsed;
        }

        private async void UpdateProject_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EditProjectTitle.Text) ||
                string.IsNullOrWhiteSpace(EditProjectGenre.Text) ||
                string.IsNullOrWhiteSpace(EditProjectSynopsis.Text))
            {
                EditProjectError.Text = "Please fill in all fields.";
                EditProjectError.Visibility = Visibility.Visible;
                return;
            }

            EditProjectError.Visibility = Visibility.Collapsed;
            UpdateProjectButton.IsEnabled = false;
            UpdateProjectButton.Content = "Saving...";

            try
            {
                var request = new Models.UpdateProjectRequest
                {
                    Title = EditProjectTitle.Text,
                    LiteraryGenre = EditProjectGenre.Text,
                    Synopsis = EditProjectSynopsis.Text
                };

                var updatedProject = await _projectApiService.UpdateProjectAsync(_editingProjectId, request);

                if (updatedProject != null)
                {
                    EditProjectModal.Visibility = Visibility.Collapsed;
                    await ReloadProjectsAsync();
                }
                else
                {
                    EditProjectError.Text = "Failed to update project.";
                    EditProjectError.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                EditProjectError.Text = $"Error: {ex.Message}";
                EditProjectError.Visibility = Visibility.Visible;
            }
            finally
            {
                UpdateProjectButton.IsEnabled = true;
                UpdateProjectButton.Content = "Save";
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
