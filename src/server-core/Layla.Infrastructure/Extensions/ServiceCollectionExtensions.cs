using Layla.Core.Entities;
using Layla.Core.Interfaces.Data;
using Layla.Core.Interfaces.Messaging;
using Layla.Core.Interfaces.Services;
using Layla.Infrastructure.Configuration;
using Layla.Infrastructure.Data;
using Layla.Infrastructure.Data.Repositories;
using Layla.Infrastructure.Messaging;
using Layla.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Layla.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<MongoDbSettings>(
            configuration.GetSection("MongoDbSettings"));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            return new MongoClient(settings!.ConnectionString);
        });

        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 12;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IDocumentRepository, MongoDocumentRepository>();
        services.AddScoped<IEventPublisher, DummyEventPublisher>();
        services.AddSingleton<IEventBus, EventBus>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
