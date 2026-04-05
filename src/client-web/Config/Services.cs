using client_web.Application.Config.Http;
using client_web.Application.Config.SignalR;
using client_web.Application.Services.ActiveStatusAuthor;
using client_web.Application.Services.Projects;
using client_web.Application.Services.Voice;
using client_web.Services;

namespace client_web.Config;

public static class Services
{
    public static void Configure(this IServiceCollection services)
    {
        // HTTP Services
        services.AddScoped<ApiClient>();
        services.AddScoped<AuthService>();

        // Other Services
        services.AddScoped<PresenceService>();
        services.AddScoped<IProjectService, ProjectService>();

        // Voice Services
        services.AddSingleton<ISignalRClient, SignalRClient>();
        services.AddSingleton<IVoiceService, VoiceService>();
        services.AddSingleton<Application.Services.Voice.IConnectionService>(sp => sp.GetRequiredService<IVoiceService>());
        services.AddSingleton<IRoomService>(sp => sp.GetRequiredService<IVoiceService>());
        services.AddSingleton<IAudioService>(sp => sp.GetRequiredService<IVoiceService>());
    }
}