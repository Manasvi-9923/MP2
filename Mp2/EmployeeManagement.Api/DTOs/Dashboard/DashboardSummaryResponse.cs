namespace EmployeeManagement.Api.DTOs.Dashboard;

public class DashboardSummaryResponse
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int Inactive { get; set; }
    public int Departments { get; set; }
    public IReadOnlyList<DepartmentCountResponse> DepartmentBreakdown { get; set; } = [];
    public IReadOnlyList<RecentEmployeeResponse> RecentEmployees { get; set; } = [];
}
