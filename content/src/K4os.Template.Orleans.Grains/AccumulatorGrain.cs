using K4os.Template.Orleans.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;

namespace K4os.Template.Orleans.Grains;

public class AccumulatorState
{
	public int Count { get; set; }
	public double Sum { get; set; }
	public DateTime LastUpdated { get; set; }
}

public partial class AccumulatorGrain: Grain<AccumulatorState>, IAccumulatorGrain
{
	protected ILogger Log { get; }

	public AccumulatorGrain(ILogger<AccumulatorGrain> logger) { Log = logger; }

	public async Task<double> Add(double value)
	{
		Log.LogInformation("{GrainId}: adding {Value}", IdentityString, value);
		State.Sum += value;
		State.Count++;
		State.LastUpdated = DateTime.UtcNow;
		await WriteStateAsync();
		return State.Sum;
	}

	public Task<double> Sum() => Task.FromResult(State.Sum);
	public Task<double> Avg() => Task.FromResult(State.Count == 0 ? 0 : State.Sum / State.Count);
}
