using System.Net;
using System.Net.Sockets;

namespace K4os.Template.Orleans.Hosting;

public static class IpAddressResolver
{
	private static readonly IPAddress Ip1111 = IPAddress.Parse("1.1.1.1");
	private static readonly IPAddress Ip8888 = IPAddress.Parse("8.8.8.8");
	
	private static IPAddress? _localAddress;

	private static IPAddress? GetMyAddress(IPAddress knownHost)
	{
		using var socket = new Socket(
			AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		socket.Connect(knownHost, 0);
		return (socket.LocalEndPoint as IPEndPoint)?.Address;
	}

	private static IPAddress GetMyAddress() =>
		_localAddress ??=
			GetMyAddress(Ip1111) ??
			GetMyAddress(Ip8888) ??
			GetAnyAddress(Dns.GetHostName()) ??
			GetAnyAddress("localhost") ??
			IPAddress.Loopback;

	private static IPAddress? GetAnyAddress(string address) =>
		Dns
			.GetHostAddresses(address)
			.Where(a => a.AddressFamily == AddressFamily.InterNetwork)
			.FirstOrDefault(a => !IPAddress.IsLoopback(a));

	public static IPAddress Advertise(string? address) =>
		address switch { null => GetMyAddress(), _ => GetAnyAddress(address), } ??
		IPAddress.Loopback;

	public static IPAddress Listen(string? address) =>
		address switch { null => null, _ => GetAnyAddress(address), } ??
		IPAddress.Any;
}
