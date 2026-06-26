using FluentValidation;

namespace StackOverflowLite.Application.Features.Answers.Commands.CreateAnswer;

public class CreateAnswerCommandValidator : AbstractValidator<CreateAnswerCommand>
{
    public CreateAnswerCommandValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty().WithMessage("Question ID is required.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Answer content is required.");
    }
}
