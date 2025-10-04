using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace K4os.Template.Orleans.Core;

public static class Extensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<T> AsReadOnly<T>(this Span<T> span) => span;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T NotLessThan<T>(this T value, T min) where T : IComparable<T> =>
		value.CompareTo(min) < 0 ? min : value;
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T NotMoreThan<T>(this T value, T max) where T : IComparable<T> =>
		value.CompareTo(max) > 0 ? max : value;
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsBlank([NotNullWhen(false)] this string? value) =>
		string.IsNullOrWhiteSpace(value);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string EmptyIfNull(this string? value) =>
		value ?? string.Empty;
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull("text")]
	public static byte[]? ToUtf8(this string? text) =>
		text is null ? null : Encoding.UTF8.GetBytes(text);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull("blob")]
	public static string? FromUtf8(this byte[]? blob) =>
		blob is null ? null : Encoding.UTF8.GetString(blob);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Task WhenAll(this IEnumerable<Task> tasks) => 
		Task.WhenAll(tasks);
}
