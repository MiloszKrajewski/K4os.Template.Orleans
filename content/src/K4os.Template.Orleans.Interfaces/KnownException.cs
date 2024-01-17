using Orleans;

namespace K4os.Template.Orleans.Interfaces;

[GenerateSerializer]
public class KnownException: Exception
{
	public KnownException(string message): base(message) { }
}