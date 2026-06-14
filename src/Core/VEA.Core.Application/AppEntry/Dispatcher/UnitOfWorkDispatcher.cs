using VEA.Core.Domain.Common;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Dispatcher;

public class UnitOfWorkDispatcher(ICommandDispatcher inner, IUnitOfWork unitOfWork) : ICommandDispatcher
{
    public async Task<Result<TResult>> DispatchAsync<TCommand, TResult>(TCommand command)
    {
        var result = await inner.DispatchAsync<TCommand, TResult>(command);

        if (result is Success<TResult>)
            await unitOfWork.SaveChangesAsync();

        return result;
    }
}
