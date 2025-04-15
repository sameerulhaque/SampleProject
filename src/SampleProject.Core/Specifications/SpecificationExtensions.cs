public static class SpecificationExtensions
{
    public static IQueryable<TEntity> Specify<TEntity>(this IQueryable<TEntity> query, Specification<TEntity> specification)
    {
        var queryable = query;

        // Apply Include Expressions for navigation properties
        //if (specification.IncludeExpressions.Count != 0)
        //{
        //    queryable = specification.IncludeExpressions.Aggregate(queryable,
        //        (current, includeExpression) => current.Include(includeExpression));
        //}

        //// Apply Include Strings for navigation properties (dynamic includes by string)
        //if (specification.IncludeStrings.Count != 0)
        //{
        //    queryable = specification.IncludeStrings.Aggregate(queryable,
        //        (current, includeString) => current.Include(includeString));
        //}

        // Apply filter criteria (WHERE clause)
        if (specification.Criteria != null)
        {
            queryable = queryable.Where(specification.Criteria);
        }

        // Apply ordering
        if (specification.OrderByExpression != null)
        {
            var orderedQuery = queryable.OrderBy(specification.OrderByExpression);
            if (specification.ThenOrderByExpression != null)
                queryable = orderedQuery.ThenBy(specification.ThenOrderByExpression);
            else if (specification.ThenOrderByDescendingExpression != null)
                queryable = orderedQuery.ThenByDescending(specification.ThenOrderByDescendingExpression);
            else
                queryable = orderedQuery;
        }
        else if (specification.OrderByDescendingExpression != null)
        {
            var orderedQuery = queryable.OrderByDescending(specification.OrderByDescendingExpression);
            if (specification.ThenOrderByExpression != null)
                queryable = orderedQuery.ThenBy(specification.ThenOrderByExpression);
            else if (specification.ThenOrderByDescendingExpression != null)
                queryable = orderedQuery.ThenByDescending(specification.ThenOrderByDescendingExpression);
            else
                queryable = orderedQuery;
        }

        // Apply paging (Skip and Take)
        if (specification.IsPagingEnabled)
        {
            queryable = queryable.Skip(specification.Skip).Take(specification.Take);
        }

        return queryable;
    }
}
