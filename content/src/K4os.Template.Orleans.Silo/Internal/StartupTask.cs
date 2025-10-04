using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace K4os.Template.Orleans.Silo.Internal;

public class StartupTask: IStartupTask
{
	private readonly ILogger _logger;

	public StartupTask(ILogger<StartupTask> logger)
	{
		_logger = logger;
	}

	public Task Execute(CancellationToken cancellationToken)
	{
		_logger.LogInformation($"{nameof(StartupTask)} finished");
		return Task.CompletedTask;
	}
}
