using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Layla.Desktop.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace Layla.Desktop.Services
{
    public class ProjectApiService : IProjectApiService
    {
        private readonly HttpClient _httpClient;
        private HubConnection? _presenceHub;

        public ProjectApiService()
        {
            _httpClient = ConfigurationService.CreateHttpClient(ConfigurationService.ServerCoreUrl);
        }

        private void AddAuthorizationHeader()
        {
            if (SessionManager.IsAuthenticated)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.CurrentToken);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<IEnumerable<Project>?> GetMyProjectsAsync()
        {
            AddAuthorizationHeader();
            
            try
            {
                var response = await _httpClient.GetAsync("/api/projects");
                if (response.IsSuccessStatusCode)
                {
                    var projects = await response.Content.ReadFromJsonAsync<IEnumerable<Project>>();
                    if (projects != null)
                    {
                        return projects;
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve projects: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving projects: {ex.Message}");
            }

            return new List<Project>();
        }

        public async Task<Project?> CreateProjectAsync(CreateProjectRequest request)
        {
            AddAuthorizationHeader();
            
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/projects", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Project>();
                }
                
                System.Diagnostics.Debug.WriteLine($"Failed to create project: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating project: {ex.Message}");
            }

            return null;
        }

        public async Task<Project?> UpdateProjectAsync(Guid id, UpdateProjectRequest request)
        {
            AddAuthorizationHeader();
            
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/api/projects/{id}", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Project>();
                }
                
                System.Diagnostics.Debug.WriteLine($"Failed to update project: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating project: {ex.Message}");
            }

            return null;
        }

        public async Task<bool> DeleteProjectAsync(Guid id)
        {
            AddAuthorizationHeader();

            try
            {
                var response = await _httpClient.DeleteAsync($"/api/projects/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                System.Diagnostics.Debug.WriteLine($"Failed to delete project: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting project: {ex.Message}");
            }

            return false;
        }

        public async Task<IEnumerable<Project>?> GetPublicProjectsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/projects/public");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<IEnumerable<Project>>();

                System.Diagnostics.Debug.WriteLine($"Failed to retrieve public projects: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving public projects: {ex.Message}");
            }

            return new List<Project>();
        }

        public async Task<Project?> GetProjectByIdAsync(Guid id)
        {
            AddAuthorizationHeader();
            try
            {
                var response = await _httpClient.GetAsync($"/api/projects/{id}");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<Project>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving project: {ex.Message}");
            }
            return null;
        }

        public async Task<Collaborator?> JoinPublicProjectAsync(Guid projectId)
        {
            AddAuthorizationHeader();
            try
            {
                var response = await _httpClient.PostAsync($"/api/projects/{projectId}/join", null);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<Collaborator>();

                System.Diagnostics.Debug.WriteLine($"Failed to join project: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error joining project: {ex.Message}");
            }
            return null;
        }

        public async Task<Collaborator?> InviteCollaboratorAsync(Guid projectId, InviteCollaboratorRequest request)
        {
            AddAuthorizationHeader();
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"/api/projects/{projectId}/collaborators", request);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<Collaborator>();

                System.Diagnostics.Debug.WriteLine($"Failed to invite collaborator: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inviting collaborator: {ex.Message}");
            }
            return null;
        }

        public async Task<IEnumerable<Collaborator>?> GetCollaboratorsAsync(Guid projectId)
        {
            AddAuthorizationHeader();
            try
            {
                var response = await _httpClient.GetAsync($"/api/projects/{projectId}/collaborators");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<IEnumerable<Collaborator>>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving collaborators: {ex.Message}");
            }
            return new List<Collaborator>();
        }

        public async Task<bool> RemoveCollaboratorAsync(Guid projectId, string collaboratorUserId)
        {
            AddAuthorizationHeader();
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/projects/{projectId}/collaborators/{collaboratorUserId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing collaborator: {ex.Message}");
            }
            return false;
        }

        public async Task ConnectPresenceHubAsync(Action<Guid, bool> onAuthorStatusChanged)
        {
            if (_presenceHub != null) return;

            _presenceHub = new HubConnectionBuilder()
                .WithUrl($"{ConfigurationService.ServerCoreUrl}/hubs/presence", options =>
                {
                    if (SessionManager.IsAuthenticated)
                        options.AccessTokenProvider = () => Task.FromResult<string?>(SessionManager.CurrentToken);
                })
                .WithAutomaticReconnect()
                .Build();

            _presenceHub.On<Guid, bool>("AuthorStatusChanged", onAuthorStatusChanged);

            await _presenceHub.StartAsync();
        }

        public async Task WatchProjectAsync(Guid projectId)
        {
            if (_presenceHub?.State == HubConnectionState.Connected)
                await _presenceHub.InvokeAsync("WatchProject", projectId);
        }

        public async Task AuthorHeartbeatAsync(Guid projectId)
        {
            if (_presenceHub?.State == HubConnectionState.Connected)
                await _presenceHub.InvokeAsync("AuthorHeartbeat", projectId);
        }

        public async Task DisconnectPresenceHubAsync()
        {
            if (_presenceHub != null)
            {
                await _presenceHub.StopAsync();
                await _presenceHub.DisposeAsync();
                _presenceHub = null;
            }
        }
    }
}
