using VEA.Core.Tools.OperationResult.Result;
using Microsoft.Extensions.DependencyInjection;
using VEA.Core.Application.AppEntry.Exceptions;

namespace VEA.Core.Application.AppEntry.Dispatcher;

public class Dispatcher(IServiceProvider serviceProvider) : ICommandDispatcher //Integrated constructor? 
{
    public Task<Result<Result>> DispatchAsync<TCommand>(TCommand command)
    {
        Type serviceType = typeof(ICommandHandler<TCommand, Result>);
        var service = serviceProvider.GetService(serviceType);
        if (service == null)
        {
            throw new ServiceNotFoundException(nameof(ICommandHandler<TCommand, Result>));
        }

        ICommandHandler<TCommand, Result> handler = (ICommandHandler<TCommand, Result>)service; //Nasty parsing syntax, where's the space?
        return handler.HandleAsync(command);
    }
}