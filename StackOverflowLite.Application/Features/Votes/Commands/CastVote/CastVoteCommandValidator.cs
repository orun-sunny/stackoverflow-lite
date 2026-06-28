using FluentValidation;

namespace StackOverflowLite.Application.Features.Votes.Commands.CastVote;

public class CastVoteCommandValidator : AbstractValidator<CastVoteCommand>
{
    public CastVoteCommandValidator()
    {
        RuleFor(x => x.TargetId)
            .NotEmpty().WithMessage("Target ID is required.");

        RuleFor(x => x.TargetType)
            .NotEmpty().WithMessage("Target type is required.")
            .Must(type => type.ToLower() == "question" || type.ToLower() == "answer")
            .WithMessage("Target type must be either 'question' or 'answer'.");

        RuleFor(x => x.VoteType)
            .IsInEnum().WithMessage("Invalid vote type.");
    }
}
