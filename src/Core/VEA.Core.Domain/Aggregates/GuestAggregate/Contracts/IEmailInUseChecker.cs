namespace VEA.Core.Domain.Aggregates.GuestAggregate.Contracts;

public interface IEmailInUseChecker
{
    bool IsEmailInUse(string email);
}