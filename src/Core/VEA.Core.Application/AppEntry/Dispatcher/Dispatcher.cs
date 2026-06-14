using Microsoft.Extensions.DependencyInjection;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Dispatcher;

public class Dispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public Task<Result<TResult>> DispatchAsync<TCommand, TResult>(TCommand command)
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        return handler.HandleAsync(command);
    }
}
