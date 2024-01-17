using StackExchange.Redis;

namespace K4os.Template.Orleans.Hosting;

public static class RedisExtensions
{
	private static void AssertRedisUri(bool condition, Uri uri)
	{
		if (!condition)
			throw new ArgumentException(
				$"Invalid Redis URI: {uri}, expected redis://[user:pass@]host[:port]",
				nameof(uri));
	}

	public static ConfigurationOptions ApplyUri(this ConfigurationOptions options, Uri uri)
	{
		AssertRedisUri(uri.Scheme is "redis" or "tcp", uri);
		AssertRedisUri(uri.PathAndQuery is "" or "/", uri);

		var (user, pass) = uri.UserInfo.Split(':', 2) switch {
			[""] => (null, null),
			[var u, var p] => (u, p),
			[var u] => (u, null),
			_ => (null, null),
		};
		if (user is not null) options.User = user;
		if (pass is not null) options.Password = pass;

		var host = uri.Host;
		var port = uri.Port switch { <= 0 => 6379, var p => p };
		options.EndPoints.Add(host, port);

		return options;
	}
}
