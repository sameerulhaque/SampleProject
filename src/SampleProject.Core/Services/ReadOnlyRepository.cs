using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using SampleProject.Core.Interfaces;
using SampleProject.Infrastructure.EF.Entities;
using SampleProject.Infrastructure.EF.Models;
using SampleProject.Infrastructure.Polly;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SampleProject.Core.Services;
public class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity>
    where TEntity : Entity
{
    private readonly VuexyContext _dbContext;
    private readonly IMapper _mapper;

    public ReadOnlyRepository(VuexyContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted != true, cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TEntity>().AsNoTracking().Where(x => x.IsDeleted != true).ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(Specification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<TEntity>().AsNoTracking().Where(x => x.IsDeleted != true).AsQueryable();
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(int TotalCount, IReadOnlyList<TResponse> Data)> ListAsync<TResponse>(Specification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<TEntity>().AsNoTracking().Where(x => x.IsDeleted != true).AsQueryable();

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }
        if (specification.IncludeExpressions != null)
        {
            foreach (var expression in specification.IncludeExpressions)
            {
                query = query.Include(expression);
            }
        }

        var fallbackData = new List<TEntity>();

        var totalCount = await EfCorePolices.ExecuteWithPollyAsync(
        async () => await query.CountAsync(cancellationToken),
        fallbackValue: 0,
        cancellationToken);

        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        if (specification.OrderByExpression != null)
        {
            query = query.OrderBy(specification.OrderByExpression);
        }
        else if (specification.OrderByDescendingExpression != null)
        {
            query = query.OrderByDescending(specification.OrderByDescendingExpression);
        }

        var data = await EfCorePolices.ExecuteWithPollyAsync<IReadOnlyList<TEntity>>(
        async () => await query.ToListAsync(cancellationToken),
        fallbackValue: new List<TEntity>(),
        cancellationToken);

        var res = _mapper.Map<List<TResponse>>(data);
        return (totalCount, res);
    }
}
