namespace VEA.Core.Tools.ObjectMapper;

public interface IMapping<TIn, TOut>
{
    TOut Map(TIn input);
}