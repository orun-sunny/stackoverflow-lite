using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;
using StackOverflowLite.Application.Interfaces;

namespace StackOverflowLite.Application.Features.Health.Queries.GetHealth;

public class GetHealthHandler(IHealthProbeService healthProbe)
    : IQueryHandler<GetHealthQuery, ErrorOr<HealthDto>>
{
    public async Task<ErrorOr<HealthDto>> Handle(GetHealthQuery request, CancellationToken cancellationToken)
    {
        var (database, redis) = await healthProbe.ProbeAsync(cancellationToken);
        var status = database && redis ? "Healthy" : "Degraded";

        return new HealthDto(status, database, redis, DateTime.UtcNow);
    }
}
