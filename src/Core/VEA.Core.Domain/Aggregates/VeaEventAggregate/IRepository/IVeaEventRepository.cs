using VEA.Core.Domain.Common;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate.IRepository;

public interface IVeaEventRepository : IGenericRepository<VeaEvent, EventId>
{
    
}