using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackOverflowLite.Application.Interfaces;
using StackOverflowLite.Application.Models;
using StackOverflowLite.Infrastructure.Data.DbContext;
using StackOverflowLite.Infrastructure.Data.Repositories;
using StackOverflowLite.Infrastructure.Services;

namespace StackOverflowLite.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("StackOverflowLite.Infrastructure")));

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // var redisConnection = configuration.GetConnectionString("Redis")
        //     ?? configuration["Redis:ConnectionString"]
        //     ?? "localhost:6379";

        // services.AddStackExchangeRedisCache(options =>
        // {
        //     options.Configuration = redisConnection;
        
        //     options.InstanceName = "StackOverflowLite:";
        // });
        services.AddDistributedMemoryCache();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IHealthProbeService, HealthProbeService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddHostedService<DatabaseMigrationHostedService>();

        return services;
    }
}

public class DatabaseMigrationHostedService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Console.WriteLine("Applying database migrations...");
        await context.Database.MigrateAsync(cancellationToken);
        Console.WriteLine("Database migrations applied.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
