using Microsoft.EntityFrameworkCore;
using VEA.Core.Domain.Common;
using VEA.Core.Domain.Common.Bases;
using VEA.Core.Tools.OperationResult.Errors;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Infrastructure.EfcPersistence.Common;

public abstract class RepositoryBase<T, TId>(DbContext context) :
    IGenericRepository<T, TId>
    where T : AggregateRoot<TId>
{
    protected abstract ResultError NotFoundError { get; }

    public virtual async Task<Result<T>> GetAsync(TId id)
    {
        var entity = await context.Set<T>().FindAsync(id);
        return entity is null
            ? new Failure<T>([NotFoundError])
            : new Success<T>(entity);
    }

    public virtual async Task RemoveAsync(TId id)
    {
        var entity = await context.Set<T>().FindAsync(id);
        if (entity is not null)
            context.Set<T>().Remove(entity);
    }

    public virtual async Task AddAsync(T aggregate)
    {
        await context.Set<T>().AddAsync(aggregate);
    }
}
