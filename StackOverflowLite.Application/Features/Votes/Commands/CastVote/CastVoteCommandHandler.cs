using ErrorOr;
using Microsoft.AspNetCore.Http;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Domain.Entities;
using System.Security.Claims;

namespace StackOverflowLite.Application.Features.Votes.Commands.CastVote;

public class CastVoteCommandHandler : ICommandHandler<CastVoteCommand, ErrorOr<Success>>
{
    private readonly IRepository<Vote> _voteRepository;
    private readonly IRepository<Question> _questionRepository;
    private readonly IRepository<Answer> _answerRepository;
    private readonly IRepository<UserProfile> _userProfileRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CastVoteCommandHandler(
        IRepository<Vote> voteRepository,
        IRepository<Question> questionRepository,
        IRepository<Answer> answerRepository,
        IRepository<UserProfile> userProfileRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _voteRepository = voteRepository;
        _questionRepository = questionRepository;
        _answerRepository = answerRepository;
        _userProfileRepository = userProfileRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ErrorOr<Success>> Handle(CastVoteCommand request, CancellationToken cancellationToken)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst("userid")?.Value 
                        ?? _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Error.Unauthorized("Auth.Unauthorized", "User is not authenticated.");
        }

        Guid authorId;
        int upvotePoints;
        int downvotePoints;
        string entityType = request.TargetType.ToLower().Trim();

        if (entityType == "question")
        {
            var question = await _questionRepository.GetByIdAsync(request.TargetId, false, cancellationToken);
            if (question is null)
            {
                return Error.NotFound("Question.NotFound", "Question was not found.");
            }
            if (question.UserId == userId)
            {
                return Error.Validation("Vote.SelfVoting", "You cannot vote on your own question.");
            }
            authorId = question.UserId;
            upvotePoints = 5;
            downvotePoints = -1;
        }
        else // answer
        {
            var answer = await _answerRepository.GetByIdAsync(request.TargetId, false, cancellationToken);
            if (answer is null)
            {
                return Error.NotFound("Answer.NotFound", "Answer was not found.");
            }
            if (answer.UserId == userId)
            {
                return Error.Validation("Vote.SelfVoting", "You cannot vote on your own answer.");
            }
            authorId = answer.UserId;
            upvotePoints = 10;
            downvotePoints = -2;
        }

        var authorProfile = await _userProfileRepository.FindFirstAsync(up => up.UserId == authorId, true, cancellationToken);
        if (authorProfile is null)
        {
            return Error.NotFound("Profile.NotFound", "Target author profile was not found.");
        }

        // Find existing vote
        Vote? existingVote;
        if (entityType == "question")
        {
            existingVote = await _voteRepository.FindFirstAsync(
                v => v.UserId == userId && v.QuestionId == request.TargetId,
                true,
                cancellationToken);
        }
        else
        {
            existingVote = await _voteRepository.FindFirstAsync(
                v => v.UserId == userId && v.AnswerId == request.TargetId,
                true,
                cancellationToken);
        }

        await _voteRepository.BeginTransactionAsync(cancellationToken);
        try
        {
            int reputationChange = 0;

            if (existingVote is null)
            {
                // New Vote
                var vote = new Vote
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    QuestionId = entityType == "question" ? request.TargetId : null,
                    AnswerId = entityType == "answer" ? request.TargetId : null,
                    VoteType = request.VoteType,
                    CreatedAt = DateTime.UtcNow
                };

                await _voteRepository.AddAsync(vote, cancellationToken);
                reputationChange = request.VoteType == VoteType.UpVote ? upvotePoints : downvotePoints;
            }
            else if (existingVote.VoteType == request.VoteType)
            {
                // Toggle Off (Delete Vote)
                _voteRepository.Remove(existingVote);
                
                // Reversing the vote impact
                int previousValue = existingVote.VoteType == VoteType.UpVote ? upvotePoints : downvotePoints;
                reputationChange = -previousValue;
            }
            else
            {
                // Switch Vote (UpVote -> DownVote or DownVote -> UpVote)
                int previousValue = existingVote.VoteType == VoteType.UpVote ? upvotePoints : downvotePoints;
                int newValue = request.VoteType == VoteType.UpVote ? upvotePoints : downvotePoints;

                // Revert previous and apply new
                reputationChange = -previousValue + newValue;

                existingVote.VoteType = request.VoteType;
                _voteRepository.Update(existingVote);
            }

            // Apply reputation changes (floor at 0)
            authorProfile.Reputation = Math.Max(0, authorProfile.Reputation + reputationChange);
            _userProfileRepository.Update(authorProfile);

            await _voteRepository.SaveChangesAsync(cancellationToken);
            await _voteRepository.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _voteRepository.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        return Result.Success;
    }
}
