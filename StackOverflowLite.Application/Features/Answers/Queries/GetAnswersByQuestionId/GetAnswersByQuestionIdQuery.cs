using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Dtos;

namespace StackOverflowLite.Application.Features.Answers.Queries.GetAnswersByQuestionId;

public record GetAnswersByQuestionIdQuery(Guid QuestionId) : IQuery<ErrorOr<AnswerResponseDto>>;
