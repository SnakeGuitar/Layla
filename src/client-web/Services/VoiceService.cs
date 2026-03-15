using Microsoft.AspNetCore.SignalR.Client;

namespace client_web.Services;

public record VoiceParticipant(string UserId, string DisplayName, bool IsSpeaking, string Role);
public record VoiceRoomState(Guid ProjectId, List<VoiceParticipant> Participants);

public class VoiceService : IAsyncDisposable
{
    private HubConnection? _hub;
    private const string VoiceHubBaseUrl = "https://localhost:7165";

    public event Action<List<VoiceParticipant>>? OnRoomState;
    public event Action<VoiceParticipant>? OnUserJoined;
    public event Action<string>? OnUserLeft;
    public event Action<string, string>? OnSpeakerStarted;
    public event Action<string>? OnSpeakerStopped;
    public event Action<string, byte[]>? OnAudioReceived;
    public event Action<string>? OnConnectionChanged;

    public bool IsConnected => _hub?.State == HubConnectionState.Connected;

    public async Task ConnectAsync(string token)
    {
        if (_hub != null) return;

        _hub = new HubConnectionBuilder()
            .WithUrl($"{VoiceHubBaseUrl}/hubs/voice", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .WithAutomaticReconnect()
            .Build();

        RegisterHandlers();

        _hub.Closed += _ => { OnConnectionChanged?.Invoke("Disconnected"); return Task.CompletedTask; };
        _hub.Reconnecting += _ => { OnConnectionChanged?.Invoke("Reconnecting"); return Task.CompletedTask; };
        _hub.Reconnected += _ => { OnConnectionChanged?.Invoke("Connected"); return Task.CompletedTask; };

        await _hub.StartAsync();
        OnConnectionChanged?.Invoke("Connected");
    }

    public async Task JoinRoomAsync(Guid projectId)
    {
        if (_hub == null) return;
        await _hub.InvokeAsync("JoinRoom", projectId);
    }

    public async Task LeaveRoomAsync(Guid projectId)
    {
        if (_hub == null) return;
        await _hub.InvokeAsync("LeaveRoom", projectId);
    }

    public async Task StartSpeakingAsync(Guid projectId)
    {
        if (_hub == null) return;
        await _hub.InvokeAsync("StartSpeaking", projectId);
    }

    public async Task StopSpeakingAsync(Guid projectId)
    {
        if (_hub == null) return;
        await _hub.InvokeAsync("StopSpeaking", projectId);
    }

    public async Task SendAudioAsync(Guid projectId, byte[] audioData)
    {
        if (_hub?.State != HubConnectionState.Connected) return;
        await _hub.InvokeAsync("SendAudio", projectId, audioData);
    }

    public async Task DisconnectAsync()
    {
        if (_hub != null)
        {
            await _hub.StopAsync();
            await _hub.DisposeAsync();
            _hub = null;
        }
        OnConnectionChanged?.Invoke("Disconnected");
    }

    private void RegisterHandlers()
    {
        _hub!.On<VoiceRoomState>("RoomState", state =>
        {
            OnRoomState?.Invoke(state.Participants);
        });

        _hub.On<VoiceParticipant>("UserJoined", participant =>
        {
            OnUserJoined?.Invoke(participant);
        });

        _hub.On<string>("UserLeft", userId =>
        {
            OnUserLeft?.Invoke(userId);
        });

        _hub.On<string, string>("UserStartedSpeaking", (userId, displayName) =>
        {
            OnSpeakerStarted?.Invoke(userId, displayName);
        });

        _hub.On<string>("UserStoppedSpeaking", userId =>
        {
            OnSpeakerStopped?.Invoke(userId);
        });

        _hub.On<string, byte[]>("ReceiveAudio", (senderId, audioData) =>
        {
            OnAudioReceived?.Invoke(senderId, audioData);
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_hub != null)
        {
            await _hub.DisposeAsync();
            _hub = null;
        }
    }
}
