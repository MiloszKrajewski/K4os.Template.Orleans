using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;

// ReSharper disable UnusedParameter.Local

namespace K4os.Template.Orleans.Client
{
	public static class Program
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
			var client = await StartClient();

			await Task.Delay(TimeSpan.FromSeconds(60));
		}

		private static async Task<IClusterClient> StartClient()
		{
			var client = new ClientBuilder()
				.UseLocalhostClustering()
				.Configure<ClusterOptions>(
					options => {
						options.ClusterId = "dev";
						options.ServiceId = "K4os.Template.Orleans";
					})
				.ConfigureLogging(logging => logging.AddProvider(new ColorConsoleProvider()))
				.Build();

			var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(20));
			await client.Connect(_ => RetryUntilCancelled(TimeSpan.FromSeconds(1), cancel.Token));

			return client;
		}

		private static async Task<bool> RetryUntilCancelled(
			TimeSpan interval, CancellationToken cancel)
		{
			try
			{
				await Task.Delay(interval, cancel);
			}
			catch (TaskCanceledException)
			{
				return false;
			}

			return !cancel.IsCancellationRequested;
		}
	}
}
