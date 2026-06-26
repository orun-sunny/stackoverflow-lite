using ErrorOr;
using Microsoft.Extensions.Caching.Distributed;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Dtos;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Application.Features.Questions.Queries.GetQuestionById;

public class GetQuestionByIdQueryHandler : IQueryHandler<GetQuestionByIdQuery, ErrorOr<QuestionDto>>
{
    private readonly IRepository<Question> _questionRepository;
    private readonly IDistributedCache _cache;

    public GetQuestionByIdQueryHandler(IRepository<Question> questionRepository, IDistributedCache cache)
    {
        _questionRepository = questionRepository;
        _cache = cache;
    }

    public async Task<ErrorOr<QuestionDto>> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.GetByIdAsync(
            request.Id,
            false,
            cancellationToken,
            q => q.Tags);

        if (question is null)
        {
            return Error.NotFound("Question.NotFound", "Question was not found.");
        }

        //tracking using IDistributedCache
        var viewKey = $"question:views:{question.Id}";
        var viewsString = await _cache.GetStringAsync(viewKey, cancellationToken);
        long views = 0;
        if (!string.IsNullOrEmpty(viewsString) && long.TryParse(viewsString, out var currentViews))
        {
            views = currentViews;
        }
        views++;
        await _cache.SetStringAsync(viewKey, views.ToString(), cancellationToken);

        return new QuestionDto(
            question.Id,
            question.UserId,
            question.Title,
            question.Description,
            question.Tags.Select(t => t.Name).ToList(),
            views,
            question.CreatedAt,
            question.UpdatedAt);
    }
}
