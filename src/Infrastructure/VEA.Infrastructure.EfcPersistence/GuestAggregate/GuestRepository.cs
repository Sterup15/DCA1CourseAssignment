using Microsoft.EntityFrameworkCore;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.GuestAggregate.IRepository;
using VEA.Core.Tools.OperationResult.Errors;
using VEA.Core.Tools.OperationResult.Result;
using VEA.Infrastructure.EfcPersistence.Common;

namespace VEA.Infrastructure.EfcPersistence.GuestAggregate;

public class GuestRepository (EfcWriteDbContext context) : RepositoryBase<Guest, GuestId>(context), IGuestRepository
{
    protected override ResultError NotFoundError => GuestErrors.Guest.NotFound;

    public async Task<Result<Guest>> FindByEmailAsync(ViaMail email)
    {
        var guest = await context.Guests.FirstOrDefaultAsync(g => g.ViaMail == email);
        return guest is null
            ? new Failure<Guest>([NotFoundError])
            : new Success<Guest>(guest);
    }

    public async Task<Result<IEnumerable<Guest>>> FindByNameAsync(Name firstName, Name lastName)
    {
        var guests = await context.Guests
            .Where(g => g.FirstName == firstName && g.LastName == lastName)
            .ToListAsync();
        return new Success<IEnumerable<Guest>>(guests);
    }
}