using ErrorOr;
using Microsoft.AspNetCore.Identity;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Dtos;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Application.Models;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, ErrorOr<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<UserProfile> _userProfileRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IRepository<UserProfile> userProfileRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _userProfileRepository = userProfileRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ErrorOr<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return Error.Conflict("Auth.DuplicateEmail", "Email already in use.");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            RegisteredAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => Error.Validation("Auth.RegistrationFailed", e.Description))
                .ToList();
            return errors;
        }

        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            DisplayName = request.DisplayName,
            Reputation = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _userProfileRepository.AddAsync(profile, cancellationToken);
        await _userProfileRepository.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse(user.Id, user.Email ?? string.Empty, profile.DisplayName, token);
    }
}
