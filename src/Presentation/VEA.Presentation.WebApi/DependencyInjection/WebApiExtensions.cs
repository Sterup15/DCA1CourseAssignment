using Microsoft.Extensions.DependencyInjection;
using VEA.Core.Tools.ObjectMapper.DependencyInjection;

namespace VEA.Presentation.WebApi.DependencyInjection;

public static class WebApiExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        services.AddObjectMapper();

        // Auto-register all IMapping<,> implementations found in this assembly
        services.AddMappings(typeof(WebApiExtensions).Assembly);

        return services;
    }
}