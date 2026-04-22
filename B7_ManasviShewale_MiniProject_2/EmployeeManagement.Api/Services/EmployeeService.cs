using EmployeeManagement.Api.Configuration;
using EmployeeManagement.Api.DTOs.Common;
using EmployeeManagement.Api.DTOs.Dashboard;
using EmployeeManagement.Api.DTOs.Employees;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EmployeeManagement.Api.Services;

public class EmployeeService(IEmployeeRepository repository)
{
    public async Task<PagedResult<EmployeeResponse>> GetAllAsync(EmployeeQueryParameters query)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : Math.Min(query.PageSize, 100);

        var employeesQuery = repository.Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = $"%{query.Search.Trim()}%";
            employeesQuery = employeesQuery.Where(employee =>
                EF.Functions.Like(employee.FirstName + " " + employee.LastName, search) ||
                EF.Functions.Like(employee.Email, search));
        }

        if (!string.IsNullOrWhiteSpace(query.Department) &&
            !string.Equals(query.Department, "all", StringComparison.OrdinalIgnoreCase))
        {
            employeesQuery = employeesQuery.Where(employee => employee.Department == query.Department);
        }

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            !string.Equals(query.Status, "all", StringComparison.OrdinalIgnoreCase))
        {
            employeesQuery = employeesQuery.Where(employee => employee.Status == query.Status);
        }

        employeesQuery = ApplySorting(employeesQuery, query.SortBy, query.SortDir);

        var totalCount = await employeesQuery.CountAsync();
        var items = await employeesQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(MapEmployeeExpression())
            .ToListAsync();

        return new PagedResult<EmployeeResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<EmployeeResponse?> GetByIdAsync(int id)
    {
        var employee = await repository.GetByIdAsync(id);
        return employee is null ? null : MapEmployee(employee);
    }

    public async Task<DashboardSummaryResponse> GetDashboardAsync()
    {
        var employees = repository.Query().AsNoTracking();

        var total = await employees.CountAsync();
        var active = await employees.CountAsync(employee => employee.Status == "Active");
        var inactive = await employees.CountAsync(employee => employee.Status == "Inactive");
        var departmentBreakdown = await employees
            .GroupBy(employee => employee.Department)
            .Select(group => new DepartmentCountResponse
            {
                Department = group.Key,
                Count = group.Count()
            })
            .OrderBy(item => item.Department)
            .ToListAsync();

        var recentEmployees = await employees
            .OrderByDescending(employee => employee.CreatedAt)
            .ThenByDescending(employee => employee.Id)
            .Take(5)
            .Select(employee => new RecentEmployeeResponse
            {
                Id = employee.Id,
                FullName = employee.FirstName + " " + employee.LastName,
                Department = employee.Department,
                Designation = employee.Designation,
                Status = employee.Status
            })
            .ToListAsync();

        return new DashboardSummaryResponse
        {
            Total = total,
            Active = active,
            Inactive = inactive,
            Departments = departmentBreakdown.Count,
            DepartmentBreakdown = departmentBreakdown,
            RecentEmployees = recentEmployees
        };
    }

    public async Task<ServiceResult<EmployeeResponse>> CreateAsync(EmployeeRequest request)
    {
        var validationError = ValidateEmployeeRequest(request);
        if (validationError is not null)
        {
            return ServiceResult<EmployeeResponse>.BadRequest(validationError);
        }

        if (await repository.EmailExistsAsync(request.Email))
        {
            return ServiceResult<EmployeeResponse>.Conflict("Email already exists.");
        }

        var now = DateTime.UtcNow;
        var employee = new Employee
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone.Trim(),
            Department = request.Department.Trim(),
            Designation = request.Designation.Trim(),
            Salary = request.Salary,
            JoinDate = DateTime.SpecifyKind(request.JoinDate.Date, DateTimeKind.Utc),
            Status = request.Status.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        await repository.AddAsync(employee);
        await repository.SaveChangesAsync();

        return ServiceResult<EmployeeResponse>.Created(MapEmployee(employee), "Employee created successfully.");
    }

    public async Task<ServiceResult<EmployeeResponse>> UpdateAsync(int id, EmployeeRequest request)
    {
        var validationError = ValidateEmployeeRequest(request);
        if (validationError is not null)
        {
            return ServiceResult<EmployeeResponse>.BadRequest(validationError);
        }

        var employee = await repository.GetByIdAsync(id);
        if (employee is null)
        {
            return ServiceResult<EmployeeResponse>.NotFound("Employee not found.");
        }

        if (await repository.EmailExistsAsync(request.Email, id))
        {
            return ServiceResult<EmployeeResponse>.Conflict("Email already exists.");
        }

        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.Email = request.Email.Trim();
        employee.Phone = request.Phone.Trim();
        employee.Department = request.Department.Trim();
        employee.Designation = request.Designation.Trim();
        employee.Salary = request.Salary;
        employee.JoinDate = DateTime.SpecifyKind(request.JoinDate.Date, DateTimeKind.Utc);
        employee.Status = request.Status.Trim();
        employee.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync();

        return ServiceResult<EmployeeResponse>.Ok(MapEmployee(employee), "Employee updated successfully.");
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var employee = await repository.GetByIdAsync(id);
        if (employee is null)
        {
            return ServiceResult.NotFound("Employee not found.");
        }

        repository.Remove(employee);
        await repository.SaveChangesAsync();

        return ServiceResult.Ok("Employee deleted successfully.");
    }

    private static IQueryable<Employee> ApplySorting(IQueryable<Employee> query, string? sortBy, string? sortDir)
    {
        var descending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
        var field = (sortBy ?? "name").Trim().ToLowerInvariant();

        return field switch
        {
            "salary" => descending ? query.OrderByDescending(employee => employee.Salary) : query.OrderBy(employee => employee.Salary),
            "joindate" => descending ? query.OrderByDescending(employee => employee.JoinDate) : query.OrderBy(employee => employee.JoinDate),
            _ => descending
                ? query.OrderByDescending(employee => employee.LastName).ThenByDescending(employee => employee.FirstName)
                : query.OrderBy(employee => employee.LastName).ThenBy(employee => employee.FirstName)
        };
    }

    private static string? ValidateEmployeeRequest(EmployeeRequest request)
    {
        if (!AppConstants.Departments.Contains(request.Department.Trim()))
        {
            return "Department must be one of: Engineering, Marketing, HR, Finance, Operations.";
        }

        if (!AppConstants.Statuses.Contains(request.Status.Trim()))
        {
            return "Status must be Active or Inactive.";
        }

        return null;
    }

    private static EmployeeResponse MapEmployee(Employee employee)
    {
        return new EmployeeResponse
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            FullName = $"{employee.FirstName} {employee.LastName}",
            Email = employee.Email,
            Phone = employee.Phone,
            Department = employee.Department,
            Designation = employee.Designation,
            Salary = employee.Salary,
            JoinDate = employee.JoinDate,
            Status = employee.Status,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };
    }

    private static Expression<Func<Employee, EmployeeResponse>> MapEmployeeExpression()
    {
        return employee => new EmployeeResponse
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            FullName = employee.FirstName + " " + employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            Department = employee.Department,
            Designation = employee.Designation,
            Salary = employee.Salary,
            JoinDate = employee.JoinDate,
            Status = employee.Status,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };
    }
}
