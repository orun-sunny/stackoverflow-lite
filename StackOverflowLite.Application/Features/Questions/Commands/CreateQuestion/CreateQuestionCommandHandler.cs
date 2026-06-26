using ErrorOr;
using Microsoft.AspNetCore.Http;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Domain.Entities;
using System.Security.Claims;

namespace StackOverflowLite.Application.Features.Questions.Commands.CreateQuestion;

public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand, ErrorOr<Guid>>
{
    private readonly IRepository<Question> _questionRepository;
    private readonly IRepository<Tag> _tagRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateQuestionCommandHandler(
        IRepository<Question> questionRepository,
        IRepository<Tag> tagRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _questionRepository = questionRepository;
        _tagRepository = tagRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst("userid")?.Value 
                        ?? _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Error.Unauthorized("Auth.Unauthorized", "User is not authenticated.");
        }

        var tagsList = new List<Tag>();
        if (request.Tags != null && request.Tags.Any())
        {
            foreach (var tagStr in request.Tags)
            {
                var normalizedTagName = tagStr.ToLower().Trim();
                if (string.IsNullOrEmpty(normalizedTagName)) continue;

                var existingTag = await _tagRepository.FindFirstAsync(
                    t => t.Name == normalizedTagName,
                    true, // Track changes
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

        var question = new Question
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Tags = tagsList
        };

        await _questionRepository.AddAsync(question, cancellationToken);
        await _questionRepository.SaveChangesAsync(cancellationToken);

        return question.Id;
    }
}
