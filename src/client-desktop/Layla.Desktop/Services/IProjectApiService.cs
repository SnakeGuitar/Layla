using Layla.Desktop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Layla.Desktop.Services
{
    public interface IProjectApiService
    {
        Task<IEnumerable<ProjectDto>> GetMyProjectsAsync();
        Task<ProjectDto?> CreateProjectAsync(CreateProjectRequest request);
    }
}
