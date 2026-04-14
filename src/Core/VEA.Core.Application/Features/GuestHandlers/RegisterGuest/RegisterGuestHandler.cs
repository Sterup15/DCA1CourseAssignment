using VEA.Core.Application.AppEntry;
using VEA.Core.Application.AppEntry.Commands.GuestCommands;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.GuestAggregate.IRepository;
using VEA.Core.Domain.Common;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.Features.GuestHandlers.RegisterGuest;

internal class RegisterGuestHandler : ICommandHandler<RegisterGuestCommand, GuestId>
{
    private readonly IGuestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    internal RegisterGuestHandler(IGuestRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GuestId>> HandleAsync(RegisterGuestCommand command)
    {
        var result = Guest.Create(command.FirstName, command.LastName, command.Email);

        if (result is Failure<Guest> failure)
        {
            return new Failure<GuestId>(failure.Errors);
        }

        var guest = ((Success<Guest>)result).Value;

        await _repository.AddAsync(guest);
        await _unitOfWork.SaveChangesAsync();

        return new Success<GuestId>(guest.Id);
    }
}
