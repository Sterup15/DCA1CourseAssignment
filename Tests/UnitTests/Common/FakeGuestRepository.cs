using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.GuestAggregate.IRepository;
using VEA.Core.Tools.OperationResult.Errors;
using VEA.Core.Tools.OperationResult.Result;
using VEA.Infrastructure.EfcPersistence.Common;

namespace UnitTests.Common;

internal class FakeGuestRepository(FakeDbContext db)
    : RepositoryBase<Guest, GuestId>(db), IGuestRepository
{
    protected override ResultError NotFoundError => GuestErrors.Guest.NotFound;

    public Task<Result<Guest>> FindByEmailAsync(ViaMail email)
    {
        var entity = db.Guests.FirstOrDefault(g => g.ViaMail == email);
        return Task.FromResult<Result<Guest>>(entity is null
            ? new Failure<Guest>([NotFoundError])
            : new Success<Guest>(entity));
    }

    public Task<Result<IEnumerable<Guest>>> FindByNameAsync(Name firstName, Name lastName)
    {
        var results = db.Guests.Where(g => g.FirstName == firstName && g.LastName == lastName);
        return Task.FromResult<Result<IEnumerable<Guest>>>(new Success<IEnumerable<Guest>>(results));
    }
}
