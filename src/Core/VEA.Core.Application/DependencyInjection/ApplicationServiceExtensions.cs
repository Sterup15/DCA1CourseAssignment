using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using VEA.Core.Application.AppEntry;
using VEA.Core.Application.AppEntry.Dispatcher;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationHandlers(this IServiceCollection services, Assembly handlersAssembly)
    {
        Type openHandlerInterface = typeof(ICommandHandler<,>);

        IEnumerable<Type> handlerTypes = handlersAssembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } &&
                        t.GetInterfaces().Any(i => i.IsGenericType &&
                                                   i.GetGenericTypeDefinition() == openHandlerInterface));

        foreach (Type handlerType in handlerTypes)
        {
            Type serviceType = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == openHandlerInterface);

            services.AddScoped(serviceType, handlerType);
        }

        return services;
    }

    public static IServiceCollection AddCommandDispatcher(this IServiceCollection services)
    {
        services.AddScoped<Dispatcher>();

        services.AddScoped<ICommandDispatcher>(sp =>
            new LoggingDispatcher(
                new UnitOfWorkDispatcher(
                    sp.GetRequiredService<Dispatcher>(),
                    sp.GetRequiredService<VEA.Core.Domain.Common.IUnitOfWork>()
                ),
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<LoggingDispatcher>>()
            ));

        return services;
    }
}
