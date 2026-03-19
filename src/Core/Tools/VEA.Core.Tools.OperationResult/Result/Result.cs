using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Tools.OperationResult.Result;

public abstract record Result;

public abstract record Failure : Result;

public abstract record Result<T> : Result 
{
    public static implicit operator Result<T>(ResultError error) => new Failure<T>([error]);
    public static implicit operator Result<T>(T value) => new Success<T>(value);
}
public record Success<T>(T Value) : Result<T>;
public record Failure<T>(IEnumerable<ResultError> Errors) : Result<T>;

public record None;




