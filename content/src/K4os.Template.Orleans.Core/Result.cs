using System;
using System.Runtime.CompilerServices;

namespace K4os.Template.Orleans.Core;

public static class ResultExtensions
{
	public static T? OrNull<T>(this in Option<T> option) where T: struct =>
		option.IsNone ? default(T?) : option.OrDefault();

	public static T? OrNull<T>(this Result<T> result) where T: struct =>
		result.IsFailure ? default(T?) : result.OrDefault();

	public static Option<T> Flatten<T>(this Option<Option<T>> option) =>
		option.IsNone ? Option.None<T>() : option.OrDefault();

	public static Result<T> Flatten<T>(this Result<Result<T>> result) =>
		result.IsFailure ? Result.Failure<T>(result.Error) : result.OrDefault();
}

public static class Result
{
	public static Result<T> Success<T>(T value) => new(value);

	public static Result<T> Failure<T>(Exception? error = null) =>
		new(error);

	public static ResultFailure Failure(Exception? error = null) =>
		new(error);

	public static Result<T> Try<T>(Func<T> func)
	{
		try
		{
			return Success(func());
		}
		catch (Exception e)
		{
			return Failure<T>(e);
		}
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Result<T> Try<A, T>(in A arg, Func<A, T> func)
	{
		try
		{
			return Success(func(arg));
		}
		catch (Exception e)
		{
			return Failure<T>(e);
		}
	}


	internal static Exception UnknownError() => new("Unknown error");
}

public readonly struct ResultFailure
{
	public Exception Error { get; }

	public ResultFailure(Exception? error = null) =>
		Error = error ?? Result.UnknownError();
}

public readonly struct Result<T>
{
	private readonly T? _value;
	private readonly Exception? _error;

	public Result(T value)
	{
		_value = value;
		_error = null;
	}

	public Result(Exception? error)
	{
		_value = default;
		_error = error ?? Result.UnknownError();
	}

	public bool IsSuccess => _error is null;
	public bool IsFailure => _error is not null;

	public Exception? Error => _error;

	public static implicit operator Result<T>(T value) => Result.Success(value);

	public static implicit operator Result<T>(ResultFailure error) =>
		Result.Failure<T>(error.Error);

	public static implicit operator Result<T>(Exception error) => Result.Failure<T>(error);

	public Result<U> Map<U>(Func<T, U> map)
	{
		if (_error is not null)
			return Result.Failure<U>(_error);

		try
		{
			return Result.Success(map(_value!));
		}
		catch (Exception ex)
		{
			return Result.Failure<U>(ex);
		}
	}

	public T OrFail(Exception? exception = null) =>
		_error switch { null => _value!, var e => throw exception ?? e.Rethrow(false), };

	public T? OrElse(T? value = default) =>
		_error switch { null => _value, _ => value };

	public T? OrDefault() => _value;

	public Option<T> ToOption() =>
		_error switch { null => Option.Some(_value!), _ => Option.None<T>() };
}

public static class Option
{
	public static Option<T> Some<T>(T value) => new(value);
	public static Option<T> None<T>() => new();

	public static Option<T> From<T>(T? value) =>
		value is not null ? Some(value) : None<T>();

	public static Option<T> From<T>(T? value) where T: struct =>
		value is not null ? Some(value.Value) : None<T>();

	public static OptionNone None() => new();

	internal static Exception NoValue() => new ArgumentException("Option has no value");
}

public struct OptionNone;

public readonly struct Option<T>
{
	private readonly T? _value;
	private readonly bool _some;

	public Option(T value)
	{
		_value = value;
		_some = true;
	}

	public bool IsSome => _some;
	public bool IsNone => !_some;

	public static implicit operator Option<T>(T value) => Option.Some(value);
	public static implicit operator Option<T>(OptionNone _) => Option.None<T>();

	public Option<U> Map<U>(Func<T, U> map) =>
		_some ? Option.Some(map(_value!)) : Option.None<U>();

	public T OrFail(Exception? exception = null) =>
		_some ? _value! : throw exception ?? Option.NoValue();

	public T? OrElse(T? value) => _some ? _value : value;

	public T? OrDefault() => _value;
}
