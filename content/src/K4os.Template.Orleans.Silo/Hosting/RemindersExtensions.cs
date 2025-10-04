using System;
using K4os.Template.Orleans.Hosting;
using K4os.Template.Orleans.Hosting.Configuration;
using Orleans.Configuration;
using Orleans.Hosting;

namespace K4os.Template.Orleans.Silo.Hosting;

public static class RemindersExtensions
{
	public static RedisReminderTableOptions Apply(
		this RedisReminderTableOptions redisOptions, SiloConfig? config)
	{
		var endpoint = config?.Reminders?.RedisEndpoint ?? ConfigDefaults.DefaultRedisUri;
		(redisOptions.ConfigurationOptions ??= new()).ApplyUri(endpoint);
		return redisOptions;
	}

	public static ReminderOptions Apply(
		this ReminderOptions reminderOptions, SiloConfig? config)
	{
		reminderOptions.MinimumReminderPeriod = TimeSpan.Zero;
		return reminderOptions;
	}
}
