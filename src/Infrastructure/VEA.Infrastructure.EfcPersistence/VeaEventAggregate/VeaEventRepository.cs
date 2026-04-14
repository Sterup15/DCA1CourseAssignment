using Microsoft.EntityFrameworkCore;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.IRepository;
using VEA.Core.Tools.OperationResult.Errors;
using VEA.Core.Tools.OperationResult.Result;
using VEA.Infrastructure.EfcPersistence.Common;

namespace VEA.Infrastructure.EfcPersistence.VeaEventAggregate;

public class VeaEventRepository(EfcWriteDbContext context) : RepositoryBase<VeaEvent, EventId>(context), IVeaEventRepository
{
    protected override ResultError NotFoundError => EventErrors.VeaEvent.EventNotFound;

    public override async Task<Result<VeaEvent>> GetAsync(EventId id)
    {
        var entity = await context.Events
            .Include("_participations")
            .FirstOrDefaultAsync(e => e.Id == id);

        return entity is null
            ? new Failure<VeaEvent>([NotFoundError])
            : new Success<VeaEvent>(entity);
    }
}