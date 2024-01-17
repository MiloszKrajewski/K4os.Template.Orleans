using K4os.Template.Orleans.Hosting;
using Orleans.Clustering.Redis;
using Orleans.Configuration;
using Orleans.Persistence;

namespace K4os.Template.Orleans.Silo.Configuration.Extensions;

public static class RedisConfigExtensions
{
	public static RedisClusteringOptions Apply(
		this RedisClusteringOptions redisOptions, SiloConfig? config)
	{
		var endpoint = config?.Persistence?.RedisEndpoint ?? SiloConfig.DefaultRedisUri;
		(redisOptions.ConfigurationOptions ??= new()).ApplyUri(endpoint);
		return redisOptions;
	}

	public static RedisStorageOptions Apply(
		this RedisStorageOptions redisOptions, SiloConfig? config)
	{
		var endpoint = config?.Persistence?.RedisEndpoint ?? SiloConfig.DefaultRedisUri;
		(redisOptions.ConfigurationOptions ??= new()).ApplyUri(endpoint);
		return redisOptions;
	}
	
	public static RedisReminderTableOptions Apply(
		this RedisReminderTableOptions redisOptions, SiloConfig? config)
	{
		var endpoint = config?.Reminders?.RedisEndpoint ?? SiloConfig.DefaultRedisUri;
		(redisOptions.ConfigurationOptions ??= new()).ApplyUri(endpoint);
		return redisOptions;
	}
}
