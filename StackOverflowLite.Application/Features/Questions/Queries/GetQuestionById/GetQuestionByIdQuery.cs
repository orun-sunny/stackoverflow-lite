using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Dtos;

namespace StackOverflowLite.Application.Features.Questions.Queries.GetQuestionById;

public record GetQuestionByIdQuery(Guid Id) : IQuery<ErrorOr<QuestionDto>>;
