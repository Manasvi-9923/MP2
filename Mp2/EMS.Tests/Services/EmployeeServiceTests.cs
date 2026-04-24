using EmployeeManagement.Api.DTOs.Employees;
using EmployeeManagement.Api.Models;
using EmployeeManagement.Api.Services;
using Moq;

namespace EMS.Tests.Services;

[TestFixture]
public class EmployeeServiceTests
{
    private Mock<IEmployeeRepository> _repoMock = null!;
    private EmployeeService _service = null!;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<IEmployeeRepository>();
        _service = new EmployeeService(_repoMock.Object);
    }

    [Test]
    public async Task GetByIdAsync_ValidId_ReturnsMappedDto()
    {
        var fakeEmployee = new Employee
        {
            Id = 1,
            FirstName = "Priya",
            LastName = "Menon",
            Email = "priya.menon@xyz.com",
            Phone = "9876543210",
            Department = "Engineering",
            Designation = "Software Engineer",
            Salary = 750000m,
            JoinDate = new DateTime(2022, 6, 15, 0, 0, 0, DateTimeKind.Utc),
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repoMock.Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(fakeEmployee);

        var result = await _service.GetByIdAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.FirstName, Is.EqualTo("Priya"));
            Assert.That(result.FullName, Is.EqualTo("Priya Menon"));
        });

        _repoMock.Verify(repository => repository.GetByIdAsync(1), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
        _repoMock.Setup(repository => repository.GetByIdAsync(9999))
            .ReturnsAsync((Employee?)null);

        var result = await _service.GetByIdAsync(9999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateAsync_ValidEmployee_CallsAddAsync()
    {
        _repoMock.Setup(repository => repository.EmailExistsAsync("new.employee@xyz.com", null))
            .ReturnsAsync(false);

        var request = new EmployeeRequest
        {
            FirstName = "New",
            LastName = "Employee",
            Email = "new.employee@xyz.com",
            Phone = "9876512345",
            Department = "Engineering",
            Designation = "Developer",
            Salary = 700000m,
            JoinDate = new DateTime(2024, 1, 15),
            Status = "Active"
        };

        var result = await _service.CreateAsync(request);

        Assert.That(result.Success, Is.True);
        _repoMock.Verify(repository => repository.AddAsync(It.Is<Employee>(employee =>
            employee.Email == "new.employee@xyz.com" &&
            employee.FirstName == "New")), Times.Once);
        _repoMock.Verify(repository => repository.SaveChangesAsync(), Times.Once);
    }
}
