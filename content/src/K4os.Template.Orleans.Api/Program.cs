using K4os.Template.Orleans.Api.Middleware;
using K4os.Template.Orleans.Hosting;
using K4os.Template.Orleans.Hosting.Configuration;
using K4os.Template.Orleans.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();
builder.ConfigureTelemetry();

var host = builder.Host;
host.UseConsoleLifetime();

host.UseOrleansClient(
    (context, orleans) =>
    {
        var config = context.Configuration.GetSection("Silo").Get<ClientConfig>();
        orleans.Configure<ClusterOptions>(cluster => cluster.Apply(config));
        orleans.UseRedisClustering(redis => redis.Apply(config));
    });

var services = builder.Services;
services.AddSingleton<IClientConnectionRetryFilter, ConnectionRetryFilter>();
services.AddEndpointsApiExplorer();
services.AddControllers();
services.AddOpenApi();

services.AddScoped<IDistributedLocksGateway>(
    p => p.GetRequiredService<IClusterClient>().GetGrain<IDistributedLocksGateway>(0));

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseHealthChecks("/health");

app.MapOpenApi();
app.UseMiddleware<WellKnownErrorHandler>();
app.MapControllers();

app.MapGet("/echo", (string? text) => Results.Ok(text ?? "<null>"));
app.MapGet("/time", () => Results.Ok(DateTime.UtcNow));
app.MapGet("/fail", (int? status = null) => Results.StatusCode(status ?? 500));

app.MapPost(
    "/locks/{name}", async (
        [FromServices] IDistributedLocksGateway manager,
        [FromRoute] string name,
        [FromQuery] double? timeout
    ) => {
        var receipt = await manager.Acquire(name, TimeoutFromSeconds(timeout));
        return Results.Ok(receipt);
    }
);

app.MapPut(
    "/locks/{name}/{receiptId:guid}", async (
        [FromServices] IDistributedLocksGateway manager,
        [FromRoute] string name,
        [FromRoute] Guid receiptId,
        [FromQuery] double? timeout
    ) => {
        var receipt = await manager.Renew(name, receiptId, TimeoutFromSeconds(timeout));
        return Results.Ok(receipt);
    });

app.MapDelete(
    "/locks/{name}/{receiptId:guid}", async (
        [FromServices] IDistributedLocksGateway manager,
        [FromRoute] string name,
        [FromRoute] Guid receiptId
    ) => {
        await manager.Release(name, receiptId);
        return Results.NoContent();
    }
);

app.Run();
return;

TimeSpan? TimeoutFromSeconds(double? timeout) => 
    timeout is null ? null : TimeSpan.FromSeconds(timeout.Value);
