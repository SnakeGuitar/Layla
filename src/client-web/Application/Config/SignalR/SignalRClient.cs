using Microsoft.AspNetCore.SignalR.Client;
using Polly;
using Polly.Retry;

namespace client_web.Application.Config.SignalR;

public class SignalRClient : ISignalRClient
{
    private string _baseUrl;
    private AsyncRetryPolicy _retryPolicy;
    private readonly ILogger<SignalRClient> _logger;

    public SignalRClient(IConfiguration configuration, ILogger<SignalRClient> logger)
    {
        _baseUrl = configuration["ApiUrls:BackendURL"]!;
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, time) =>
                {
                    Console.WriteLine($"Retrying in {time}. Error: {ex.Message}");
                });
        _logger = logger;
    }

    private HubConnection? _hub;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    public bool IsConnected => _hub?.State == HubConnectionState.Connected;
    public event EventHandler<SignalRConnectionState>? OnConnectionChanged;

    public async Task ConnectAsync(string url, string token)
    {
        if (_hub != null && IsConnected) return;

        await _connectionLock.WaitAsync();
        try
        {
            if (IsConnected && _hub != null && _hub.State == HubConnectionState.Connected) return;
            await _hub!.StopAsync();
            await _hub!.DisposeAsync();

            _hub = new HubConnectionBuilder()
                .WithUrl(_baseUrl + url, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                })
                .WithAutomaticReconnect()
                .Build();

            _hub.Reconnecting += _ =>
            {
                _logger.LogWarning("SignalR connection lost. Attempting to reconnect...");
                Notify(SignalRConnectionState.Reconnecting);
                return Task.CompletedTask;
            };

            _hub.Reconnected += _ =>
            {
                _logger.LogInformation("SignalR reconnected.");
                Notify(SignalRConnectionState.Connected);
                return Task.CompletedTask;
            };

            _hub.Closed += _ =>
            {
                _logger.LogInformation("SignalR connection closed.");
                Notify(SignalRConnectionState.Disconnected);
                return Task.CompletedTask;
            };

            Notify(SignalRConnectionState.Connecting);

            await _retryPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation("Attempting to connect to SignalR hub...");
                await _hub.StartAsync();
            });

            Notify(SignalRConnectionState.Connected);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task DisconnectAsync()
    {
        await _connectionLock.WaitAsync();
        try
        {
            if (_hub == null) return;

            _logger.LogInformation("Disconnecting from SignalR hub...");
            await _hub.StopAsync();
            await _hub.DisposeAsync();
            _hub = null;

            OnConnectionChanged?.Invoke(this, SignalRConnectionState.Disconnected);
        }
        finally
        {
            _logger.LogInformation("SignalR disconnect process completed.");
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Disposing SignalR client...");
        await DisconnectAsync();
        _connectionLock.Dispose();
    }

    private void Notify(SignalRConnectionState state)
    {
        try
        {
            OnConnectionChanged?.Invoke(this, state);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error in event handler: {ex.Message}");
        }
    }

    public async Task InvokeSafeAsync(string method, params object[] args)
    {
        if (!IsConnected || _hub == null) return;
        await _hub.InvokeAsync(method, args);
    }

    public void On<T>(string methodName, Action<T> handler)
    {
        if (_hub == null) throw new InvalidOperationException("Call ConnectAsync first");
        _hub.On(methodName, handler);
    }

    public void On<T1, T2>(string methodName, Action<T1, T2> handler)
    {
        if (_hub == null) throw new InvalidOperationException("Call ConnectAsync first");
        _hub.On(methodName, handler);
    }
}
