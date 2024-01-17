using System.Net;
using Orleans.Configuration;
using Orleans.Hosting;

namespace K4os.Template.Orleans.Silo.Configuration.Extensions;

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

	public static EndpointOptions Apply(
		this EndpointOptions endpointOptions, SiloConfig? config)
	{
		// advertise
		var advertise = config?.Advertise;
		endpointOptions.AdvertisedIPAddress = IpAddressResolver.Advertise(advertise?.Address);
		endpointOptions.SiloPort = advertise?.SiloPort ?? SiloConfig.DefaultSiloPort;
		endpointOptions.GatewayPort = advertise?.GatewayPort ?? SiloConfig.DefaultGatewayPort;

		// listen
		var listen = config?.Listen;
		var @interface = IpAddressResolver.Listen(listen?.Interface);
		var siloPort = listen?.SiloPort ?? SiloConfig.DefaultSiloPort;
		var gatewayPort = listen?.GatewayPort ?? SiloConfig.DefaultGatewayPort;

		endpointOptions.SiloListeningEndpoint = new IPEndPoint(@interface, siloPort);
		endpointOptions.GatewayListeningEndpoint = new IPEndPoint(@interface, gatewayPort);

		return endpointOptions;
	}
	
	public static ReminderOptions Apply(
		this ReminderOptions reminderOptions, SiloConfig? config)
	{
		reminderOptions.MinimumReminderPeriod = TimeSpan.Zero;
		return reminderOptions;
	}
}
