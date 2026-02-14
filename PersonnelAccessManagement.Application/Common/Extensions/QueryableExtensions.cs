using System.Linq.Expressions;

namespace PersonnelAccessManagement.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> FilterIf<TEntity>(this IQueryable<TEntity> query, Expression<Func<TEntity, bool>> predicate, bool runCondition) where TEntity : class
    {
        return runCondition ? query.Where(predicate) : query;
    }

    public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> query, Expression<Func<TEntity, bool>> predicate, object value) where TEntity : class
    {
        if ((value as bool?).HasValue)
            return query.Where(predicate);

        return value switch
        {
            string when !string.IsNullOrWhiteSpace(value.ToString()) => query.Where(predicate),
            int when Convert.ToInt32(value) > 0 => query.Where(predicate),
            decimal when Convert.ToDecimal(value) > 0 => query.Where(predicate),
            double when Convert.ToDouble(value) > 0 => query.Where(predicate),
            float when Convert.ToDouble(value) > 0 => query.Where(predicate),
            short when Convert.ToSByte(value) > 0 => query.Where(predicate),
            int[] list when list.Length != 0 => query.Where(predicate),
            Enum when Convert.ToInt32(value) > 0 => query.Where(predicate),
            DateTime when Convert.ToDateTime(value) > DateTime.MinValue => query.Where(predicate),
            List<int> list when list.Count != 0 => query.Where(predicate),
            List<string> obj when obj.Count != 0 => query.Where(predicate),
            string[] arr when arr.Length != 0 => query.Where(predicate),
            _ => query
        };
    }
}