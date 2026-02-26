using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Layla.Desktop.Models;

namespace Layla.Desktop.Services
{
    public class ProjectApiService : IProjectApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7165";

        public ProjectApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
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

        public async Task<IEnumerable<Project>> GetMyProjectsAsync()
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
    }
}
