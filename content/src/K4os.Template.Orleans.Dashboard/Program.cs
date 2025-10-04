using K4os.Template.Orleans.Dashboard.Hosting;
using K4os.Template.Orleans.Hosting;
using K4os.Template.Orleans.Hosting.Configuration;
using Microsoft.AspNetCore.Builder;
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
host.UseOrleans(
	(context, silo) => {
		var config = context.Configuration.GetSection("Silo").Get<SiloConfig>();
		silo
			.Configure<ClusterMembershipOptions>(cluster => cluster.Apply(config))
			.Configure<ClusterOptions>(cluster => cluster.Apply(config))
			.Configure<EndpointOptions>(endpoints => endpoints.Apply(config))
			.UseDashboard(dashboard => dashboard.HostSelf = true)
			.UseRedisClustering(redis => redis.Apply(config));
	});

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseHealthChecks("/health");
app.Map("/dashboard", x => x.UseOrleansDashboard());

app.Run();
