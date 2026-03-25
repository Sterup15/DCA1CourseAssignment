using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Dispatcher;

public interface ICommandDispatcher
{
    public Task<Result<Result>> DispatchAsync<TCommand>(TCommand command);
}