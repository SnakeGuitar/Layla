namespace Layla.Core.Contracts.Voice;

public record VoiceParticipantDto(
    string UserId,
    string DisplayName,
    bool IsSpeaking,
    string Role,
    DateTime JoinedAt
);

public record VoiceRoomStateDto(
    Guid ProjectId,
    List<VoiceParticipantDto> Participants
);
