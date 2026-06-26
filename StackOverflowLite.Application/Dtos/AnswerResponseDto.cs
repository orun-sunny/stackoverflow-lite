namespace StackOverflowLite.Application.Dtos;

public record AnswerResponseDto(
    AnswerDto? AcceptedAnswer,
    List<AnswerDto> OtherAnswers);
