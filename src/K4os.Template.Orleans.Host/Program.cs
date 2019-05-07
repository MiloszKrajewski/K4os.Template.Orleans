using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

// ReSharper disable UnusedParameter.Local

namespace K4os.Template.Orleans.Host
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var loggerFactory = new LoggerFactory();
			loggerFactory.AddProvider(new ColorConsoleProvider());
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddSingleton<ILoggerFactory>(loggerFactory);

			Configure(serviceCollection);
			var serviceProvider = serviceCollection.BuildServiceProvider();
			Execute(loggerFactory, serviceProvider, args).Wait();
		}

		private static void Configure(ServiceCollection serviceCollection) { }

		private static async Task Execute(
			ILoggerFactory loggerFactory, IServiceProvider serviceProvider, string[] args)
		{
			var silo = await StartSilo();
			await silo.Stopped;
		}

		private static async Task<ISiloHost> StartSilo()
		{
			var builder = new SiloHostBuilder()
				.UseLocalhostClustering()
				.UseDashboard(
					o => {
						o.CounterUpdateIntervalMs = 5 * 1000;
						o.Port = 30001;
					})
				.Configure<ClusterOptions>(
					o => {
						o.ClusterId = "dev";
						o.ServiceId = "K4os.Template.Orleans";
					})
				.Configure<EndpointOptions>(
					o => {
						o.AdvertisedIPAddress = IPAddress.Loopback;
					})
				// .ConfigureApplicationParts(parts => parts.AddApplicationPart(...).WithReferences())
				.AddMemoryGrainStorage("store")
				.ConfigureServices((cx, cl) => { })
				.ConfigureLogging(logging => logging.AddProvider(new ColorConsoleProvider()));

			var host = builder.Build();
			await host.StartAsync();
			return host;
		}
	}
}
