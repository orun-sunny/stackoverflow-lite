using ErrorOr;
using Microsoft.AspNetCore.Http;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Domain.Entities;
using System.Security.Claims;

namespace StackOverflowLite.Application.Features.Answers.Commands.AcceptAnswer;

public class AcceptAnswerCommandHandler : ICommandHandler<AcceptAnswerCommand, ErrorOr<Success>>
{
    private readonly IRepository<Question> _questionRepository;
    private readonly IRepository<Answer> _answerRepository;
    private readonly IRepository<UserProfile> _userProfileRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AcceptAnswerCommandHandler(
        IRepository<Question> questionRepository,
        IRepository<Answer> answerRepository,
        IRepository<UserProfile> userProfileRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _questionRepository = questionRepository;
        _answerRepository = answerRepository;
        _userProfileRepository = userProfileRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ErrorOr<Success>> Handle(AcceptAnswerCommand request, CancellationToken cancellationToken)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst("userid")?.Value 
                        ?? _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Error.Unauthorized("Auth.Unauthorized", "User is not authenticated.");
        }

        var question = await _questionRepository.GetByIdAsync(request.QuestionId, true, cancellationToken);
        if (question is null)
        {
            return Error.NotFound("Question.NotFound", "Question was not found.");
        }

        if (question.UserId != userId)
        {
            return Error.Forbidden("Question.Forbidden", "Only the question author can accept an answer.");
        }

        var answers = await _answerRepository.FindAsync(a => a.QuestionId == request.QuestionId, true, cancellationToken);

        await _answerRepository.BeginTransactionAsync(cancellationToken);
        try
        {
            if (request.AnswerId is null)
            {
                // Unmark current accepted answer
                var currentlyAccepted = answers.FirstOrDefault(a => a.IsAccepted);
                if (currentlyAccepted is not null)
                {
                    currentlyAccepted.IsAccepted = false;
                    _answerRepository.Update(currentlyAccepted);

                    var authorProfile = await _userProfileRepository.FindFirstAsync(up => up.UserId == currentlyAccepted.UserId, true, cancellationToken);
                    if (authorProfile is not null)
                    {
                        authorProfile.Reputation = Math.Max(0, authorProfile.Reputation - 15);
                        _userProfileRepository.Update(authorProfile);
                    }
                }
            }
            else
            {
                // Accept new or switch answer
                var selectedAnswer = answers.FirstOrDefault(a => a.Id == request.AnswerId.Value);
                if (selectedAnswer is null)
                {
                    await _answerRepository.RollbackTransactionAsync(cancellationToken);
                    return Error.NotFound("Answer.NotFound", "Answer does not belong to this question.");
                }

                if (selectedAnswer.UserId == question.UserId)
                {
                    await _answerRepository.RollbackTransactionAsync(cancellationToken);
                    return Error.Validation("Answer.SelfAcceptance", "Users cannot accept their own answers on their own questions.");
                }

                if (!selectedAnswer.IsAccepted)
                {
                    // Unaccept previous if it exists
                    var previousAccepted = answers.FirstOrDefault(a => a.IsAccepted);
                    if (previousAccepted is not null)
                    {
                        previousAccepted.IsAccepted = false;
                        _answerRepository.Update(previousAccepted);

                        var prevAuthorProfile = await _userProfileRepository.FindFirstAsync(up => up.UserId == previousAccepted.UserId, true, cancellationToken);
                        if (prevAuthorProfile is not null)
                        {
                            prevAuthorProfile.Reputation = Math.Max(0, prevAuthorProfile.Reputation - 15);
                            _userProfileRepository.Update(prevAuthorProfile);
                        }
                    }

                    // Accept selected answer
                    selectedAnswer.IsAccepted = true;
                    _answerRepository.Update(selectedAnswer);

                    var selectedAuthorProfile = await _userProfileRepository.FindFirstAsync(up => up.UserId == selectedAnswer.UserId, true, cancellationToken);
                    if (selectedAuthorProfile is not null)
                    {
                        selectedAuthorProfile.Reputation += 15;
                        _userProfileRepository.Update(selectedAuthorProfile);
                    }
                }
            }

            await _answerRepository.SaveChangesAsync(cancellationToken);
            await _answerRepository.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _answerRepository.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        return Result.Success;
    }
}
