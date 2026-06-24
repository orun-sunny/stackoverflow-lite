namespace StackOverflowLite.Application.Interfaces;

public interface IHealthProbeService
{
    Task<(bool Database, bool Redis)> ProbeAsync(CancellationToken cancellationToken = default);
}
