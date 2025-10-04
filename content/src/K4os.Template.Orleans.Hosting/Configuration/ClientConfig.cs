namespace K4os.Template.Orleans.Hosting.Configuration;

public class ClientConfig
{
	public class ClusterConfig
	{
		public string? ClusterId { get; set; }
		public string? ServiceId { get; set; }
		public Uri? RedisEndpoint { get; set; }
	}
	
	public ClusterConfig? Cluster { get; set; }
}
