using ErrorOr;
using Microsoft.AspNetCore.Http;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Domain.Entities;
using System.Security.Claims;

namespace StackOverflowLite.Application.Features.Answers.Commands.UpdateAnswer;

public class UpdateAnswerCommandHandler : ICommandHandler<UpdateAnswerCommand, ErrorOr<Success>>
{
    private readonly IRepository<Answer> _answerRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateAnswerCommandHandler(
        IRepository<Answer> answerRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _answerRepository = answerRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateAnswerCommand request, CancellationToken cancellationToken)
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
            return Error.Forbidden("Answer.Forbidden", "You can only edit your own answers.");
        }

        answer.Content = request.Content;
        answer.UpdatedAt = DateTime.UtcNow;

        _answerRepository.Update(answer);
        await _answerRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
