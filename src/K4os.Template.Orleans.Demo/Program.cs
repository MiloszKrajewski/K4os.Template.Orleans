using System;
using System.Threading.Tasks;

namespace K4os.Template.Orleans.Demo
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			var noArgs = Array.Empty<string>();
			
			var client = Fork(() => Client.Program.Main(noArgs));
			var server = Fork(() => Host.Program.Main(noArgs));

			client.Wait();
			server.Wait(TimeSpan.FromSeconds(3));
			
			Console.WriteLine("Done...");
		}

		private static Task Fork(Action action) => 
			Task.Factory.StartNew(action, TaskCreationOptions.LongRunning);
	}
}
