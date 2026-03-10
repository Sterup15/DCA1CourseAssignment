namespace VEA.Core.Tools.OperationResult;

using VEA.Core.Tools.OperationResult.Errors;
public abstract record Result<T>
{
    private Result() { }

    public sealed record Success(T Value) : Result<T>;
    public sealed record Failure(IReadOnlyList<Error> Errors) : Result<T>;
    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;
    public static Result<T> Fail(params Error[] errors) => new Failure(errors);
    public static Result<T> Ok(T value) => new Success(value);
    public static implicit operator Result<T>(T value) => new Success(value);
    public static implicit operator Result<T>(Error error) => new Failure(new[] { error });
    public static implicit operator Result<T>(Error[] errors) => new Failure(errors);
}




