using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace K4os.Template.Orleans.Core;

public static class ExceptionExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Rethrow<T>(
		this T exception, [DoesNotReturnIf(true)] bool force = true)
		where T: Exception
	{
		if (force || exception.TargetSite is not null)
			ExceptionDispatchInfo.Capture(exception).Throw();
		return exception;
	}

	private static string? ExceptionName(Exception exception) =>
		exception.GetType().FullName;

	public static string Explain(this Exception? exception, bool stackTrace = true) =>
		exception is null ? "<null>" :
		stackTrace ? ExplainWithStackTrace(exception) :
		$"{ExceptionName(exception)}: {exception.Message}";

	private static string ExplainWithStackTrace(this Exception? exception)
	{
		if (exception is null) return "<null>";

		var result = new StringBuilder();
		ExplainWithStackTrace(exception, 0, result);
		return result.ToString();
	}

	internal static void ExplainWithStackTrace(
		Exception? exception, int level, StringBuilder builder)
	{
		if (exception is null)
			return;

		// $"{exception.GetType().FullName}@{level}: {exception.Message}\n{exception.StackTrace}\n"
		builder
			.Append(ExceptionName(exception)).Append('@').Append(level).Append(": ")
			.AppendLine(exception.Message)
			.AppendLine(exception.StackTrace);

		if (exception is AggregateException aggregate)
		{
			foreach (var inner in aggregate.InnerExceptions)
				ExplainWithStackTrace(inner, level + 1, builder);
		}
		else
		{
			ExplainWithStackTrace(exception.InnerException, level + 1, builder);
		}
	}

	public static IEnumerable<Exception> Flatten(this Exception exception)
	{
		var exceptions = new Queue<Exception>();
		exceptions.Enqueue(exception);

		while (exceptions.Count > 0)
		{
			exception = exceptions.Dequeue();

			yield return exception;

			if (exception is AggregateException aggregate)
			{
				foreach (var inner in aggregate.InnerExceptions)
					exceptions.Enqueue(inner);
			}
			else
			{
				var inner = exception.InnerException;
				if (inner is not null) exceptions.Enqueue(inner);
			}
		}
	}
}
