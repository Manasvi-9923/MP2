using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

public class DatabaseSeeder(AppDbContext dbContext)
{
    public async Task SeedAsync()
    {
        await dbContext.Database.EnsureCreatedAsync();

        if (!await dbContext.AppUsers.AnyAsync())
        {
            dbContext.AppUsers.AddRange(
                new AppUser
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                },
                new AppUser
                {
                    Username = "viewer",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("viewer123"),
                    Role = "Viewer",
                    CreatedAt = DateTime.UtcNow
                });
        }

        if (!await dbContext.Employees.AnyAsync())
        {
            dbContext.Employees.AddRange(BuildSeedEmployees());
        }

        await dbContext.SaveChangesAsync();
    }

    private static IEnumerable<Employee> BuildSeedEmployees()
    {
        var createdAt = DateTime.UtcNow.AddDays(-30);

        return
        [
            CreateEmployee("Priya", "Menon", "priya.menon@xyz.com", "9876543210", "Engineering", "Software Engineer", 750000, "2022-06-15", "Active", createdAt.AddDays(1)),
            CreateEmployee("Arjun", "Sharma", "arjun.sharma@xyz.com", "9876501234", "Engineering", "Senior Engineer", 950000, "2021-02-10", "Active", createdAt.AddDays(2)),
            CreateEmployee("Meera", "Iyer", "meera.iyer@xyz.com", "9898989898", "HR", "HR Executive", 520000, "2020-11-01", "Active", createdAt.AddDays(3)),
            CreateEmployee("Rohan", "Patil", "rohan.patil@xyz.com", "9812345678", "Marketing", "Marketing Specialist", 600000, "2023-01-05", "Inactive", createdAt.AddDays(4)),
            CreateEmployee("Sneha", "Desai", "sneha.desai@xyz.com", "9822334455", "Finance", "Accountant", 580000, "2019-09-20", "Active", createdAt.AddDays(5)),
            CreateEmployee("Vikram", "Singh", "vikram.singh@xyz.com", "9800011223", "Operations", "Operations Executive", 540000, "2021-07-12", "Inactive", createdAt.AddDays(6)),
            CreateEmployee("Anjali", "Kulkarni", "anjali.kulkarni@xyz.com", "9876123456", "Engineering", "UI Developer", 680000, "2022-03-18", "Active", createdAt.AddDays(7)),
            CreateEmployee("Rahul", "Verma", "rahul.verma@xyz.com", "9811122233", "Marketing", "Content Strategist", 510000, "2020-05-14", "Active", createdAt.AddDays(8)),
            CreateEmployee("Kavya", "Rao", "kavya.rao@xyz.com", "9890011223", "HR", "Talent Partner", 610000, "2021-10-09", "Inactive", createdAt.AddDays(9)),
            CreateEmployee("Sanjay", "Nair", "sanjay.nair@xyz.com", "9800998877", "Finance", "Finance Manager", 880000, "2018-12-01", "Active", createdAt.AddDays(10)),
            CreateEmployee("Divya", "Gupta", "divya.gupta@xyz.com", "9877001122", "Operations", "Operations Manager", 820000, "2019-04-21", "Active", createdAt.AddDays(11)),
            CreateEmployee("Manish", "Joshi", "manish.joshi@xyz.com", "9865321470", "Engineering", "QA Engineer", 560000, "2023-02-16", "Active", createdAt.AddDays(12)),
            CreateEmployee("Nidhi", "Kapoor", "nidhi.kapoor@xyz.com", "9844556677", "Marketing", "Brand Manager", 720000, "2020-08-30", "Inactive", createdAt.AddDays(13)),
            CreateEmployee("Amit", "Bose", "amit.bose@xyz.com", "9833445566", "Finance", "Financial Analyst", 640000, "2022-11-11", "Active", createdAt.AddDays(14)),
            CreateEmployee("Shruti", "Banerjee", "shruti.banerjee@xyz.com", "9822003344", "Operations", "Logistics Coordinator", 500000, "2021-06-03", "Inactive", createdAt.AddDays(15))
        ];
    }

    private static Employee CreateEmployee(
        string firstName,
        string lastName,
        string email,
        string phone,
        string department,
        string designation,
        decimal salary,
        string joinDate,
        string status,
        DateTime createdAt)
    {
        return new Employee
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Department = department,
            Designation = designation,
            Salary = salary,
            JoinDate = DateTime.SpecifyKind(DateTime.Parse(joinDate), DateTimeKind.Utc),
            Status = status,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };
    }
}
