using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;

namespace StackOverflowLite.Application.Features.Answers.Commands.DeleteAnswer;

public record DeleteAnswerCommand(Guid Id) : ICommand<ErrorOr<Deleted>>;
