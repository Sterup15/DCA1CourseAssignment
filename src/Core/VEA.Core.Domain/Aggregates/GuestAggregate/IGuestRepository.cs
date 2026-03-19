namespace VEA.Core.Domain.Aggregates.GuestAggregate;

public interface IGuestRepository
{
    Task AddAsync(Guest guest);
    Task<Guest?> GetByIdAsync(GuestId id);
}