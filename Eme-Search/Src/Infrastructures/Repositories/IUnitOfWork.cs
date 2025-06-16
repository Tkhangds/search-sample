using Eme_Search.Common;

namespace Eme_Search.Infrastructures.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();

    Task RollBackChangesAsync();

    IBaseRepository<T> Repository<T>() where T : BaseEntity, new() ;
}