using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Tools.OperationResult;

public static class ResultExtensions
{
    // Bind: chain a function that returns Result<TOut>
    public static Result<TOut> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> binder)
        => result switch
        {
            Result<TIn>.Success s => binder(s.Value),
            Result<TIn>.Failure f => new Result<TOut>.Failure(f.Errors),
        };

    // Ensure: fail if predicate false (only evaluated on success)
    public static Result<T> Ensure<T>(
        this Result<T> result,
        Func<T, bool> predicate,
        Error error)
        => result switch
        {
            Result<T>.Success s when predicate(s.Value) => result,
            Result<T>.Success => new Result<T>.Failure(new[] { error }),
            Result<T>.Failure f => new Result<T>.Failure(f.Errors),
        };

    // Match: end the pipeline
    public static TOut Match<T, TOut>(
        this Result<T> result,
        Func<T, TOut> onSuccess,
        Func<IReadOnlyList<Error>, TOut> onFailure)
        => result switch
        {
            Result<T>.Success s => onSuccess(s.Value),
            Result<T>.Failure f => onFailure(f.Errors),
        };

    // Tap: side-effect on success
    public static Result<T> Tap<T>(
        this Result<T> result,
        Action<T> action)
        => result switch
        {
            Result<T>.Success s => TapImpl(result, s, action),
            Result<T>.Failure => result,
        };

    private static Result<T> TapImpl<T>(
        Result<T> original,
        Result<T>.Success success,
        Action<T> action)
    {
        action(success.Value);
        return original;
    }

    // CombineInto: collect errors from all results, or map values into TResult
    public static Result<TResult> CombineInto<T1, T2, TResult>(
        Result<T1> r1,
        Result<T2> r2,
        Func<T1, T2, TResult> factory)
    {
        var errors = new List<Error>();

        if (r1 is Result<T1>.Failure f1) errors.AddRange(f1.Errors);
        if (r2 is Result<T2>.Failure f2) errors.AddRange(f2.Errors);

        if (errors.Count > 0)
            return Result<TResult>.Fail(errors.ToArray());

        return Result<TResult>.Ok(factory(
            ((Result<T1>.Success)r1).Value,
            ((Result<T2>.Success)r2).Value));
    }

    public static Result<TResult> CombineInto<T1, T2, T3, TResult>(
        Result<T1> r1,
        Result<T2> r2,
        Result<T3> r3,
        Func<T1, T2, T3, TResult> factory)
    {
        var errors = new List<Error>();

        if (r1 is Result<T1>.Failure f1) errors.AddRange(f1.Errors);
        if (r2 is Result<T2>.Failure f2) errors.AddRange(f2.Errors);
        if (r3 is Result<T3>.Failure f3) errors.AddRange(f3.Errors);

        if (errors.Count > 0)
            return Result<TResult>.Fail(errors.ToArray());

        return Result<TResult>.Ok(factory(
            ((Result<T1>.Success)r1).Value,
            ((Result<T2>.Success)r2).Value,
            ((Result<T3>.Success)r3).Value));
    }

    // WithPayloadIfSuccess: replace the success value, propagate errors on failure
    public static Result<TOut> WithPayloadIfSuccess<T, TOut>(
        this Result<T> result,
        TOut payload)
        => result switch
        {
            Result<T>.Success => Result<TOut>.Ok(payload),
            Result<T>.Failure f => Result<TOut>.Fail(f.Errors.ToArray()),
        };
}
