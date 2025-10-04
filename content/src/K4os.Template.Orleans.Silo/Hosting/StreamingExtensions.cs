using System;
using K4os.Template.Orleans.Hosting;
using K4os.Template.Orleans.Hosting.Configuration;
using Orleans.Configuration;
using Orleans.Hosting;

namespace K4os.Template.Orleans.Silo.Hosting;

public static class StreamingExtensions
{
	public static RedisStreamingOptions Apply(
		this RedisStreamingOptions redisOptions, SiloConfig? config)
	{
		var endpoint = config?.Streaming?.RedisEndpoint ?? ConfigDefaults.DefaultRedisUri;
		redisOptions.ConnectionOptions.ApplyUri(endpoint);
		return redisOptions;
	}
}
