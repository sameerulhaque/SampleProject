using System.Linq.Expressions;

public class ConcreteSpecification<TEntity> : Specification<TEntity>
{
}
public abstract class Specification<TEntity>
{
    protected Specification() { }

    public Expression<Func<TEntity, bool>>? Criteria { get; set; }
    public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = new List<Expression<Func<TEntity, object>>>();
    public List<string> IncludeStrings { get; set; } = new List<string>();
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }
    public Expression<Func<TEntity, object>>? OrderByExpression { get; private set; }
    public Expression<Func<TEntity, object>>? OrderByDescendingExpression { get; private set; }
    public Expression<Func<TEntity, object>>? ThenOrderByExpression { get; private set; }
    public Expression<Func<TEntity, object>>? ThenOrderByDescendingExpression { get; private set; }
    public void AddSearchCriteria(Dictionary<string, string> searchConditions)
    {
        foreach (var searchCondition in searchConditions)
        {
            var searchKey = searchCondition.Key;
            var searchValue = searchCondition.Value;

            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, searchKey);

            var value = Expression.Constant(searchValue);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            var containsExpression = Expression.Call(property, containsMethod, value);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(containsExpression, parameter);

            if (Criteria == null)
            {
                Criteria = lambda;
            }
            else
            {
                var combinedExpression = Expression.AndAlso(Criteria.Body, lambda.Body);
                Criteria = Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter);
            }
        }
    }

    public void AddPaging(int pageSize, int pageNumber)
    {
        Skip = (pageNumber - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }

    public void AddOrderBy(Expression<Func<TEntity, object>> orderExpression, OrderType orderType)
    {
        if (orderType == OrderType.Descending)
        {
            OrderByDescendingExpression = orderExpression;
        }
        else
        {
            OrderByExpression = orderExpression;
        }
    }

    public void AddThenOrderBy(Expression<Func<TEntity, object>> orderExpression, OrderType orderType)
    {
        if (orderType == OrderType.Descending)
        {
            ThenOrderByDescendingExpression = orderExpression;
        }
        else
        {
            ThenOrderByExpression = orderExpression;
        }
    }

    public void AddInclude(Expression<Func<TEntity, object>> includeExpression)
    {
        IncludeExpressions.Add(includeExpression);
    }

    public void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }
}


public enum OrderType
{
    Ascending,
    Descending
}
