using K4os.Template.Orleans.Interfaces;

namespace K4os.Template.Orleans.Grains;

public class SimpleGrain: ISimpleGrain
{
	public async Task<string> Return(string message)
	{
		await Task.CompletedTask;

		return message;
	}

	public async Task Fail(string? message)
	{
		await Task.CompletedTask;

		if (message is null)
			return;

		throw new KnownException(message);
	}

	public async Task<DateTime> Delay(TimeSpan delay)
	{
		await Task.CompletedTask;

		if (delay > TimeSpan.Zero)
			await Task.Delay(delay);

		return DateTime.UtcNow;
	}
}
