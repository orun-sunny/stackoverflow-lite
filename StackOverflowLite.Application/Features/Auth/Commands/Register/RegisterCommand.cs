using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Dtos;

namespace StackOverflowLite.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string DisplayName) : ICommand<ErrorOr<AuthResponse>>;
