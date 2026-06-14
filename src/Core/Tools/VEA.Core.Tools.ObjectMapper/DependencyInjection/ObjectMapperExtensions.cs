using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace VEA.Core.Tools.ObjectMapper.DependencyInjection;

public static class ObjectMapperExtensions
{
    public static IServiceCollection AddObjectMapper(this IServiceCollection services)
    {
        services.AddScoped<IObjectMapper, ObjectMapper>();
        return services;
    }

    public static IServiceCollection AddMappings(this IServiceCollection services, params Assembly[] assemblies)
    {
        var openMappingInterface = typeof(IMapping<,>);

        foreach (var assembly in assemblies)
        {
            var mappingTypes = assembly.GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false }
                         && t.GetInterfaces().Any(i => i.IsGenericType
                             && i.GetGenericTypeDefinition() == openMappingInterface));

            foreach (var mappingType in mappingTypes)
            {
                var serviceType = mappingType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == openMappingInterface);

                services.AddScoped(serviceType, mappingType);
            }
        }

        return services;
    }
}