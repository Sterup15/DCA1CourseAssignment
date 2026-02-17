namespace VEA.Core.Tools.OperationResult;

using VEA.Core.Tools.OperationResult.Errors;

public sealed class Result<T> : Result
{
    private Result(bool isSuccess, T? value, IReadOnlyList<Error> errors)
        : base(isSuccess, errors)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) =>
        new(true, value, Array.Empty<Error>());

    public static new Result<T> Failure(params Error[] errors) =>
        new(false, default, errors.ToList());

    public static Result<T> Failure(IEnumerable<Error> errors) =>
        new(false, default, errors.ToList());
}
