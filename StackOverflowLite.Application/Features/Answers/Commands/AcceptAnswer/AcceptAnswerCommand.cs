using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;

namespace StackOverflowLite.Application.Features.Answers.Commands.AcceptAnswer;

public record AcceptAnswerCommand(
    Guid QuestionId,
    Guid? AnswerId) : ICommand<ErrorOr<Success>>;
