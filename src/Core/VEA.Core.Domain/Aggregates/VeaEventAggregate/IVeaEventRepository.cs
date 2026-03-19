namespace VEA.Core.Domain.Aggregates.VeaEventAggregate;

public interface IVeaEventRepository
{
    Task AddAsync(VeaEvent veaEvent);
    Task<VeaEvent?> GetByIdAsync(EventId id);
}