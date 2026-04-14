using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.IRepository;
using VEA.Core.Tools.OperationResult.Errors;
using VEA.Infrastructure.EfcPersistence.Common;

namespace UnitTests.Common;

internal class FakeVeaEventRepository(FakeDbContext db)
    : RepositoryBase<VeaEvent, EventId>(db), IVeaEventRepository
{
    protected override ResultError NotFoundError => EventErrors.VeaEvent.EventNotFound;
}
