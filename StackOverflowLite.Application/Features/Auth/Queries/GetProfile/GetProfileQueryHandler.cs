using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Dtos;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Application.Features.Auth.Queries.GetProfile;

public class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, ErrorOr<UserProfileResponse>>
{
    private readonly IRepository<UserProfile> _userProfileRepository;

    public GetProfileQueryHandler(IRepository<UserProfile> userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<ErrorOr<UserProfileResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _userProfileRepository.FindFirstAsync(
            up => up.UserId == request.UserId,
            false,
            cancellationToken);

        if (profile is null)
        {
            return Error.NotFound("Profile.NotFound", "User profile was not found.");
        }

        return new UserProfileResponse(
            profile.UserId,
            profile.DisplayName,
            profile.Reputation,
            profile.CreatedAt);
    }
}
