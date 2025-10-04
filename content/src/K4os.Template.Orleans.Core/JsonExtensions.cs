using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace K4os.Template.Orleans.Core;

// buffer -> json	: ParseJson
// buffer -> object	: ParseJson<T>
// buffer -> string : FromUtf8	

// string -> buffer : ToUtf8
// string -> json	: ParseJson
// string -> object	: ParseJson<T>

// json -> buffer	: ToJsonBuffer
// json -> object	: ParseJson<T> (*Deserialize)
// json -> string	: (*ToJsonString)

// object -> json	: ToJsonNode
// object -> string : ToJsonString
// object -> buffer	: ToJsonBuffer

public static class JsonExtensions
{
	public static readonly string JsonNullString = "null";
	public static readonly byte[] JsonNullBytes = JsonNullString.ToUtf8().ToArray();
	
	// buffer -> ...
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] OrJsonNull(this byte[]? buffer, bool shared = false) =>
		buffer is null or { Length: 0 } 
			? shared ? JsonNullBytes : JsonNullBytes.ToArray() 
			: buffer;
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static JsonNode? ParseJson(this ReadOnlySpan<byte> buffer) =>
		buffer is { Length: 0 } ? null : JsonNode.Parse(buffer);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static JsonNode? ParseJson(this byte[]? buffer) =>
		buffer is null ? null : ParseJson(buffer.AsSpan());
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? ParseJson<T>(this ReadOnlySpan<byte> buffer) =>
		buffer is { Length: 0 } ? default : JsonSerializer.Deserialize<T>(buffer);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? ParseJson<T>(this byte[]? buffer) =>
		buffer is null ? default : ParseJson<T>(buffer.AsSpan());
	
	// string -> ...

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string OrJsonNull(this string? buffer) =>
		buffer is null or { Length: 0 } ? JsonNullString : buffer;
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static JsonNode? ParseJson(this ReadOnlySpan<char> buffer) =>
		ParseJson(new string(buffer)); // TODO: optimize (avoid allocation)

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static JsonNode? ParseJson(this string? buffer) =>
		buffer is null ? null : JsonNode.Parse(buffer);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? ParseJson<T>(this ReadOnlySpan<char> buffer) =>
		buffer is { Length: 0 } ? default : JsonSerializer.Deserialize<T>(buffer);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? ParseJson<T>(this string? buffer) =>
		buffer is null ? default : ParseJson<T>(buffer.AsSpan());
	
	// json -> ...
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[]? ToJsonBuffer(this JsonNode? value) =>
		value is null ? null : JsonSerializer.SerializeToUtf8Bytes(value);

	//	public static string? ToJsonString(this JsonNode? value) =>
	//		value is null ? null : value.ToJsonString();
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? ParseJson<T>(this JsonNode? value) =>
		value is null ? default : value.Deserialize<T>();
	
	// object -> ...
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull("value")]
	public static JsonNode? ToJsonNode<T>(this T? value) =>
		value is null ? null : JsonSerializer.SerializeToNode<T>(value);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull("value")]
	public static string? ToJsonString<T>(this T? value) =>
		value is null ? null : JsonSerializer.Serialize<T>(value);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull("value")]
	public static byte[]? ToJsonBuffer<T>(this T? value) =>
		value is null ? null : JsonSerializer.SerializeToUtf8Bytes(value);
}

