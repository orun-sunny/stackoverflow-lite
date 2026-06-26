using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;

namespace StackOverflowLite.Application.Features.Questions.Commands.UpdateQuestion;

public record UpdateQuestionCommand(
    Guid Id,
    string Title,
    string Description,
    List<string> Tags) : ICommand<ErrorOr<Success>>;
