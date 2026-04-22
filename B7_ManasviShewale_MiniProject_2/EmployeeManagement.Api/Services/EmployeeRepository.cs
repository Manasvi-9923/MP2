using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

public class EmployeeRepository(AppDbContext dbContext) : IEmployeeRepository
{
    public IQueryable<Employee> Query()
    {
        return dbContext.Employees.AsQueryable();
    }

    public Task<Employee?> GetByIdAsync(int id)
    {
        return dbContext.Employees.FirstOrDefaultAsync(employee => employee.Id == id);
    }

    public Task AddAsync(Employee employee)
    {
        return dbContext.Employees.AddAsync(employee).AsTask();
    }

    public void Remove(Employee employee)
    {
        dbContext.Employees.Remove(employee);
    }

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var normalizedEmail = email.Trim().ToLower();

        return dbContext.Employees.AnyAsync(employee =>
            employee.Email.ToLower() == normalizedEmail &&
            (!excludeId.HasValue || employee.Id != excludeId.Value));
    }

    public Task SaveChangesAsync()
    {
        return dbContext.SaveChangesAsync();
    }
}
