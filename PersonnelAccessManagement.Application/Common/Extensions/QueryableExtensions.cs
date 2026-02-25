using System.Linq.Expressions;

namespace PersonnelAccessManagement.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> FilterIf<TEntity>(this IQueryable<TEntity> query, Expression<Func<TEntity, bool>> predicate, bool runCondition) where TEntity : class
    {
        return runCondition ? query.Where(predicate) : query;
    }
}