using Orleans.Storage;

namespace K4os.Template.Orleans.Silo.Middleware;

public class BinaryAsBase64Serializer: IGrainStorageSerializer
{
	private readonly IGrainStorageSerializer _serializer;

	public BinaryAsBase64Serializer(IGrainStorageSerializer serializer) => 
		_serializer = serializer;

	public BinaryData Serialize<T>(T input) =>
		Encode(_serializer.Serialize(input));

	public T Deserialize<T>(BinaryData input) =>
		_serializer.Deserialize<T>(Decode(input));
	
	// this has a little bit more memory overhead than I would wish
	// not only it allocates twice (to char[] and then to byte[])
	// but also char uses more space than needed as it is all ASCII anyway
	// using ToBase64Transform and FromBase64Transform is more complex
	// but potentially much more efficient option

	private static BinaryData Encode(BinaryData serialized) =>
		BinaryData.FromString(Convert.ToBase64String(serialized.ToMemory().Span));

	private static BinaryData Decode(BinaryData input) =>
		BinaryData.FromBytes(Convert.FromBase64String(input.ToString()));
}
