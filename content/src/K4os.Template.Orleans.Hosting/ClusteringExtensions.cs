using K4os.Template.Orleans.Hosting.Configuration;
using Orleans.Clustering.Redis;
using Orleans.Configuration;

namespace K4os.Template.Orleans.Hosting;

public static class ClusteringExtensions
{
	private static ClusterOptions Apply(
		ClusterOptions clusterOptions, 
		ClientConfig.ClusterConfig? config)
	{
		var clusterId = config?.ClusterId ?? ConfigDefaults.ClusterName;
		var serviceId = config?.ServiceId ?? "Silo";
		clusterOptions.ClusterId = clusterId;
		clusterOptions.ServiceId = $"{clusterId}/{serviceId}";

		return clusterOptions;
	}
	
	private static RedisClusteringOptions Apply(
		RedisClusteringOptions redisOptions, ClientConfig.ClusterConfig? config)
	{
		var endpoint = config?.RedisEndpoint ?? ConfigDefaults.DefaultRedisUri;
		(redisOptions.ConfigurationOptions ??= new()).ApplyUri(endpoint);
		return redisOptions;
	}

	public static ClusterOptions Apply(
		this ClusterOptions clusterOptions, ClientConfig? config) =>
		Apply(clusterOptions, config?.Cluster);

	public static ClusterOptions Apply(
		this ClusterOptions clusterOptions, SiloConfig? config) =>
		Apply(clusterOptions, config?.Cluster);

	public static RedisClusteringOptions Apply(
		this RedisClusteringOptions redisOptions, ClientConfig? config) =>
		Apply(redisOptions, config?.Cluster);

	public static RedisClusteringOptions Apply(
		this RedisClusteringOptions redisOptions, SiloConfig? config) =>
		Apply(redisOptions, config?.Cluster);
}
