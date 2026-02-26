using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Layla.Desktop.Models;

namespace Layla.Desktop.Services
{
    public interface IManuscriptApiService
    {
        Task<IEnumerable<Manuscript>> GetManuscriptsByProjectIdAsync(Guid projectId);
    }
}
