using Microsoft.Extensions.Logging;
using Orleans;

namespace K4os.Template.Orleans.Api.Middleware;

public class ConnectionRetryFilter: IClientConnectionRetryFilter
{
	protected readonly ILogger Log;

	public ConnectionRetryFilter(ILoggerFactory loggerFactory)
	{
		Log = loggerFactory.CreateLogger(GetType());
	}
	
	public async Task<bool> ShouldRetryConnectionAttempt(
		Exception exception, CancellationToken cancellationToken)
	{
		Log.LogWarning(exception, "Silo connection attempt failed");
		await Task.Delay(1000, cancellationToken);
		return !cancellationToken.IsCancellationRequested;
	}
}