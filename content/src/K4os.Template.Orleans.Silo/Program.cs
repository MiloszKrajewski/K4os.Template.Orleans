using K4os.Template.Orleans.Hosting;
using K4os.Template.Orleans.Silo.Configuration;
using K4os.Template.Orleans.Silo.Configuration.Extensions;
using K4os.Template.Orleans.Silo.Internal;
using K4os.Template.Orleans.Silo.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.BootstrapSerilog();
builder.Services.AddHealthChecks();
builder.Services.AddHostedService<BackgroundLoop>();

builder.Host.UseOrleans(
	(context, silo) => {
		var config = context.Configuration.GetSection("Silo").Get<SiloConfig>();
		silo
			.Configure<ClusterOptions>(cluster => cluster.Apply(config))
			.Configure<EndpointOptions>(endpoints => endpoints.Apply(config))
			.Configure<ReminderOptions>(reminders => reminders.Apply(config))
			.UseDashboard(dashboard => dashboard.HostSelf = true)
			.UseRedisClustering(redis => redis.Apply(config))
			.AddRedisGrainStorageAsDefault(redis => redis.Apply(config))
			.UseRedisReminderService(redis => redis.Apply(config))
			.AddReminders()
			.AddIncomingGrainCallFilter<IncomingCallsFilter>()
			.AddStartupTask<StartupTask>();
	});

var app = builder.Build();

app.UseHealthChecks("/health");
app.Map("/dashboard", x => x.UseOrleansDashboard());

app.UseSerilogRequestLogging();

app.Run();