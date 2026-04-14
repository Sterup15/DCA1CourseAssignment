using VEA.Core.Domain.Common;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Aggregates.GuestAggregate.IRepository;

public interface IGuestRepository : IGenericRepository<Guest, GuestId>
{
    Task<Result<Guest>> FindByEmailAsync(ViaMail email);
    Task<Result<IEnumerable<Guest>>> FindByNameAsync(Name firstName, Name lastName);
}