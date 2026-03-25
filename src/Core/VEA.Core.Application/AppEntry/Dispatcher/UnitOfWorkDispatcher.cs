using VEA.Core.Domain.Common;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Dispatcher;

public class UnitOfWorkDispatcher(ICommandDispatcher inner, IUnitOfWork unitOfWork) : ICommandDispatcher
{
    public async Task<Result<Result>> DispatchAsync<TCommand>(TCommand command)
    {
        var result = await inner.DispatchAsync(command);

        if (result is Success<Result>)
            await unitOfWork.SaveChangesAsync();

        return result;
    }
}
