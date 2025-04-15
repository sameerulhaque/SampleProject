using SampleProject.Infrastructure.EF.Models;

namespace SampleProject.Core.Interfaces;

public interface IWriteRepository<TEntity>
    where TEntity : Entity
{
    Task<TEntity> AddAsync<TEntityRequest>(TEntityRequest request)
        where TEntityRequest : class;

    Task<TEntity> UpdateAsync<TEntityRequest>(int id, TEntityRequest request, bool isPartialUpdate = false)
        where TEntityRequest : class;

    Task SoftDeleteAsync(int id);
}
