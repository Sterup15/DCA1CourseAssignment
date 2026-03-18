using VEA.Core.Domain.Aggregates.GuestAggregate.Contracts;

namespace UnitTests.Fakes;

public class FakeEmailInUseChecker : IEmailInUseChecker
{
    private readonly bool _isInUse;

    public FakeEmailInUseChecker(bool isInUse = false)
    {
        _isInUse = isInUse;
    }

    public bool IsEmailInUse(string email) => _isInUse;
}