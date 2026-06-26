using ErrorOr;
using Microsoft.AspNetCore.Http;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Domain.Entities;
using System.Security.Claims;

namespace StackOverflowLite.Application.Features.Questions.Commands.UpdateQuestion;

public class UpdateQuestionCommandHandler : ICommandHandler<UpdateQuestionCommand, ErrorOr<Success>>
{
    private readonly IRepository<Question> _questionRepository;
    private readonly IRepository<Tag> _tagRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateQuestionCommandHandler(
        IRepository<Question> questionRepository,
        IRepository<Tag> tagRepository,
        IHttpContextAccessor _httpContextAccessor)
    {
        _questionRepository = questionRepository;
        _tagRepository = tagRepository;
        this._httpContextAccessor = _httpContextAccessor;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
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
            cancellationToken,
            q => q.Tags);

        if (question is null)
        {
            return Error.NotFound("Question.NotFound", "Question was not found.");
        }

        if (question.UserId != userId)
        {
            return Error.Forbidden("Question.Forbidden", "You can only modify your own questions.");
        }

        question.Title = request.Title;
        question.Description = request.Description;
        question.UpdatedAt = DateTime.UtcNow;

        var tagsList = new List<Tag>();
        if (request.Tags != null)
        {
            foreach (var tagStr in request.Tags)
            {
                var normalizedTagName = tagStr.ToLower().Trim();
                if (string.IsNullOrEmpty(normalizedTagName)) continue;

                var existingTag = await _tagRepository.FindFirstAsync(
                    t => t.Name == normalizedTagName,
                    true,
                    cancellationToken);

                if (existingTag is null)
                {
                    var newTag = new Tag
                    {
                        Id = Guid.NewGuid(),
                        Name = normalizedTagName
                    };
                    await _tagRepository.AddAsync(newTag, cancellationToken);
                    tagsList.Add(newTag);
                }
                else
                {
                    tagsList.Add(existingTag);
                }
            }
        }

        question.Tags = tagsList;

        _questionRepository.Update(question);
        await _questionRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
