using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Layla.Desktop.Models;
using Layla.Desktop.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Layla.Desktop.ViewModels
{
    public partial class WorkspaceViewModel : ObservableObject
    {
        private readonly IProjectApiService _projectApiService;

        [ObservableProperty]
        private Project? _currentProject;

        [ObservableProperty]
        private bool _isCollaboratorsModalVisible;

        [ObservableProperty]
        private ObservableCollection<Collaborator> _collaborators = new();

        [ObservableProperty]
        private string _inviteEmail = string.Empty;

        [ObservableProperty]
        private string _inviteError = string.Empty;

        [ObservableProperty]
        private bool _isInviting;

        public event EventHandler? OnLogout;
        public event EventHandler? OnBackToProjects;
        public event EventHandler? OnSettings;

        public WorkspaceViewModel(IProjectApiService projectApiService)
        {
            _projectApiService = projectApiService;
        }

        public void Initialize(Project project)
        {
            CurrentProject = project;
        }

        [RelayCommand]
        private void Logout()
        {
            Services.SessionManager.ClearSession();
            OnLogout?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void BackToProjects()
        {
            OnBackToProjects?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void OpenSettings()
        {
            OnSettings?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private async Task OpenCollaboratorsModalAsync()
        {
            InviteEmail = string.Empty;
            InviteError = string.Empty;
            IsCollaboratorsModalVisible = true;
            await LoadCollaboratorsAsync();
        }

        [RelayCommand]
        private void CloseCollaboratorsModal()
        {
            IsCollaboratorsModalVisible = false;
        }

        [RelayCommand]
        private async Task LoadCollaboratorsAsync()
        {
            if (CurrentProject == null) return;

            try
            {
                var result = await _projectApiService.GetCollaboratorsAsync(CurrentProject.Id);
                Collaborators.Clear();
                if (result != null)
                {
                    foreach (var c in result)
                        Collaborators.Add(c);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading collaborators: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task InviteCollaboratorAsync()
        {
            if (CurrentProject == null) return;

            if (string.IsNullOrWhiteSpace(InviteEmail))
            {
                InviteError = "Please enter an email address.";
                return;
            }

            IsInviting = true;
            InviteError = string.Empty;

            try
            {
                var request = new InviteCollaboratorRequest
                {
                    Email = InviteEmail,
                    Role = "READER"
                };

                var result = await _projectApiService.InviteCollaboratorAsync(CurrentProject.Id, request);
                if (result != null)
                {
                    InviteEmail = string.Empty;
                    await LoadCollaboratorsAsync();
                }
                else
                {
                    InviteError = "Could not invite user. Check the email and try again.";
                }
            }
            catch (Exception ex)
            {
                InviteError = $"Error: {ex.Message}";
            }
            finally
            {
                IsInviting = false;
            }
        }

        [RelayCommand]
        private async Task RemoveCollaboratorAsync(Collaborator collaborator)
        {
            if (CurrentProject == null) return;

            var confirm = MessageBox.Show($"Remove {collaborator.DisplayName ?? collaborator.Email} from this project?",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                var success = await _projectApiService.RemoveCollaboratorAsync(CurrentProject.Id, collaborator.UserId);
                if (success)
                    await LoadCollaboratorsAsync();
                else
                    MessageBox.Show("Failed to remove collaborator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
