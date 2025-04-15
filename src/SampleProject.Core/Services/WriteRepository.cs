

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SampleProject.Infrastructure.EF.Models;
using SampleProject.Core.Interfaces;
using System.Reflection;
using SampleProject.Infrastructure.EF.Entities;

namespace SampleProject.Core.Services;

public class WriteRepository<TEntity> : IWriteRepository<TEntity>
    where TEntity : Entity
{
    private readonly VuexyContext _dbContext;
    private readonly IMapper _mapper;

    public WriteRepository(VuexyContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    // Add a new entity
    public async Task<TEntity> AddAsync<TEntityRequest>(TEntityRequest request)
        where TEntityRequest : class
    {
        var entity = _mapper.Map<TEntity>(request);
        if (entity is TrackableEntity trackableEntity)
        {
            trackableEntity.Created("System");
        }
        await _dbContext.Set<TEntity>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    // Update an existing entity
    public async Task<TEntity> UpdateAsync<TEntityRequest>(int id, TEntityRequest request, bool isPartialUpdate = false)
        where TEntityRequest : class
    {
        var entity = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted != true);
        if (entity == null) throw new KeyNotFoundException($"Entity with ID {id} not found.");
        _dbContext.Entry(entity).State = EntityState.Detached;

        if (isPartialUpdate)
            UpdatePartialEntity(entity, request);
        else
        {
            entity = _mapper.Map<TEntity>(request);
            entity.Id = id;
        }

        if (entity is TrackableEntity trackableEntity)
        {
            trackableEntity.Updated("System");
        }

        _dbContext.Set<TEntity>().Update(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public void UpdatePartialEntity<TEntityRequest>(TEntity entity, TEntityRequest request)
        where TEntityRequest : class
    {
        var entityProperties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var requestProperties = typeof(TEntityRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var requestProperty in requestProperties)
        {
            // Check if the property exists on the entity
            var entityProperty = entityProperties.FirstOrDefault(p => p.Name == requestProperty.Name);

            if (entityProperty != null && entityProperty.CanWrite)
            {
                // Get the value from the request and set it on the entity
                var requestValue = requestProperty.GetValue(request);
                if (requestValue != null)
                {
                    entityProperty.SetValue(entity, requestValue);
                }
            }
        }
    }


    // Delete an entity
    public async Task DeleteAsync(TEntity entity)
    {
        if (entity is TrackableEntity trackableEntity)
        {
            trackableEntity.Deleted("System");
        }
        _dbContext.Set<TEntity>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var entity = await _dbContext.Set<TEntity>().FindAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Entity with ID {id} not found");
        }
        if (entity is TrackableEntity trackableEntity)
        {
            trackableEntity.Deleted("System");
        }
        _dbContext.Set<TEntity>().Update(entity);
        await _dbContext.SaveChangesAsync();
    }
}
