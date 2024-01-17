using Orleans;

namespace K4os.Template.Orleans.Interfaces;

public interface ISimpleGrain: IGrainWithIntegerKey
{
	public Task<string> Return(string message);
	public Task Fail(string? message);
	public Task<DateTime> Delay(TimeSpan delay);
}