namespace StackOverflowLite.Application.Dtos;

public record AuthResponse(
    Guid UserId,
    string Email,
    string DisplayName,
    string Token);
