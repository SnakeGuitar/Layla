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

        public async Task<IEnumerable<ProjectDto>> GetMyProjectsAsync()
        {
            AddAuthorizationHeader();
            
            try
            {
                var response = await _httpClient.GetAsync("/api/projects");
                if (response.IsSuccessStatusCode)
                {
                    var projects = await response.Content.ReadFromJsonAsync<IEnumerable<ProjectDto>>();
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

            return new List<ProjectDto>();
        }

        public async Task<ProjectDto?> CreateProjectAsync(CreateProjectRequest request)
        {
            AddAuthorizationHeader();
            
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/projects", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ProjectDto>();
                }
                
                System.Diagnostics.Debug.WriteLine($"Failed to create project: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating project: {ex.Message}");
            }

            return null;
        }
    }
}
