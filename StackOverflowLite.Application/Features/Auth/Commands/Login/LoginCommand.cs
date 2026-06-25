using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Dtos;

namespace StackOverflowLite.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) : ICommand<ErrorOr<AuthResponse>>;
