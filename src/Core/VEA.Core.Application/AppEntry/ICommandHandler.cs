using VEA.Core.Tools.OperationResult;

namespace VEA.Core.Application.AppEntry;

public interface ICommandHandler<TCommand, TResult>
{
    Task<Result<TResult>> HandleAsync(TCommand command);
}