using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace K4os.Template.Orleans.Silo.Internal;

public class BackgroundLoop: BackgroundService
{
	protected ILogger Log { get; }

	public BackgroundLoop(ILogger<BackgroundLoop> logger) { Log = logger; }

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		try
		{
			while (true)
			{
				Log.LogInformation("Background loop is still running");
				await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
			}
		}
		catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
		{
			Log.LogInformation("Background loop finished");
		}
		catch (Exception error)
		{
			Log.LogError(error, "Background loop failed");
		}
	}
}
