using VEA.Core.Application.AppEntry;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Common;
using VEA.Core.Tools.OperationResult;

namespace VEA.Core.Application.Features.VeaEventHandlers.UpdateEventTitle;

internal class UpdateEventTitleHandler : ICommandHandler<UpdateEventTitleCommand, None>
{
    private readonly IVeaEventRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    internal UpdateEventTitleHandler(IVeaEventRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<None>> HandleAsync(UpdateEventTitleCommand command)
    {
        var veaEvent = await _repository.GetByIdAsync(command.EventId);

        if (veaEvent is null)
        {
            return Result<None>.Fail(EventErrors.VeaEvent.EventNotFound);
        }

        var updateResult = veaEvent.UpdateTitle(command.Title)
            .WithPayloadIfSuccess(None.Value);

        if (updateResult.IsSuccess)
        {
            await _unitOfWork.SaveChangesAsync();
        }
        
        return updateResult;
    }
}