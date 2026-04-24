using EmployeeManagement.Api.Controllers;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs.Common;
using EmployeeManagement.Api.DTOs.Employees;
using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.Tests.Controllers;

[TestFixture]
public class EmployeesControllerTests
{
    [Test]
    public async Task GetAll_ReturnsPagedEnvelope_WithExpectedFlags()
    {
        await using var db = CreateDbContext();
        var controller = new EmployeesController(new EmployeeService(new EmployeeRepository(db)));

        var result = await controller.GetAll(new EmployeeQueryParameters
        {
            Page = 1,
            PageSize = 5,
            SortBy = "name",
            SortDir = "asc"
        });

        var ok = result.Result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);

        var payload = ok!.Value as PagedResult<EmployeeResponse>;
        Assert.That(payload, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(payload!.Items.Count, Is.EqualTo(5));
            Assert.That(payload.TotalCount, Is.EqualTo(15));
            Assert.That(payload.Page, Is.EqualTo(1));
            Assert.That(payload.TotalPages, Is.EqualTo(3));
            Assert.That(payload.HasNextPage, Is.True);
            Assert.That(payload.HasPrevPage, Is.False);
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
