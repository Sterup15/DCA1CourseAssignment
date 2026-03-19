namespace VEA.Core.Domain.Common;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}