using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Application.Features.Votes.Commands.CastVote;

public record CastVoteCommand(
    Guid TargetId,
    string TargetType, // "question" or "answer"
    VoteType VoteType) : ICommand<ErrorOr<Success>>;
