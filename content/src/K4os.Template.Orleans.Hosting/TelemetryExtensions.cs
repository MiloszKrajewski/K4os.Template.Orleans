using System.Reflection;
using K4os.Template.Orleans.Hosting.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace K4os.Template.Orleans.Hosting;

public static class TelemetryExtensions
{
	private const string OTEL_EXPORTER_OTLP_ENDPOINT = "OTEL_EXPORTER_OTLP_ENDPOINT";
	
	private const string CONSOLE_LOG_TEMPLATE = 
		"[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}";

	private static readonly string ApplicationName =
		Assembly.GetEntryAssembly()?.GetName().Name!;
	
	private static Uri GetTelemetryEndpoint(this IConfiguration config)
	{
		var endpoint = 
			config.GetConnectionString("Otlp") ??
			config.GetValue<string>(OTEL_EXPORTER_OTLP_ENDPOINT) ??
			Environment.GetEnvironmentVariable(OTEL_EXPORTER_OTLP_ENDPOINT);
		return endpoint is null ? ConfigDefaults.DefaultTelemetryUri : new(endpoint);
	}

	public static void ConfigureLogging(
		this IHostBuilder hostBuilder)
	{
		hostBuilder.UseSerilog((context, logging) => ConfigureLogging(
			logging, context.Configuration));
	}

	public static void ConfigureLogging(
		this IHostApplicationBuilder builder)
	{
		builder.Services.AddSerilog((provider, logging) => ConfigureLogging(
			logging, provider.GetRequiredService<IConfiguration>()));
	}
	
	private static void ConfigureLogging(LoggerConfiguration logging, IConfiguration config)
	{
		var telemetryEndpoint = config.GetTelemetryEndpoint();
		logging
			.ReadFrom.Configuration(config)
			.Enrich.FromLogContext()
			.WriteTo.Console(outputTemplate: CONSOLE_LOG_TEMPLATE)
			.WriteTo.OpenTelemetry(x => {
				x.Endpoint = telemetryEndpoint.ToString();
				x.ResourceAttributes = new Dictionary<string, object> {
					{ "service.name", ApplicationName }
				};
				x.IncludedData =
					IncludedData.TraceIdField |
					IncludedData.SpanIdField |
					IncludedData.MessageTemplateMD5HashAttribute;
			});
	}
	
	public static void ConfigureTelemetry(
		this IHostApplicationBuilder builder)
	{
		var services = builder.Services;
		var config = builder.Configuration;
		var telemetryConfig = 
			config.GetSection("Telemetry").Get<TelemetryConfig>() ?? 
			new TelemetryConfig();
		var telemetryEndpoint = config.GetTelemetryEndpoint();

		services.AddMetrics();
		services
			.AddOpenTelemetry()
			.ConfigureResource(r => r.AddService(ApplicationName))
			.WithLogging()
			.WithMetrics(x => x
				.AddOtlpExporter(e => e.Endpoint = telemetryEndpoint)
				.AddRuntimeInstrumentation()
				.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				.AddMeter(telemetryConfig.Meters ?? []))
			.WithTracing(x => x
				.AddOtlpExporter(e => e.Endpoint = telemetryEndpoint)
				.SetSampler(CreateSampler(telemetryConfig.SamplingRatio))
				.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				.AddSource(telemetryConfig.Traces ?? []));
		services
			.AddHealthChecks()
			.AddCheck("default", () => HealthCheckResult.Healthy());
	}

	private static Sampler CreateSampler(double? ratio) =>
		ratio switch {
			null or <= 0 => new AlwaysOffSampler(),
			>= 1 => new AlwaysOnSampler(),
			_ => new TraceIdRatioBasedSampler(ratio.Value)
		};
}