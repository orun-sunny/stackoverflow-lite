using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;

namespace StackOverflowLite.Application.Features.Questions.Commands.CreateQuestion;

public record CreateQuestionCommand(
    string Title,
    string Description,
    List<string> Tags) : ICommand<ErrorOr<Guid>>;
