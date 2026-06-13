using VEA.Core.QueryContracts.Contract;

namespace IntegrationTests.EfcQueries.Common;

public class FakeSystemTime(DateTime fixedTime) : ISystemTime
{
    public DateTime CurrentTime() => fixedTime;
}