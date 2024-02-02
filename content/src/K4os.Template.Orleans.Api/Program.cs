using K4os.Template.Orleans.Api.Middleware;
using K4os.Template.Orleans.Hosting;
using K4os.Template.Orleans.Hosting.Configuration;
using K4os.Template.Orleans.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var host = builder.Host;
host.UseConsoleLifetime();
host.BootstrapSerilog();

var services = builder.Services;
services.AddSingleton<IClientConnectionRetryFilter, ConnectionRetryFilter>();
services.AddTransient<ISimpleGrain>(
	p => p.GetRequiredService<IClusterClient>().GetGrain<ISimpleGrain>(0));

host.UseOrleansClient(
	(context, orleans) => {
		var config = context.Configuration.GetSection("Silo").Get<ClientConfig>();
		orleans.Configure<ClusterOptions>(cluster => cluster.Apply(config));
		orleans.UseRedisClustering(redis => redis.Apply(config));
	});

services.AddHealthChecks();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddOpenTelemetry().WithMetrics(b => b.AddPrometheusExporter());

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseHealthChecks("/health");
app.UseOpenTelemetryPrometheusScrapingEndpoint();

if (builder.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
else
{
	app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapGet("/now", (ISimpleGrain grain) => grain.Delay(TimeSpan.Zero));
app.MapGet(
	"/delay", (ISimpleGrain grain, int delay) => grain.Delay(TimeSpan.FromMilliseconds(delay)));
app.MapGet("/echo", (ISimpleGrain grain, string? text) => grain.Return(text ?? ""));
app.MapGet("/fail", (ISimpleGrain grain, string? message) => grain.Fail(message));

app.MapGet(
	"/acc/{id}/sum",
	(IClusterClient client, string id) => 
		client.GetGrain<IAccumulatorGrain>(id).Sum());
app.MapGet(
	"/acc/{id}/avg",
	(IClusterClient client, string id) => 
		client.GetGrain<IAccumulatorGrain>(id).Avg());
app.MapPut(
	"/acc/{id}/add",
	(IClusterClient client, string id, double value) =>
		client.GetGrain<IAccumulatorGrain>(id).Add(value));

app.MapControllers();

app.Run();
