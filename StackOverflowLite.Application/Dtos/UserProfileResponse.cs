namespace StackOverflowLite.Application.Dtos;

public record UserProfileResponse(
    Guid UserId,
    string DisplayName,
    int Reputation,
    DateTime CreatedAt);
