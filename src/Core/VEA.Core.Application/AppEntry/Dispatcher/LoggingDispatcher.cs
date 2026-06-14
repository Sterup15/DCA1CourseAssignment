using Microsoft.Extensions.Logging;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Dispatcher;

public class LoggingDispatcher(ICommandDispatcher inner, ILogger<LoggingDispatcher> logger) : ICommandDispatcher
{
    public async Task<Result<TResult>> DispatchAsync<TCommand, TResult>(TCommand command)
    {
        logger.LogInformation("Dispatching {Command}: {@Payload}", typeof(TCommand).Name, command);

        var result = await inner.DispatchAsync<TCommand, TResult>(command);

        if (result is Success<TResult>)
            logger.LogInformation("Dispatch succeeded: {Command}", typeof(TCommand).Name);
        else if (result is Failure<TResult> f)
            logger.LogWarning("Dispatch failed: {Command} — {Errors}", typeof(TCommand).Name, f.Errors);

        return result;
    }
}
