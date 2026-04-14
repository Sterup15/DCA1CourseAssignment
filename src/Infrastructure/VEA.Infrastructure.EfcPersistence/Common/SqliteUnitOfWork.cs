using Microsoft.EntityFrameworkCore;
using VEA.Core.Domain.Common;

namespace VEA.Infrastructure.EfcPersistence.Common;

public class SqliteUnitOfWork(DbContext context) : IUnitOfWork
{
    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}