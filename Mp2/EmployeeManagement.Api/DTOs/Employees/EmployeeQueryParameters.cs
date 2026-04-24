namespace EmployeeManagement.Api.DTOs.Employees;

public class EmployeeQueryParameters
{
    public string? Search { get; set; }
    public string? Department { get; set; }
    public string? Status { get; set; }
    public string SortBy { get; set; } = "name";
    public string SortDir { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
