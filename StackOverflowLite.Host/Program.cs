using StackOverflowLite.Application;
using StackOverflowLite.Host;
using StackOverflowLite.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddPresentationServices(builder.Configuration);

var app = builder.Build();
app.UsePresentationPipeline();

app.Run();
