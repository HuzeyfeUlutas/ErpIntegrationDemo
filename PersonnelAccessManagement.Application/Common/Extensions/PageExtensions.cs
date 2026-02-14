using PersonnelAccessManagement.Application.Common.Models;
using PersonnelAccessManagement.Domain.Common;

namespace PersonnelAccessManagement.Application.Common.Extensions;

public static class PageExtensions
{
    public static IQueryable<TAny> GetPaged<TAny>(this IQueryable<TAny> query, FilterBase paged)
        where TAny : class
    {
        return GetPaged(query, paged.PageIndex, paged.PageSize);
    }

    private static IQueryable<TAny> GetPaged<TAny>(this IQueryable<TAny> query, int pageIndex, int pageSize) where TAny : class
    {
        if (pageIndex < 1) pageIndex = 1;
        if (pageSize < 1) pageSize = 10;
        return query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
    }
    
    public static PagedQueryResult<IEnumerable<TAny>> ToPagedQueryResult<TAny>(this IEnumerable<TAny> obj, FilterBase filter, int rowCount)
    {
        return obj.ToPagedQueryResult(filter.PageIndex, filter.PageSize, rowCount);
    }

    private static PagedQueryResult<IEnumerable<TAny>> ToPagedQueryResult<TAny>(this IEnumerable<TAny> obj, int pageIndex, int pageSize, int rowCount)
    {
        if (pageIndex < 1) pageIndex = 1;
        if (pageSize < 1) pageSize = 10;
        var pageCountCalc = (double)rowCount / pageSize;
        var pageCount = (int)Math.Ceiling(pageCountCalc);
        return new PagedQueryResult<IEnumerable<TAny>>(obj, pageIndex, rowCount, pageSize, pageCount);
    }
}