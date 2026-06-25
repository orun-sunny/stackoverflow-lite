using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Dtos;

namespace StackOverflowLite.Application.Features.Auth.Queries.GetProfile;

public record GetProfileQuery(Guid UserId) : IQuery<ErrorOr<UserProfileResponse>>;
