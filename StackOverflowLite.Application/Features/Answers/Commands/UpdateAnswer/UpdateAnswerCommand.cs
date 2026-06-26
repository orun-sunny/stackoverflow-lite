using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;

namespace StackOverflowLite.Application.Features.Answers.Commands.UpdateAnswer;

public record UpdateAnswerCommand(
    Guid Id,
    string Content) : ICommand<ErrorOr<Success>>;
