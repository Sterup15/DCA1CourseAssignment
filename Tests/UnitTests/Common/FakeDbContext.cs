using Microsoft.EntityFrameworkCore;
using VEA.Infrastructure.EfcPersistence.Common;

namespace UnitTests.Common;

internal class FakeDbContext() : EfcWriteDbContext(
    new DbContextOptionsBuilder<EfcWriteDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options);
