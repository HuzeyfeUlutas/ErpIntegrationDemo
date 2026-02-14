namespace PersonnelAccessManagement.Application.Common.Models;

public class PagedQueryResult<TResult>
{
    public PagedQueryResult(TResult result, int pageIndex, int rowCount, int pageSize, int pageCount)
    {
        Result = result;
        PageIndex = pageIndex;
        PageSize = pageSize;
        PageCount = pageCount;
        RowCount = rowCount;
    }

    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int PageCount { get; set; }
    public int RowCount { get; set; }
    public TResult? Result { get; set; }
}