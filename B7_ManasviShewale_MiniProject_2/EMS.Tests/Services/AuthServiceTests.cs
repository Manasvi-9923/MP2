using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs.Auth;
using EmployeeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EMS.Tests.Services;

[TestFixture]
public class AuthServiceTests
{
    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        await using var db = CreateDbContext();
        var service = new AuthService(db, CreateConfiguration().Object);

        var result = await service.LoginAsync(new LoginRequest
        {
            Username = "admin",
            Password = "admin123"
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.Token, Is.Not.Empty);
            Assert.That(result.Data.Role, Is.EqualTo("Admin"));
        });
    }

    [Test]
    public async Task LoginAsync_WrongPassword_ReturnsFailure()
    {
        await using var db = CreateDbContext();
        var service = new AuthService(db, CreateConfiguration().Object);

        var result = await service.LoginAsync(new LoginRequest
        {
            Username = "admin",
            Password = "wrong-password"
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(401));
            Assert.That(result.Message, Does.Contain("Invalid username or password"));
        });
    }

    [Test]
    public async Task RegisterAsync_DuplicateUsername_ReturnsFailure()
    {
        await using var db = CreateDbContext();
        var service = new AuthService(db, CreateConfiguration().Object);

        var result = await service.RegisterAsync(new RegisterRequest
        {
            Username = "admin",
            Password = "admin123",
            Role = "Admin"
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(409));
            Assert.That(result.Message, Does.Contain("Username already exists"));
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

    private static Mock<IConfiguration> CreateConfiguration()
    {
        var configuration = new Mock<IConfiguration>();
        configuration.SetupGet(config => config["Jwt:Key"])
            .Returns("TestSecretKey_AtLeast32Characters_Long!");
        configuration.SetupGet(config => config["Jwt:Issuer"])
            .Returns("EMS.API");
        configuration.SetupGet(config => config["Jwt:Audience"])
            .Returns("EMS.Client");
        configuration.SetupGet(config => config["Jwt:ExpiryHours"])
            .Returns("8");

        return configuration;
    }
}
