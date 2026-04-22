using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs.Employees;
using EmployeeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace EMS.Tests.Integration;

[TestFixture]
public class EmployeeIntegrationTests
{
    [Test]
    public async Task CreateAsync_AddsEmployee_AndEmployeeIsRetrievable()
    {
        await using var db = CreateDbContext();
        var service = new EmployeeService(new EmployeeRepository(db));

        var createResult = await service.CreateAsync(new EmployeeRequest
        {
            FirstName = "Kiran",
            LastName = "Patel",
            Email = "kiran.patel@xyz.com",
            Phone = "9876543201",
            Department = "Finance",
            Designation = "Analyst",
            Salary = 650000m,
            JoinDate = new DateTime(2024, 2, 1),
            Status = "Active"
        });

        var stored = await db.Employees.FirstOrDefaultAsync(employee => employee.Email == "kiran.patel@xyz.com");

        Assert.Multiple(() =>
        {
            Assert.That(createResult.Success, Is.True);
            Assert.That(stored, Is.Not.Null);
            Assert.That(stored!.FirstName, Is.EqualTo("Kiran"));
        });
    }

    [Test]
    public async Task DeleteAsync_RemovesEmployee()
    {
        await using var db = CreateDbContext();
        var service = new EmployeeService(new EmployeeRepository(db));

        var result = await service.DeleteAsync(1);
        var employeeExists = await db.Employees.AnyAsync(employee => employee.Id == 1);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(employeeExists, Is.False);
        });
    }

    [Test]
    public async Task CreateAsync_DuplicateEmail_ReturnsConflict()
    {
        await using var db = CreateDbContext();
        var service = new EmployeeService(new EmployeeRepository(db));

        var result = await service.CreateAsync(new EmployeeRequest
        {
            FirstName = "Duplicate",
            LastName = "User",
            Email = "priya.menon@xyz.com",
            Phone = "9876543202",
            Department = "Engineering",
            Designation = "Developer",
            Salary = 700000m,
            JoinDate = new DateTime(2024, 1, 1),
            Status = "Active"
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(409));
        });
    }

    [Test]
    public async Task GetDashboardAsync_ReturnsExpectedSummary()
    {
        await using var db = CreateDbContext();
        var service = new EmployeeService(new EmployeeRepository(db));

        var result = await service.GetDashboardAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result.Total, Is.EqualTo(15));
            Assert.That(result.Active, Is.EqualTo(10));
            Assert.That(result.Inactive, Is.EqualTo(5));
            Assert.That(result.Departments, Is.EqualTo(5));
            Assert.That(result.RecentEmployees.Count, Is.EqualTo(5));
        });
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new AppDbContext(options);
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        return db;
    }
}
