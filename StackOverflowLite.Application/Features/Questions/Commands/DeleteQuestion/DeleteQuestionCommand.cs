using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;

namespace StackOverflowLite.Application.Features.Questions.Commands.DeleteQuestion;

public record DeleteQuestionCommand(Guid Id) : ICommand<ErrorOr<Deleted>>;
