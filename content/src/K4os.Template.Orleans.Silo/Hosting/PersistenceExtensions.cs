using System;
using K4os.Template.Orleans.Hosting;
using K4os.Template.Orleans.Hosting.Configuration;
using K4os.Template.Orleans.Silo.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Persistence;
using Orleans.Serialization;
using Orleans.Storage;

namespace K4os.Template.Orleans.Silo.Hosting;

public static class PersistenceExtensions
{
	public static OptionsBuilder<RedisStorageOptions> Apply(
		this OptionsBuilder<RedisStorageOptions> builder, SiloConfig? config)
	{
		builder.Configure<IServiceProvider>(
			(redisOptions, services) => {
				var json = config?.Persistence?.UseBinary ?? false;
				IGrainStorageSerializer serializer = json 
					? CreateJsonSerializer(services) 
					: CreateNativeSerializer(services);
				var endpoint = config?.Persistence?.RedisEndpoint ?? ConfigDefaults.DefaultRedisUri;
				(redisOptions.ConfigurationOptions ??= new()).ApplyUri(endpoint);
				redisOptions.GrainStorageSerializer = serializer;
			});
		return builder;
	}

	private static IGrainStorageSerializer CreateNativeSerializer(IServiceProvider services, bool base64 = false) =>
		base64
			? new BinaryAsBase64Serializer(CreateNativeSerializer(services))
			: new OrleansGrainStorageSerializer(services.GetRequiredService<Serializer>());

	private static JsonGrainStorageSerializer CreateJsonSerializer(IServiceProvider services) =>
		new(services.GetRequiredService<OrleansJsonSerializer>());
}

