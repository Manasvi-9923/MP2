namespace EmployeeManagement.Api.DTOs.Common;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public IReadOnlyList<T> Data
    {
        get => Items;
        set => Items = value;
    }

    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage => Page < TotalPages;
    public bool HasPrevPage => Page > 1;
}
