namespace K4os.Template.Orleans.Hosting.Configuration;

public class SiloConfig
{
	public class ClusterConfig: ClientConfig.ClusterConfig
	{
		public TimeSpan? KeepAliveInterval { get; set; }
		public TimeSpan? KeepAliveTimeout { get; set; }
	}

	public class ListenConfig
	{
		public string? Interface { get; set; }
		public int? SiloPort { get; set; }
		public int? GatewayPort { get; set; }
	}
	
	public class AdvertiseConfig
	{
		public string? Address { get; set; } = null;
		public int? SiloPort { get; set; } = null;
		public int? GatewayPort { get; set; } = null;
	}

	public class PersistenceConfig
	{
		public Uri? RedisEndpoint { get; set; }
		public bool UseBinary { get; set; }
	}
	
	public class RemindersConfig
	{
		public Uri? RedisEndpoint { get; set; }
	}
	
	public class StreamingConfig
	{
		public Uri? RedisEndpoint { get; set; }
	}

	public ListenConfig? Listen { get; set; }
	public AdvertiseConfig? Advertise { get; set; }
	public ClusterConfig? Cluster { get; set; }
	public PersistenceConfig? Persistence { get; set; }
	public RemindersConfig? Reminders { get; set; }
	public StreamingConfig? Streaming { get; set; }
}
