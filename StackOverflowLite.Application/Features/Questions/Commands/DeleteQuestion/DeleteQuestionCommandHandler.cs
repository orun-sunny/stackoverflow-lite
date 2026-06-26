using ErrorOr;
using Microsoft.AspNetCore.Http;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Domain.Entities;
using System.Security.Claims;

namespace StackOverflowLite.Application.Features.Questions.Commands.DeleteQuestion;

public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand, ErrorOr<Deleted>>
{
    private readonly IRepository<Question> _questionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteQuestionCommandHandler(
        IRepository<Question> questionRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _questionRepository = questionRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst("userid")?.Value 
                        ?? _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Error.Unauthorized("Auth.Unauthorized", "User is not authenticated.");
        }

        var question = await _questionRepository.GetByIdAsync(
            request.Id,
            true, // Track changes
            cancellationToken);

        if (question is null)
        {
            return Error.NotFound("Question.NotFound", "Question was not found.");
        }

        if (question.UserId != userId)
        {
            return Error.Forbidden("Question.Forbidden", "You can only delete your own questions.");
        }

        _questionRepository.Remove(question);
        await _questionRepository.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
