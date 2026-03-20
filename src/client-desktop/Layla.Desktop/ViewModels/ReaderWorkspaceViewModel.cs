using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Layla.Desktop.Models;
using Layla.Desktop.Services;
using System;
using System.Threading.Tasks;

namespace Layla.Desktop.ViewModels
{
    public partial class ReaderWorkspaceViewModel : ObservableObject
    {
        private readonly IProjectApiService _projectApiService;

        [ObservableProperty]
        private Project? _currentProject;

        [ObservableProperty]
        private bool _isAuthorActive;

        [ObservableProperty]
        private string _authorStatusText = "Author is offline";

        public event EventHandler? OnBackToPublicProjects;
        public event EventHandler? OnLogout;

        public ReaderWorkspaceViewModel(IProjectApiService projectApiService)
        {
            _projectApiService = projectApiService;
        }

        public void Initialize(Project project)
        {
            CurrentProject = project;
            IsAuthorActive = project.IsAuthorActive;
            AuthorStatusText = project.IsAuthorActive ? "Author is active - live changes" : "Author is offline";
        }

        public async Task StartWatchingPresenceAsync()
        {
            if (CurrentProject == null) return;

            try
            {
                await _projectApiService.ConnectPresenceHubAsync((projectId, isActive) =>
                {
                    if (CurrentProject != null && projectId == CurrentProject.Id)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            IsAuthorActive = isActive;
                            AuthorStatusText = isActive ? "Author is active - live changes" : "Author is offline";
                        });
                    }
                });

                await _projectApiService.WatchProjectAsync(CurrentProject.Id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error connecting to presence hub: {ex.Message}");
            }
        }

        [RelayCommand]
        private void BackToPublicProjects()
        {
            OnBackToPublicProjects?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void Logout()
        {
            SessionManager.ClearSession();
            OnLogout?.Invoke(this, EventArgs.Empty);
        }
    }
}
