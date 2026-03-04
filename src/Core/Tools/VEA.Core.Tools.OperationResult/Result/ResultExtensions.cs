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
}
