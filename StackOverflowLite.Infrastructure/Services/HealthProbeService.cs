using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Infrastructure.Data.DbContext;

namespace StackOverflowLite.Infrastructure.Services;

public class HealthProbeService(
    ApplicationDbContext dbContext,
    IDistributedCache cache) : IHealthProbeService
{
    public async Task<(bool Database, bool Redis)> ProbeAsync(CancellationToken cancellationToken = default)
    {
        var database = false;
        var redis = false;

        try
        {
            database = await dbContext.Database.CanConnectAsync(cancellationToken);
        }
        catch
        {
            database = false;
        }

        try
        {
            var key = "health:probe";
            await cache.SetStringAsync(key, "ok", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
            }, cancellationToken);

            var value = await cache.GetStringAsync(key, cancellationToken);
            redis = value == "ok";
        }
        catch
        {
            redis = false;
        }

        return (database, redis);
    }
}
