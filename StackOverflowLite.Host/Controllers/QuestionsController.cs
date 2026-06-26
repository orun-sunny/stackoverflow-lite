using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackOverflowLite.Application.Dtos;
using StackOverflowLite.Application.Features.Questions.Commands.CreateQuestion;
using StackOverflowLite.Application.Features.Questions.Commands.DeleteQuestion;
using StackOverflowLite.Application.Features.Questions.Commands.UpdateQuestion;
using StackOverflowLite.Application.Features.Questions.Queries.GetQuestionById;
using StackOverflowLite.Application.Features.Questions.Queries.GetQuestions;

namespace StackOverflowLite.Host.Controllers;

[Route("api/questions")]
public class QuestionsController : BaseApiController
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateQuestionCommand command)
    {
        var result = await Mediator.Send(command);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateQuestionCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in route path must match ID in the body.");
        }

        var result = await Mediator.Send(command);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteQuestionCommand(id));
        return ToActionResult(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionDto>> Get(Guid id)
    {
        var result = await Mediator.Send(new GetQuestionByIdQuery(id));
        return ToActionResult(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAll([FromQuery] string? tag)
    {
        var result = await Mediator.Send(new GetQuestionsQuery(tag));
        return ToActionResult(result);
    }
}
