using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using VEA.Infrastructure.EfcPersistence.Common;
using VEA.Infrastructure.EfcPersistence.GuestAggregate;
using VEA.Infrastructure.EfcPersistence.VeaEventAggregate;

namespace IntegrationTests.Common;

public abstract class IntegrationTestBase : IDisposable
{
    protected readonly EfcWriteDbContext DbContext;
    protected readonly VeaEventRepository EventRepository;
    protected readonly GuestRepository GuestRepository;
    protected readonly SqliteUnitOfWork UnitOfWork;

    private readonly SqliteConnection _connection;

    protected IntegrationTestBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<EfcWriteDbContext>()
            .UseSqlite(_connection)
            .Options;

        DbContext = new EfcWriteDbContext(options);
        DbContext.Database.EnsureCreated();

        EventRepository = new VeaEventRepository(DbContext);
        GuestRepository = new GuestRepository(DbContext);
        UnitOfWork = new SqliteUnitOfWork(DbContext);
    }

    protected EfcWriteDbContext CreateFreshContext() =>
        new(new DbContextOptionsBuilder<EfcWriteDbContext>()
            .UseSqlite(_connection)
            .Options);

    public void Dispose()
    {
        DbContext.Dispose();
        _connection.Dispose();
    }
}
