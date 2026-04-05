using client_web.Application.Config.Http;

namespace client_web.Application.Services.Projects;

public class ProjectService : IProjectService
{
    private readonly ApiClient _client;

    public ProjectService(ApiClient api)
    {
        _client = api;
    }

    public async Task<IEnumerable<ProjectResponse>> GetUserProjectsAsync(string token)
    {
        return await _client.SendAsync<IEnumerable<ProjectResponse>>(new APIRequest
        {
            Endpoint = "/api/projects",
            Method = HttpMethod.Get,
            Token = token
        });
    }

    public async Task<IEnumerable<ProjectResponse>> GetAllProjectsAsync(string token)
    {
        return await _client.SendAsync<IEnumerable<ProjectResponse>>(new APIRequest
        {
            Endpoint = "/api/projects/all",
            Method = HttpMethod.Get,
            Token = token
        });
    }

    public async Task<List<PublicProjectDto>> GetPublicProjectsAsync()
    {
        return await _client.SendAsync<List<PublicProjectDto>>(new APIRequest
        {
            Endpoint = "/api/projects/public",
            Method = HttpMethod.Get,
            Token = null
        });
    }
}