using K4os.Template.Orleans.Hosting;
using Orleans.Clustering.Redis;

namespace K4os.Template.Orleans.Api.Configuration.Extensions;

public static class RedisConfigExtensions
{
	public static RedisClusteringOptions Apply(
		this RedisClusteringOptions redisOptions, SiloConfig? config)
	{
		var endpoint = config?.Cluster?.RedisEndpoint ?? SiloConfig.DefaultRedisUri;
		(redisOptions.ConfigurationOptions ??= new()).ApplyUri(endpoint);
		return redisOptions;
	}
}
