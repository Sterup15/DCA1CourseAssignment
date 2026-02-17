namespace VEA.Core.Tools.OperationResult.Errors;

public sealed record Error(
    string Code,
    string Message,
    ErrorType Type,
    string? Target = null
);
