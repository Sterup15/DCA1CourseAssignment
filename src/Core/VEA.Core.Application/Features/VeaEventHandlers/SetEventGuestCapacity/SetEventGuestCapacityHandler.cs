using VEA.Core.Application.AppEntry;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.IRepository;
using VEA.Core.Domain.Common;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.Features.VeaEventHandlers.SetEventGuestCapacity;

internal class SetEventGuestCapacityHandler : ICommandHandler<SetEventGuestCapacityCommand, None>
{
    private readonly IVeaEventRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    internal SetEventGuestCapacityHandler(IVeaEventRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<None>> HandleAsync(SetEventGuestCapacityCommand command)
    {
        var result = (await _repository.GetAsync(command.EventId))
            .Bind(veaEvent => veaEvent.SetGuestCapacity(command.GuestCapacity))
            .WithPayloadIfSuccess(new None());

        if (result is Success<None>)
            await _unitOfWork.SaveChangesAsync();

        return result;
    }
}
