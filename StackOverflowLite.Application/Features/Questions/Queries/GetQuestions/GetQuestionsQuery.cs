using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Dtos;

namespace StackOverflowLite.Application.Features.Questions.Queries.GetQuestions;

public record GetQuestionsQuery(string? Tag) : IQuery<ErrorOr<IEnumerable<QuestionDto>>>;
