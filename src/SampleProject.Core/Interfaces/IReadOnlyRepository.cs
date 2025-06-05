
using SampleProject.Infrastructure.EF.Models;

namespace SampleProject.Core.Interfaces;

public interface IReadOnlyRepository<TEntity>
    where TEntity : Entity
{
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<(int TotalCount, IReadOnlyList<TModel> Data)> ListAsync<TModel>(Specification<TEntity> specification, CancellationToken cancellationToken = default);
}
