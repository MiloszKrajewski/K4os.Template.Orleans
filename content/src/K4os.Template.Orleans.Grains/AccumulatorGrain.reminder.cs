using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace K4os.Template.Orleans.Grains;

public partial class AccumulatorGrain: IRemindable
{
	public override async Task OnActivateAsync(CancellationToken cancellationToken)
	{
		await base.OnActivateAsync(cancellationToken);
		await this.RegisterOrUpdateReminder(
			"tick", TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(15));
	}

	public Task ReceiveReminder(string reminderName, TickStatus status) =>
		reminderName switch
		{
			"tick" => HandleTick(),
			_ => Task.CompletedTask,
		};

	private async Task HandleTick()
	{
		var age = DateTime.UtcNow - State.LastUpdated;
		var count = State.Count;
		if (age < TimeSpan.FromMinutes(5))
		{
			Log.LogDebug("{GrainId}: keeping state due to young age", IdentityString);
			return;
		}
		
		await this.UnregisterReminder(await this.GetReminder("tick"));

		if (count >= 5)
		{
			Log.LogInformation("{GrainId}: keeping state at enough values were provided", IdentityString);
			return;
		}

		Log.LogWarning("{GrainId}: deactivating due to inactivity", IdentityString);
		await ClearStateAsync();
		DeactivateOnIdle();
	}
}
