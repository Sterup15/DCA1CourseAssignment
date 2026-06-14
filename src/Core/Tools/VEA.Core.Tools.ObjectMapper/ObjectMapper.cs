using Microsoft.Extensions.DependencyInjection;

namespace VEA.Core.Tools.ObjectMapper;

public class ObjectMapper(IServiceProvider serviceProvider) : IObjectMapper
{
    public TOut Map<TIn, TOut>(TIn input)
    {
        var mapping = serviceProvider.GetService<IMapping<TIn, TOut>>();
        if (mapping is null)
            throw new InvalidOperationException(
                $"No mapping registered for {typeof(TIn).Name} → {typeof(TOut).Name}.");
        return mapping.Map(input);
    }
}