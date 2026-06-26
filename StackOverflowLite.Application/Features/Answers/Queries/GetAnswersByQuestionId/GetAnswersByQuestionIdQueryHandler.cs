using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Dtos;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Application.Features.Answers.Queries.GetAnswersByQuestionId;

public class GetAnswersByQuestionIdQueryHandler : IQueryHandler<GetAnswersByQuestionIdQuery, ErrorOr<AnswerResponseDto>>
{
    private readonly IRepository<Answer> _answerRepository;

    public GetAnswersByQuestionIdQueryHandler(IRepository<Answer> answerRepository)
    {
        _answerRepository = answerRepository;
    }

    public async Task<ErrorOr<AnswerResponseDto>> Handle(GetAnswersByQuestionIdQuery request, CancellationToken cancellationToken)
    {
        var answers = await _answerRepository.FindAsync(
            a => a.QuestionId == request.QuestionId,
            false,
            cancellationToken);

        var acceptedAnswer = answers
            .Where(a => a.IsAccepted)
            .Select(a => new AnswerDto(a.Id, a.QuestionId, a.UserId, a.Content, a.IsAccepted, a.CreatedAt, a.UpdatedAt))
            .FirstOrDefault();

        var otherAnswers = answers
            .Where(a => !a.IsAccepted)
            .Select(a => new AnswerDto(a.Id, a.QuestionId, a.UserId, a.Content, a.IsAccepted, a.CreatedAt, a.UpdatedAt))
            .ToList();

        return new AnswerResponseDto(acceptedAnswer, otherAnswers);
    }
}
