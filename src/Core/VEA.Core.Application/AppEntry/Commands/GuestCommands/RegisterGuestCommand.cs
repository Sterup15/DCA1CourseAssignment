using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Commands.GuestCommands;

public record RegisterGuestCommand
{
    public Name FirstName { get; }
    public Name LastName { get; }
    public ViaMail Email { get; }

    private RegisterGuestCommand(Name firstName, Name lastName, ViaMail email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public static Result<RegisterGuestCommand> Create(string firstName, string lastName, string email)
        => ResultExtensions.CombineInto(
            Name.Create(firstName),
            Name.Create(lastName),
            ViaMail.Create(email),
            (fn, ln, e) => new RegisterGuestCommand(fn, ln, e));
}
