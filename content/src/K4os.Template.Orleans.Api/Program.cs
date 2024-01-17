using K4os.Template.Orleans.Api.Configuration;
using K4os.Template.Orleans.Api.Configuration.Extensions;
using K4os.Template.Orleans.Api.Middleware;
using K4os.Template.Orleans.Hosting;
using K4os.Template.Orleans.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.BootstrapSerilog();

builder.Services.AddSingleton<IClientConnectionRetryFilter, ConnectionRetryFilter>();
builder.Services.AddTransient<ISimpleGrain>(
	p => p.GetRequiredService<IClusterClient>().GetGrain<ISimpleGrain>(0));

builder.Host.UseOrleansClient(
	(context, orleans) => {
		var config = context.Configuration.GetSection("Silo").Get<SiloConfig>();
		orleans.Configure<ClusterOptions>(cluster => cluster.Apply(config));
		orleans.UseRedisClustering(redis => redis.Apply(config));
	});

builder.Services.AddHealthChecks().ForwardToPrometheus();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseHealthChecks("/health");
app.UseMetricServer();

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
