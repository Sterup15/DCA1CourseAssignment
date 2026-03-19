using VEA.Core.Domain.Aggregates.GuestAggregate;

namespace UnitTests.Common;

internal class FakeGuestRepository : IGuestRepository
{
    public Guest? Added { get; private set; }

    public Task AddAsync(Guest guest)
    {
        Added = guest;
        return Task.CompletedTask;
    }

    public Task<Guest?> GetByIdAsync(GuestId id) => Task.FromResult<Guest?>(null);
}
