using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackOverflowLite.Application.Features.Health.Queries.GetHealth;

namespace StackOverflowLite.Host.Controllers;

public class HealthController : BaseApiController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<HealthDto>> Get()
        => ToActionResult(await Mediator.Send(new GetHealthQuery()));
}
