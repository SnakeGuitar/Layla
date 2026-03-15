using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Layla.Desktop.Services;

namespace Layla.Desktop.Views;

public partial class VoicePanelView : Page
{
    private readonly Guid _projectId;
    private VoiceConnection? _voice;
    private readonly ObservableCollection<ParticipantViewModel> _participants = new();

    public VoicePanelView(Guid projectId)
    {
        InitializeComponent();
        _projectId = projectId;
        ParticipantList.ItemsSource = _participants;
    }

    private async void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ConnectButton.IsEnabled = false;
            ConnectButton.Content = "Connecting...";

            _voice = new VoiceConnection();
            WireEvents();

            await _voice.ConnectAsync();
            await _voice.JoinRoomAsync(_projectId);

            ConnectButton.Visibility = Visibility.Collapsed;
            LeaveButton.Visibility = Visibility.Visible;
            PttButton.IsEnabled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to connect: {ex.Message}", "Voice Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            ConnectButton.IsEnabled = true;
            ConnectButton.Content = "Connect";
        }
    }

    private async void LeaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_voice != null)
                await _voice.LeaveRoomAsync();

            await CleanupAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error leaving room: {ex.Message}", "Voice Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void PttButton_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (_voice == null || !_voice.IsConnected) return;

        try
        {
            await _voice.StartSpeakingAsync();
            PttStatusText.Text = "Transmitting...";
        }
        catch (HubExceptionWrapper ex)
        {
            PttStatusText.Text = ex.Message;
        }
        catch (Exception)
        {
            PttStatusText.Text = "Cannot transmit";
        }
    }

    private async void PttButton_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_voice == null || !_voice.IsConnected) return;

        try
        {
            await _voice.StopSpeakingAsync();
            PttStatusText.Text = "";
        }
        catch { }
    }

    private void WireEvents()
    {
        _voice!.ConnectionStateChanged += state =>
        {
            StatusText.Text = state;
            StatusIndicator.Fill = state switch
            {
                "Connected" => Brushes.LimeGreen,
                "Reconnecting" => Brushes.Orange,
                _ => Brushes.Gray
            };
        };

        _voice.RoomStateReceived += participants =>
        {
            _participants.Clear();
            foreach (var p in participants)
                _participants.Add(new ParticipantViewModel(p.UserId, p.DisplayName, p.IsSpeaking, p.Role));

            EmptyRoomText.Visibility = _participants.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        };

        _voice.ParticipantJoined += (userId, displayName, isSpeaking, role) =>
        {
            if (_participants.All(p => p.UserId != userId))
                _participants.Add(new ParticipantViewModel(userId, displayName, isSpeaking, role));

            EmptyRoomText.Visibility = Visibility.Collapsed;
        };

        _voice.ParticipantLeft += userId =>
        {
            var p = _participants.FirstOrDefault(x => x.UserId == userId);
            if (p != null) _participants.Remove(p);

            EmptyRoomText.Visibility = _participants.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        };

        _voice.SpeakerStarted += (userId, _) =>
        {
            var p = _participants.FirstOrDefault(x => x.UserId == userId);
            if (p != null) p.IsSpeaking = true;
        };

        _voice.SpeakerStopped += userId =>
        {
            var p = _participants.FirstOrDefault(x => x.UserId == userId);
            if (p != null) p.IsSpeaking = false;
        };
    }

    private async Task CleanupAsync()
    {
        PttButton.IsEnabled = false;
        LeaveButton.Visibility = Visibility.Collapsed;
        ConnectButton.Visibility = Visibility.Visible;
        ConnectButton.IsEnabled = true;
        ConnectButton.Content = "Connect";
        _participants.Clear();
        EmptyRoomText.Visibility = Visibility.Visible;
        StatusText.Text = "Disconnected";
        StatusIndicator.Fill = Brushes.Gray;
        PttStatusText.Text = "";

        if (_voice != null)
        {
            await _voice.DisposeAsync();
            _voice = null;
        }
    }
}

public class ParticipantViewModel : INotifyPropertyChanged
{
    private bool _isSpeaking;

    public string UserId { get; }
    public string DisplayName { get; }
    public string Role { get; }
    public string RoleLabel => Role == "Reader" ? "(Listener)" : "";

    public bool IsSpeaking
    {
        get => _isSpeaking;
        set { _isSpeaking = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSpeaking))); }
    }

    public ParticipantViewModel(string userId, string displayName, bool isSpeaking, string role)
    {
        UserId = userId;
        DisplayName = displayName;
        _isSpeaking = isSpeaking;
        Role = role;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

internal class HubExceptionWrapper : Exception
{
    public HubExceptionWrapper(string message) : base(message) { }
}
