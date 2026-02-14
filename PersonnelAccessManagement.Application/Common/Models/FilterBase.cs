namespace PersonnelAccessManagement.Application.Common.Models;

public abstract class FilterBase 
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string[] OrderBy { get; set; } = [];
}