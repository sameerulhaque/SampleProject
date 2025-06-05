using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            var propertyType = property.Type;

            object typedValue;
            try
            {
                typedValue = Convert.ChangeType(searchValue, Nullable.GetUnderlyingType(propertyType) ?? propertyType);
            }
            catch
            {
                continue; // Skip this condition if conversion fails
            }

            var constant = Expression.Constant(typedValue, propertyType);
            Expression predicate;

            if (propertyType == typeof(string))
            {
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                predicate = Expression.Call(property, containsMethod!, constant);
            }
            else
            {
                predicate = Expression.Equal(property, constant);
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(predicate, parameter);

            if (Criteria == null)
            {
                Criteria = lambda;
            }
            else
            {
                // Replace parameter in existing criteria
                var parameterReplacer = new ParameterReplacer(lambda.Parameters[0], Criteria.Parameters[0]);
                var updatedBody = parameterReplacer.Visit(lambda.Body);
                var combined = Expression.AndAlso(Criteria.Body, updatedBody);
                Criteria = Expression.Lambda<Func<TEntity, bool>>(combined, Criteria.Parameters[0]);
            }
        }
    }


    public void AddPaging(int pageSize, int pageNumber)
    {
        if (pageSize > 0)
        {
            Skip = (pageNumber - 1) * pageSize;
            Take = pageSize;
            IsPagingEnabled = true;
        }
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
    public void AddIncludes(List<string> includes)
    {
        var seenPaths = new HashSet<string>();

        foreach (var include in includes)
        {
            if (seenPaths.Contains(include))
                continue;

            if (include.Split('.').Distinct().Count() != include.Split('.').Length)
                continue; // skip obviously recursive chains like User.User.User

            var parameter = Expression.Parameter(typeof(TEntity), "x");
            Expression body = parameter;

            foreach (var member in include.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);
            }

            var lambda = CreateLambda(body, parameter);
            AddInclude(lambda);

            seenPaths.Add(include);
        }
    }


    private static Expression<Func<TEntity, object>> CreateLambda(Expression body, ParameterExpression parameter)
{
    // If the body is a value type, box it to object
    if (body.Type.IsValueType)
    {
        body = Expression.Convert(body, typeof(object));
    }

    return Expression.Lambda<Func<TEntity, object>>(body, parameter);
}


    public void AddInclude(Expression<Func<TEntity, object>> includeExpression)
    {
        IncludeExpressions.Add(includeExpression);
    }

    //public void AddInclude(string includeString)
    //{
    //    IncludeStrings.Add(includeString);
    //}
}

public class ParameterReplacer : ExpressionVisitor
{
    private readonly ParameterExpression _oldParameter;
    private readonly ParameterExpression _newParameter;

    public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        _oldParameter = oldParameter;
        _newParameter = newParameter;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == _oldParameter ? _newParameter : base.VisitParameter(node);
    }
}
public enum OrderType
{
    Ascending,
    Descending
}
