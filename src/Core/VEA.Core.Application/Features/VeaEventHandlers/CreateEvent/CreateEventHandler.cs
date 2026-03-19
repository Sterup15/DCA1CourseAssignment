using VEA.Core.Application.AppEntry;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Common;
using VEA.Core.Tools.OperationResult;

namespace VEA.Core.Application.Features.VeaEventHandlers.CreateEvent;

internal class CreateEventHandler : ICommandHandler<CreateEventCommand, EventId>
{
    private readonly IVeaEventRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    internal CreateEventHandler(IVeaEventRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EventId>> HandleAsync(CreateEventCommand command)
    {
        var result = VeaEvent.Create();

        if (result.IsFailure)
        {
            return Result<EventId>.Fail(result.GetErrors().ToArray());
        }

        var veaEvent = result.GetValue();

        await _repository.AddAsync(veaEvent);
        await _unitOfWork.SaveChangesAsync();

        return Result<EventId>.Ok(veaEvent.Id);
    }
}
