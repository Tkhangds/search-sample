using Eme_Search.Common;
using Eme_Search.Infrastructures.Specifications;

namespace Eme_Search.Infrastructures.Repositories;

public interface IBaseRepository<TEntity>
    where TEntity : BaseEntity, new()
{
    Task<TEntity> GetFirstOrThrowAsync(ISpecification<TEntity>? spec = null);

    Task<TEntity?> GetFirstAsync(ISpecification<TEntity>? spec = null);

    Task<List<TEntity>> GetAllAsync(ISpecification<TEntity>? spec = null);

    Task<int> CountAsync(ISpecification<TEntity>? spec = null);

    Task<bool> AnyAsync(ISpecification<TEntity>? spec = null);

    Task<TEntity> AddAsync(TEntity entity);

    Task<TEntity> UpdateAsync(TEntity partialEntity, TEntity entity);

    Task<TEntity> DeleteAsync(TEntity entity);

    Task Detach(TEntity entity);

}