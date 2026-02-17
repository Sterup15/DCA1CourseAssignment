namespace VEA.Core.Tools.OperationResult;

using VEA.Core.Tools.OperationResult.Errors;
public class Result
{
    protected Result(bool isSuccess, IReadOnlyList<Error> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<Error> Errors { get; }

    public static Result Success() => new(true, Array.Empty<Error>());

    public static Result Failure(params Error[] errors) =>
        new(false, errors.ToList());

    public static Result Failure(IEnumerable<Error> errors) =>
        new(false, errors.ToList());
}
