using ErrorOr;
using Microsoft.Extensions.Caching.Distributed;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Dtos;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Application.Features.Questions.Queries.GetQuestions;

public class GetQuestionsQueryHandler : IQueryHandler<GetQuestionsQuery, ErrorOr<IEnumerable<QuestionDto>>>
{
    private readonly IRepository<Question> _questionRepository;
    private readonly IDistributedCache _cache;

    public GetQuestionsQueryHandler(IRepository<Question> questionRepository, IDistributedCache cache)
    {
        _questionRepository = questionRepository;
        _cache = cache;
    }

    public async Task<ErrorOr<IEnumerable<QuestionDto>>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Question> questions;

        if (!string.IsNullOrEmpty(request.Tag))
        {
            var normalizedTag = request.Tag.ToLower().Trim();
            questions = await _questionRepository.FindAsync(
                q => q.Tags.Any(t => t.Name == normalizedTag),
                false,
                cancellationToken,
                q => q.Tags);
        }
        else
        {
            questions = await _questionRepository.FindAsync(
                _ => true,
                false,
                cancellationToken,
                q => q.Tags);
        }

        var questionDtos = new List<QuestionDto>();
        foreach (var question in questions)
        {
            var viewKey = $"question:views:{question.Id}";
            var viewsString = await _cache.GetStringAsync(viewKey, cancellationToken);
            long views = 0;
            if (!string.IsNullOrEmpty(viewsString) && long.TryParse(viewsString, out var currentViews))
            {
                views = currentViews;
            }

            questionDtos.Add(new QuestionDto(
                question.Id,
                question.UserId,
                question.Title,
                question.Description,
                question.Tags.Select(t => t.Name).ToList(),
                views,
                question.CreatedAt,
                question.UpdatedAt));
        }

        return questionDtos;
    }
}
