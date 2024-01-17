using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;

namespace K4os.Template.Orleans.Hosting;

public static class SerilogExtensions
{
	public static void BootstrapSerilog(this IHostBuilder builder)
	{
		builder.UseSerilog(Configure);
	}

	private static void Configure(
		HostBuilderContext context,
		IServiceProvider provider,
		LoggerConfiguration logging)
	{
		const string outputTemplate =
			"[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}";
		var targetFile = Path.Combine(
			AppContext.BaseDirectory, "Logs", context.HostingEnvironment.ApplicationName + "-.json");
		logging
			.ReadFrom.Configuration(context.Configuration)
			.WriteTo.Console(outputTemplate: outputTemplate)
			.WriteTo.File(
				new RenderedCompactJsonFormatter(), targetFile,
				rollingInterval: RollingInterval.Day,
				fileSizeLimitBytes: null);
	}
}
