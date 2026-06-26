using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;

namespace StackOverflowLite.Application.Features.Answers.Commands.CreateAnswer;

public record CreateAnswerCommand(
    Guid QuestionId,
    string Content) : ICommand<ErrorOr<Guid>>;
