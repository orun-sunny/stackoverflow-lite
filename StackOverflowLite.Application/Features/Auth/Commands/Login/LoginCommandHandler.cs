using ErrorOr;
using Microsoft.AspNetCore.Identity;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Dtos;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Application.Models;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, ErrorOr<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<UserProfile> _userProfileRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        IRepository<UserProfile> userProfileRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _userProfileRepository = userProfileRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ErrorOr<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");
        }

        var profile = await _userProfileRepository.FindFirstAsync(up => up.UserId == user.Id, false, cancellationToken);
        var displayName = profile?.DisplayName ?? user.UserName ?? "User";

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse(user.Id, user.Email ?? string.Empty, displayName, token);
    }
}
