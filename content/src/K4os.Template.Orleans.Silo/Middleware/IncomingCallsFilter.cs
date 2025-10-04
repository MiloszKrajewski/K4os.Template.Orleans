using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Orleans;

namespace K4os.Template.Orleans.Silo.Middleware;

public class IncomingCallsFilter: IIncomingGrainCallFilter
{
	private readonly ILoggerFactory _loggerFactory;

	public IncomingCallsFilter(ILoggerFactory loggerFactory) => 
		_loggerFactory = loggerFactory;

	public async Task Invoke(IIncomingGrainCallContext context)
	{
		var type = context.InterfaceMethod.DeclaringType;
		
		var log = _loggerFactory.CreateLogger(type ?? typeof(IncomingCallsFilter));

		var typeName = type?.Name ?? "<unknown>";
		var grainKey = context.TargetContext.GrainId.Key.ToString();
		var methodName = context.InterfaceMethod.Name;
		
		var stopwatch = Stopwatch.StartNew();
		log.LogDebug("{GrainType}({GrainKey}).{Method} invoked", typeName, grainKey, methodName);
		try
		{
			await context.Invoke();
			log.LogInformation(
				"{GrainType}({GrainKey}).{Method} finished in {Elapsed:0.00}ms", 
				type, grainKey, methodName, stopwatch.Elapsed.TotalMilliseconds);
		}
		catch (Exception e)
		{
			log.LogWarning(
				e,
				"{GrainType}({GrainKey}).{Method} failed after {Elapsed:0.00}ms", 
				type, grainKey, methodName, stopwatch.Elapsed.TotalMilliseconds);
			throw;
		}
	}
}
