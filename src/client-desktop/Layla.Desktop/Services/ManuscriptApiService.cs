using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Layla.Desktop.Models;

namespace Layla.Desktop.Services
{
    public class ManuscriptApiService : IManuscriptApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7165";

        public ManuscriptApiService()
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

        public async Task<IEnumerable<Manuscript>> GetManuscriptsByProjectIdAsync(Guid projectId)
        {
            AddAuthorizationHeader();
            
            try
            {
                var response = await _httpClient.GetAsync($"/api/manuscripts/project/{projectId}");
                if (response.IsSuccessStatusCode)
                {
                    var manuscripts = await response.Content.ReadFromJsonAsync<IEnumerable<Manuscript>>();
                    if (manuscripts != null)
                    {
                        return manuscripts;
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve manuscripts: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving manuscripts: {ex.Message}");
            }

            return new List<Manuscript>();
        }
    }
}
