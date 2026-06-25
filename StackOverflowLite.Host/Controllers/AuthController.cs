using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackOverflowLite.Application.Dtos;
using StackOverflowLite.Application.Features.Auth.Commands.Login;
using StackOverflowLite.Application.Features.Auth.Commands.Register;
using StackOverflowLite.Application.Features.Auth.Queries.GetProfile;
using System.Security.Claims;

namespace StackOverflowLite.Host.Controllers;

[Route("api/auth")]
public class AuthController : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterCommand command)
    {
        var result = await Mediator.Send(command);
        return ToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return ToActionResult(result);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileResponse>> GetProfile()
    {
        var userIdStr = User.FindFirst("userid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var result = await Mediator.Send(new GetProfileQuery(userId));
        return ToActionResult(result);
    }
}
