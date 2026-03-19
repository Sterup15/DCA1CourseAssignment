using VEA.Core.Application.AppEntry;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Common;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

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

        if (result is Failure<VeaEvent> failure)
        {
            return new Failure<EventId>(failure.Errors);
        }

        var veaEvent = ((Success<VeaEvent>)result).Value;

        await _repository.AddAsync(veaEvent);
        await _unitOfWork.SaveChangesAsync();

        return new Success<EventId>(veaEvent.Id);
    }
}
