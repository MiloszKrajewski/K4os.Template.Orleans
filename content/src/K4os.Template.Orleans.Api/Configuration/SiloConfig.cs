namespace K4os.Template.Orleans.Api.Configuration;

public class SiloConfig
{
	public static readonly Uri DefaultRedisUri = new("redis://localhost:6379");
	
	public class ClusterConfig
	{
		public string? ClusterId { get; set; }
		public string? ServiceId { get; set; }
		public Uri? RedisEndpoint { get; set; }
	}

	public ClusterConfig? Cluster { get; set; }
}
