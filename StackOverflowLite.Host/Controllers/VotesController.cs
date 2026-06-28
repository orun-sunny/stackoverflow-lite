using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackOverflowLite.Application.Features.Votes.Commands.CastVote;

namespace StackOverflowLite.Host.Controllers;

[Authorize]
[Route("api/votes")]
public class VotesController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Vote(CastVoteCommand command)
    {
        var result = await Mediator.Send(command);
        return ToActionResult(result);
    }
}
