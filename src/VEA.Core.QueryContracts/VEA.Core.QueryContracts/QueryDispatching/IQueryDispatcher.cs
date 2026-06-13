using VEA.Core.QueryContracts.Contract;

namespace VEA.Core.QueryContracts.QueryDispatching;

public interface IQueryDispatcher
{
    Task<TAnswer> DispatchAsync<TAnswer>(IQuery<TAnswer> query);
}