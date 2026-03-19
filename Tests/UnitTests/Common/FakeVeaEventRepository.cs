using VEA.Core.Domain.Aggregates.VeaEventAggregate;

namespace UnitTests.Common;

internal class FakeVeaEventRepository : IVeaEventRepository
{
    private readonly VeaEvent? _event;
    public VeaEvent? Added { get; private set; }

    public FakeVeaEventRepository(VeaEvent? @event = null) => _event = @event;

    public Task AddAsync(VeaEvent veaEvent)
    {
        Added = veaEvent;
        return Task.CompletedTask;
    }

    public Task<VeaEvent?> GetByIdAsync(EventId id) => Task.FromResult(_event);
}
