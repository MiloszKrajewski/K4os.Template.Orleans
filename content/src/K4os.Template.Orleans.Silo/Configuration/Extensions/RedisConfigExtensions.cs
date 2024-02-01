using K4os.Template.Orleans.Hosting;
using K4os.Template.Orleans.Silo.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Clustering.Redis;
using Orleans.Configuration;
using Orleans.Persistence;
using Orleans.Serialization;
using Orleans.Storage;

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

	public static OptionsBuilder<RedisStorageOptions> Apply(
		this OptionsBuilder<RedisStorageOptions> builder, SiloConfig? config)
	{
		builder.Configure<IServiceProvider>(
			(redisOptions, services) => {
				var json = config?.Persistence?.UseJson ?? false;
				IGrainStorageSerializer serializer = json
					? CreateJsonSerializer(services)
					: CreateBinarySerializer(services);
				var endpoint = config?.Persistence?.RedisEndpoint ?? SiloConfig.DefaultRedisUri;
				(redisOptions.ConfigurationOptions ??= new()).ApplyUri(endpoint);
				redisOptions.GrainStorageSerializer = serializer;
			});
		return builder;
	}

	private static BinaryAsBase64Serializer CreateBinarySerializer(IServiceProvider services) =>
		new(new OrleansGrainStorageSerializer(services.GetRequiredService<Serializer>()));

	private static JsonGrainStorageSerializer CreateJsonSerializer(IServiceProvider services) =>
		new(services.GetRequiredService<OrleansJsonSerializer>());

	public static RedisReminderTableOptions Apply(
		this RedisReminderTableOptions redisOptions, SiloConfig? config)
	{
		var endpoint = config?.Reminders?.RedisEndpoint ?? SiloConfig.DefaultRedisUri;
		(redisOptions.ConfigurationOptions ??= new()).ApplyUri(endpoint);
		return redisOptions;
	}
}

