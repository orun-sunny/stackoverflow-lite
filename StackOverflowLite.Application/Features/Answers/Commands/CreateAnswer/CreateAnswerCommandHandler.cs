using ErrorOr;
using Microsoft.AspNetCore.Http;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Domain.Entities;
using System.Security.Claims;

namespace StackOverflowLite.Application.Features.Answers.Commands.CreateAnswer;

public class CreateAnswerCommandHandler : ICommandHandler<CreateAnswerCommand, ErrorOr<Guid>>
{
    private readonly IRepository<Answer> _answerRepository;
    private readonly IRepository<Question> _questionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateAnswerCommandHandler(
        IRepository<Answer> answerRepository,
        IRepository<Question> questionRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst("userid")?.Value 
                        ?? _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Error.Unauthorized("Auth.Unauthorized", "User is not authenticated.");
        }

        var question = await _questionRepository.GetByIdAsync(request.QuestionId, false, cancellationToken);
        if (question is null)
        {
            return Error.NotFound("Question.NotFound", "Question was not found.");
        }

        var answer = new Answer
        {
            Id = Guid.NewGuid(),
            QuestionId = request.QuestionId,
            UserId = userId,
            Content = request.Content,
            IsAccepted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _answerRepository.AddAsync(answer, cancellationToken);
        await _answerRepository.SaveChangesAsync(cancellationToken);

        return answer.Id;
    }
}
