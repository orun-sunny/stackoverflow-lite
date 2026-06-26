using FluentValidation;

namespace StackOverflowLite.Application.Features.Answers.Commands.UpdateAnswer;

public class UpdateAnswerCommandValidator : AbstractValidator<UpdateAnswerCommand>
{
    public UpdateAnswerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Answer ID is required.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Answer content is required.");
    }
}
