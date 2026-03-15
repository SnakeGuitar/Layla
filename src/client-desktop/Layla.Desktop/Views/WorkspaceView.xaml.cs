using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Layla.Desktop.Models;
using Layla.Desktop.Services;
using System.Collections.Generic;

namespace Layla.Desktop.Views
{
    public partial class WorkspaceView : Page
    {
        private readonly Project _currentProject;
        private readonly IProjectApiService _projectApiService;

        public WorkspaceView(Project currentProject)
        {
            InitializeComponent();
            _currentProject = currentProject;
            _projectApiService = new ProjectApiService();
            this.Loaded += WorkspaceView_Loaded;
        }

        private void WorkspaceView_Loaded(object sender, RoutedEventArgs e)
        {
            ProjectTitleText.Text = _currentProject.Title;
            
            EditorFrame.Navigate(new ManuscriptEditorView(_currentProject.Id));
            VoiceFrame.Navigate(new VoicePanelView(_currentProject.Id));

            try 
            {
                while (NavigationService != null && NavigationService.CanGoBack)
                {
                    NavigationService.RemoveBackEntry();
                }
            } 
            catch { }
        }
        
        private void BackToProjects_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProjectListView());
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
    }
}
