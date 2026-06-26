using FluentValidation;

namespace StackOverflowLite.Application.Features.Questions.Commands.CreateQuestion;

public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 5).WithMessage("You can specify at most 5 tags.");

        RuleForEach(x => x.Tags)
            .MaximumLength(50).WithMessage("Each tag name must be under 50 characters.");
    }
}
