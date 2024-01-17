using System.Net;
using Orleans.Clustering.Redis;
using Orleans.Configuration;

namespace K4os.Template.Orleans.Api.Configuration.Extensions;

public static class GenericConfigExtensions
{
	public static ClusterOptions Apply(
		this ClusterOptions clusterOptions, SiloConfig? config)
	{
		var clusterId = config?.Cluster?.ClusterId ?? "K4os.Template.Orleans";
		var serviceId = config?.Cluster?.ServiceId ?? "Silo";
		clusterOptions.ClusterId = clusterId;
		clusterOptions.ServiceId = $"{clusterId}/{serviceId}";

		return clusterOptions;
	}
}
