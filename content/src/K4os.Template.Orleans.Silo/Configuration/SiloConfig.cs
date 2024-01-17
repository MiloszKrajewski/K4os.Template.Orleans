namespace K4os.Template.Orleans.Silo.Configuration;

public class SiloConfig
{
	public static readonly Uri DefaultRedisUri = new("redis://localhost:6379");
	
	public const int DefaultGatewayPort = 13372;
	public const int DefaultSiloPort = 13373;

	public class AdvertiseConfig
	{
		public string? Address { get; set; } = null;
		public int? SiloPort { get; set; } = null;
		public int? GatewayPort { get; set; } = null;
	}

	public class ListenConfig
	{
		public string? Interface { get; set; }
		public int? SiloPort { get; set; }
		public int? GatewayPort { get; set; }
	}

	public class ClusterConfig
	{
		public string? ClusterId { get; set; }
		public string? ServiceId { get; set; }
		public Uri? RedisEndpoint { get; set; }
	}

	public class PersistenceConfig
	{
		public Uri? RedisEndpoint { get; set; }
	}
	
	public class RemindersConfig
	{
		public Uri? RedisEndpoint { get; set; }
	}


	public ListenConfig? Listen { get; set; }
	public AdvertiseConfig? Advertise { get; set; }
	public ClusterConfig? Cluster { get; set; }
	public PersistenceConfig? Persistence { get; set; }
	public RemindersConfig? Reminders { get; set; }
}
