using VEA.Core.Domain.Common.Bases;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Common;

public interface IGenericRepository<T, TId>
    where T : AggregateRoot<TId>
{
    Task<Result<T>> GetAsync(TId id);
    Task RemoveAsync(TId id);
    Task AddAsync(T aggregate);
}