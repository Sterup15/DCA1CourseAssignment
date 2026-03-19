namespace VEA.Core.Tools.OperationResult;

using VEA.Core.Tools.OperationResult.Errors;
public abstract record Result<T>
{
    private Result() { }

    public sealed record Success(T Value) : Result<T>;
    public sealed record Failure(IReadOnlyList<Error> Errors) : Result<T>;
    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;
    public T GetValue() => this is Success s ? s.Value : throw new InvalidOperationException("Result is not a success."); 
    public IReadOnlyList<Error> GetErrors() => this is Failure f ? f.Errors : throw new InvalidOperationException("Result is not a failure.");       
    public static Result<T> Fail(params Error[] errors) => new Failure(errors);
    public static Result<T> Ok(T value) => new Success(value);
    public static implicit operator Result<T>(T value) => new Success(value);
    public static implicit operator Result<T>(Error error) => new Failure(new[] { error });
    public static implicit operator Result<T>(Error[] errors) => new Failure(errors);
}




