using ErrorOr;
using Microsoft.AspNetCore.Http;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Domain.Entities;
using System.Security.Claims;

namespace StackOverflowLite.Application.Features.Answers.Commands.DeleteAnswer;

public class DeleteAnswerCommandHandler : ICommandHandler<DeleteAnswerCommand, ErrorOr<Deleted>>
{
    private readonly IRepository<Answer> _answerRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteAnswerCommandHandler(
        IRepository<Answer> answerRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _answerRepository = answerRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst("userid")?.Value 
                        ?? _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Error.Unauthorized("Auth.Unauthorized", "User is not authenticated.");
        }

        var answer = await _answerRepository.GetByIdAsync(request.Id, true, cancellationToken);
        if (answer is null)
        {
            return Error.NotFound("Answer.NotFound", "Answer was not found.");
        }

        if (answer.UserId != userId)
        {
            return Error.Forbidden("Answer.Forbidden", "You can only delete your own answers.");
        }

        if (answer.IsAccepted)
        {
            return Error.Validation("Answer.DeleteAccepted", "A deleted answer cannot be the accepted answer — acceptance must be removed first.");
        }

        _answerRepository.Remove(answer);
        await _answerRepository.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
