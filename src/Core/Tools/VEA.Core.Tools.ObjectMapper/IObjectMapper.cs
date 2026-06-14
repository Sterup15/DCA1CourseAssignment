namespace VEA.Core.Tools.ObjectMapper;

public interface IObjectMapper
{
    TOut Map<TIn, TOut>(TIn input);
}
