namespace VEA.Core.Tools.OperationResult;

public static class ResultExtensions
{
    public static Result<TOut> Map<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> mapper)
        => result.IsSuccess
            ? Result<TOut>.Success(mapper(result.Value!))
            : Result<TOut>.Failure(result.Errors);

    public static Result<TOut> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> binder)
        => result.IsSuccess
            ? binder(result.Value!)
            : Result<TOut>.Failure(result.Errors);
    
    public static Result<T> Tap<T>(
        this Result<T> result,
        Action<T> action)
    {
        if (result.IsSuccess)
            action(result.Value!);

        return result;
    }

}
