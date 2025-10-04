using System;
using System.Net;
using K4os.Template.Orleans.Hosting;
using K4os.Template.Orleans.Hosting.Configuration;
using Orleans.Configuration;

namespace K4os.Template.Orleans.Silo.Hosting;

public static class ClusteringExtensions
{
	private static T NotLessThan<T>(this T value, T min) where T: IComparable<T> =>
		value.CompareTo(min) < 0 ? min : value;

	public static ClusterMembershipOptions Apply(
		this ClusterMembershipOptions clusterOptions, SiloConfig? config)
	{
		var interval = (config?.Cluster?.KeepAliveInterval ?? ConfigDefaults.KeepAliveInterval)
			.NotLessThan(ConfigDefaults.MinimumKeepAliveInterval);
		var timeout = (config?.Cluster?.KeepAliveTimeout ?? ConfigDefaults.KeepAliveTimeout)
			.NotLessThan(interval);
		var missedLimit = (int)Math.Ceiling(timeout / interval - 1.0).NotLessThan(0);
		clusterOptions.IAmAliveTablePublishTimeout = interval;
		clusterOptions.NumMissedTableIAmAliveLimit = missedLimit;
		return clusterOptions;
	}

	public static EndpointOptions Apply(
		this EndpointOptions endpointOptions, SiloConfig? config)
	{
		// advertise
		var advertise = config?.Advertise;
		endpointOptions.AdvertisedIPAddress = IpAddressResolver.Advertise(advertise?.Address);
		endpointOptions.SiloPort = advertise?.SiloPort ?? ConfigDefaults.DefaultSiloPort;
		endpointOptions.GatewayPort = advertise?.GatewayPort ?? ConfigDefaults.DefaultGatewayPort;

		// listen
		var listen = config?.Listen;
		var @interface = IpAddressResolver.Listen(listen?.Interface);
		var siloPort = listen?.SiloPort ?? ConfigDefaults.DefaultSiloPort;
		var gatewayPort = listen?.GatewayPort ?? ConfigDefaults.DefaultGatewayPort;

		endpointOptions.SiloListeningEndpoint = new IPEndPoint(@interface, siloPort);
		endpointOptions.GatewayListeningEndpoint = new IPEndPoint(@interface, gatewayPort);

		return endpointOptions;
	}
}
