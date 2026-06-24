using ErrorOr;
using StackOverflowLite.Application.Core.Abstractions;

namespace StackOverflowLite.Application.Features.Health.Queries.GetHealth;

public record GetHealthQuery() : IQuery<ErrorOr<HealthDto>>;

public record HealthDto(string Status, bool Database, bool Redis, DateTime Timestamp);
