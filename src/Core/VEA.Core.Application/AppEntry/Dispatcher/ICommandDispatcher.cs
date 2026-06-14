using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Dispatcher;

public interface ICommandDispatcher
{
    Task<Result<TResult>> DispatchAsync<TCommand, TResult>(TCommand command);
}
