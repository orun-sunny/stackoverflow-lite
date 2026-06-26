using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackOverflowLite.Application.Dtos;
using StackOverflowLite.Application.Features.Answers.Commands.AcceptAnswer;
using StackOverflowLite.Application.Features.Answers.Commands.CreateAnswer;
using StackOverflowLite.Application.Features.Answers.Commands.DeleteAnswer;
using StackOverflowLite.Application.Features.Answers.Commands.UpdateAnswer;
using StackOverflowLite.Application.Features.Answers.Queries.GetAnswersByQuestionId;

namespace StackOverflowLite.Host.Controllers;

[ApiController]
public class AnswersController : BaseApiController
{
    [Authorize]
    [HttpPost("api/answers")]
    public async Task<ActionResult<Guid>> Create(CreateAnswerCommand command)
    {
        var result = await Mediator.Send(command);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpPut("api/answers/{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateAnswerCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in route path must match ID in the body.");
        }

        var result = await Mediator.Send(command);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpDelete("api/answers/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteAnswerCommand(id));
        return ToActionResult(result);
    }

    [HttpGet("api/questions/{questionId}/answers")]
    public async Task<ActionResult<AnswerResponseDto>> GetByQuestionId(Guid questionId)
    {
        var result = await Mediator.Send(new GetAnswersByQuestionIdQuery(questionId));
        return ToActionResult(result);
    }

    [Authorize]
    [HttpPut("api/questions/{questionId}/accept-answer")]
    public async Task<IActionResult> AcceptAnswer(Guid questionId, AcceptAnswerCommand command)
    {
        if (questionId != command.QuestionId)
        {
            return BadRequest("Question ID in route path must match Question ID in the body.");
        }

        var result = await Mediator.Send(command);
        return ToActionResult(result);
    }
}
