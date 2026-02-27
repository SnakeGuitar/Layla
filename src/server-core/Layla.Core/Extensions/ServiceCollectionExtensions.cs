using Layla.Core.Interfaces.Services;
using Layla.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Layla.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IManuscriptService, ManuscriptService>();
        services.AddScoped<IWikiService, WikiService>();

        return services;
    }
}
