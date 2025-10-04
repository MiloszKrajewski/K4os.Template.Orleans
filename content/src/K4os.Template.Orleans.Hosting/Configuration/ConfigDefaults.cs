namespace K4os.Template.Orleans.Hosting.Configuration;

public class ConfigDefaults
{
	public const string ClusterName = "K4os.Template.Orleans";

	public static readonly Uri DefaultRedisUri = new("redis://localhost:6379");
	public static readonly Uri DefaultTelemetryUri = new("http://localhost:4317");

	public static readonly TimeSpan KeepAliveInterval = TimeSpan.FromSeconds(10);
	public static readonly TimeSpan KeepAliveTimeout = TimeSpan.FromSeconds(20);
	public static readonly TimeSpan MinimumKeepAliveInterval = TimeSpan.FromSeconds(1);
	
	public const int DefaultGatewayPort = 13372;
	public const int DefaultSiloPort = 13373;
}
