using Eme_Search.Common;
using Eme_Search.Database;
using Eme_Search.Exceptions;
using Eme_Search.Infrastructures.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Eme_Search.Infrastructures.Repositories.Impl;

public class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : BaseEntity, new() 
{
    private readonly SSDatabaseContext Context;
    private readonly DbSet<TEntity> DbSet;

    public BaseRepository(SSDatabaseContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public async Task<List<TEntity>> GetAllAsync(ISpecification<TEntity>? spec = null)
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<TEntity> GetFirstOrThrowAsync(ISpecification<TEntity>? spec = null)
    {
        var entity = await ApplySpecification(spec).FirstOrDefaultAsync();

        if (entity == null) throw new ResourceNotFoundException(typeof(TEntity));

        return entity;
    }

    public async Task<TEntity?> GetFirstAsync(ISpecification<TEntity>? spec = null)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task<int> CountAsync(ISpecification<TEntity>? spec = null)
    {
        return await ApplySpecification(spec).CountAsync();
    }

    public async Task<bool> AnyAsync(ISpecification<TEntity>? spec = null)
    {
        return await ApplySpecification(spec).AnyAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var addedEntity = (await DbSet.AddAsync(entity)).Entity;

        return addedEntity;
    }

    public Task<TEntity> UpdateAsync(TEntity partialEntity, TEntity entity)
    {
        TEntity entityToUpdate = new();
        foreach (var prop in typeof(TEntity).GetProperties())
        {
            var updatingValue = partialEntity.GetType().GetProperty(prop.Name)?.GetValue(partialEntity);
            var originalValue = prop.GetValue(entity);
            prop.SetValue(entityToUpdate, updatingValue ?? originalValue);
        }
        DbSet.Update(entityToUpdate);

        return Task.FromResult(entityToUpdate);
    }

    public Task<TEntity> DeleteAsync(TEntity entity)
    {
        var removedEntity = DbSet.Remove(entity).Entity;

        return Task.FromResult(removedEntity);
    }

    public Task Detach(TEntity entity)
    {
        Context.Entry(entity).State = EntityState.Detached;
        return Task.CompletedTask;
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity>? spec)
    {
        return SpecificationEvaluator<TEntity>.GetQuery(Context.Set<TEntity>().AsQueryable(), spec);
    }
}