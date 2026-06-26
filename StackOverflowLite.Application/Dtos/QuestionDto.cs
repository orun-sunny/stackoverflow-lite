namespace StackOverflowLite.Application.Dtos;

public record QuestionDto(
    Guid Id,
    Guid UserId,
    string Title,
    string Description,
    List<string> Tags,
    long Views,
    DateTime CreatedAt,
    DateTime UpdatedAt);
