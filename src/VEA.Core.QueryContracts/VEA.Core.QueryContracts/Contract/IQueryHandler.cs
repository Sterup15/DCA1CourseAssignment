namespace VEA.Core.QueryContracts.Contract;

public interface IQueryHandler<TQuery, TAnswer>
{
    public Task<TAnswer> HandleAsync(TQuery query);
}