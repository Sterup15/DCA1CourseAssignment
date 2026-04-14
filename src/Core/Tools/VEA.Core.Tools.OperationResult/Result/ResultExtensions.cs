using System.Diagnostics;
using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Tools.OperationResult.Result;

public static class ResultExtensions
{
    // Map: transform success value without possibility of failure
    public static Result<TOut> Map<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> mapper)
        => result switch
        {
            Success<TIn> s => new Success<TOut>(mapper(s.Value)),
            Failure<TIn> f => new Failure<TOut>(f.Errors),
            _ => throw new UnreachableException(),
        };

    // Bind: chain a function that returns Result<TOut>
    public static Result<TOut> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> binder)
        => result switch
        {
            Success<TIn> s => binder(s.Value),
            Failure<TIn> f => new Failure<TOut>(f.Errors),
            _ => throw new UnreachableException(),
        };

    // Ensure: fail if predicate false (only evaluated on success)
    public static Result<T> Ensure<T>(
        this Result<T> result,
        Func<T, bool> predicate,
        ResultError resultError)
        => result switch
        {
            Success<T> s when predicate(s.Value) => result,
            Success<T> => new Failure<T>([resultError]),
            Failure<T> f => f,
            _ => throw new UnreachableException(),
        };

    // Match: end the pipeline
    public static TOut Match<T, TOut>(
        this Result<T> result,
        Func<T, TOut> onSuccess,
        Func<IEnumerable<ResultError>, TOut> onFailure)
        => result switch
        {
            Success<T> s => onSuccess(s.Value),
            Failure<T> f => onFailure(f.Errors),
            _ => throw new UnreachableException(),
        };

    // Tap: side-effect on success
    public static Result<T> Tap<T>(
        this Result<T> result,
        Action<T> action)
    {
        if (result is Success<T> s)
            action(s.Value);
        return result;
    }

    // CombineInto: collect errors from all results, or map values into TResult
    public static Result<TResult> CombineInto<T1, T2, TResult>(
        Result<T1> r1,
        Result<T2> r2,
        Func<T1, T2, TResult> factory)
    {
        var errors = new List<ResultError>();

        if (r1 is Failure<T1> f1) errors.AddRange(f1.Errors);
        if (r2 is Failure<T2> f2) errors.AddRange(f2.Errors);

        if (errors.Count > 0)
            return new Failure<TResult>(errors);

        return new Success<TResult>(factory(
            ((Success<T1>)r1).Value,
            ((Success<T2>)r2).Value));
    }

    public static Result<TResult> CombineInto<T1, T2, T3, TResult>(
        Result<T1> r1,
        Result<T2> r2,
        Result<T3> r3,
        Func<T1, T2, T3, TResult> factory)
    {
        var errors = new List<ResultError>();

        if (r1 is Failure<T1> f1) errors.AddRange(f1.Errors);
        if (r2 is Failure<T2> f2) errors.AddRange(f2.Errors);
        if (r3 is Failure<T3> f3) errors.AddRange(f3.Errors);

        if (errors.Count > 0)
            return new Failure<TResult>(errors);

        return new Success<TResult>(factory(
            ((Success<T1>)r1).Value,
            ((Success<T2>)r2).Value,
            ((Success<T3>)r3).Value));
    }

    // GetValue: extract the success value, throws if failure
    public static T GetValue<T>(this Result<T> result)
        => result switch
        {
            Success<T> s => s.Value,
            Failure<T> f => throw new InvalidOperationException(
                string.Join(", ", f.Errors.Select(e => e.Message))),
            _ => throw new UnreachableException(),
        };

    // WithPayloadIfSuccess: replace the success value, propagate errors on failure
    public static Result<TOut> WithPayloadIfSuccess<T, TOut>(
        this Result<T> result,
        TOut payload)
        => result switch
        {
            Success<T> => new Success<TOut>(payload),
            Failure<T> f => new Failure<TOut>(f.Errors),
            _ => throw new UnreachableException(),
        };
}