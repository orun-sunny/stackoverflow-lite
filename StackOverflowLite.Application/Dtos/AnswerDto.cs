namespace StackOverflowLite.Application.Dtos;

public record AnswerDto(
    Guid Id,
    Guid QuestionId,
    Guid UserId,
    string Content,
    bool IsAccepted,
    DateTime CreatedAt,
    DateTime UpdatedAt);
