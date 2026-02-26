using Layla.Desktop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Layla.Desktop.Services
{
    public interface IProjectApiService
    {
        Task<IEnumerable<Project>> GetMyProjectsAsync();
        Task<Project?> CreateProjectAsync(CreateProjectRequest request);
        Task<Project?> UpdateProjectAsync(Guid id, UpdateProjectRequest request);
        Task<bool> DeleteProjectAsync(Guid id);
    }
}
