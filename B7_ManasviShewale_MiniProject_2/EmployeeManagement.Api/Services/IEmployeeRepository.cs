using EmployeeManagement.Api.Models;

namespace EmployeeManagement.Api.Services;

public interface IEmployeeRepository
{
    IQueryable<Employee> Query();
    Task<Employee?> GetByIdAsync(int id);
    Task AddAsync(Employee employee);
    void Remove(Employee employee);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    Task SaveChangesAsync();
}
