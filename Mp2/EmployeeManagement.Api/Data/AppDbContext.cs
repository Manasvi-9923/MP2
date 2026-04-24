using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Salary).HasColumnType("decimal(18,2)");
            entity.Property(x => x.JoinDate).HasColumnType("datetime2");
            entity.Property(x => x.CreatedAt).HasColumnType("datetime2");
            entity.Property(x => x.UpdatedAt).HasColumnType("datetime2");
        });
        modelBuilder.Entity<Employee>().HasData(GetSeedEmployees());

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasIndex(x => x.Username).IsUnique();
            entity.Property(x => x.CreatedAt).HasColumnType("datetime2");
        });
        modelBuilder.Entity<AppUser>().HasData(GetSeedUsers());
    }

    private static Employee[] GetSeedEmployees()
    {
        var createdAt = new DateTime(2026, 03, 01, 0, 0, 0, DateTimeKind.Utc);

        return
        [
            CreateEmployee(1, "Priya", "Menon", "priya.menon@xyz.com", "9876543210", "Engineering", "Software Engineer", 750000m, new DateTime(2022, 6, 15, 0, 0, 0, DateTimeKind.Utc), "Active", createdAt.AddDays(1)),
            CreateEmployee(2, "Arjun", "Sharma", "arjun.sharma@xyz.com", "9876501234", "Engineering", "Senior Engineer", 950000m, new DateTime(2021, 2, 10, 0, 0, 0, DateTimeKind.Utc), "Active", createdAt.AddDays(2)),
            CreateEmployee(3, "Meera", "Iyer", "meera.iyer@xyz.com", "9898989898", "HR", "HR Executive", 520000m, new DateTime(2020, 11, 1, 0, 0, 0, DateTimeKind.Utc), "Active", createdAt.AddDays(3)),
            CreateEmployee(4, "Rohan", "Patil", "rohan.patil@xyz.com", "9812345678", "Marketing", "Marketing Specialist", 600000m, new DateTime(2023, 1, 5, 0, 0, 0, DateTimeKind.Utc), "Inactive", createdAt.AddDays(4)),
            CreateEmployee(5, "Sneha", "Desai", "sneha.desai@xyz.com", "9822334455", "Finance", "Accountant", 580000m, new DateTime(2019, 9, 20, 0, 0, 0, DateTimeKind.Utc), "Active", createdAt.AddDays(5)),
            CreateEmployee(6, "Vikram", "Singh", "vikram.singh@xyz.com", "9800011223", "Operations", "Operations Executive", 540000m, new DateTime(2021, 7, 12, 0, 0, 0, DateTimeKind.Utc), "Inactive", createdAt.AddDays(6)),
            CreateEmployee(7, "Anjali", "Kulkarni", "anjali.kulkarni@xyz.com", "9876123456", "Engineering", "UI Developer", 680000m, new DateTime(2022, 3, 18, 0, 0, 0, DateTimeKind.Utc), "Active", createdAt.AddDays(7)),
            CreateEmployee(8, "Rahul", "Verma", "rahul.verma@xyz.com", "9811122233", "Marketing", "Content Strategist", 510000m, new DateTime(2020, 5, 14, 0, 0, 0, DateTimeKind.Utc), "Active", createdAt.AddDays(8)),
            CreateEmployee(9, "Kavya", "Rao", "kavya.rao@xyz.com", "9890011223", "HR", "Talent Partner", 610000m, new DateTime(2021, 10, 9, 0, 0, 0, DateTimeKind.Utc), "Inactive", createdAt.AddDays(9)),
            CreateEmployee(10, "Sanjay", "Nair", "sanjay.nair@xyz.com", "9800998877", "Finance", "Finance Manager", 880000m, new DateTime(2018, 12, 1, 0, 0, 0, DateTimeKind.Utc), "Active", createdAt.AddDays(10)),
            CreateEmployee(11, "Divya", "Gupta", "divya.gupta@xyz.com", "9877001122", "Operations", "Operations Manager", 820000m, new DateTime(2019, 4, 21, 0, 0, 0, DateTimeKind.Utc), "Active", createdAt.AddDays(11)),
            CreateEmployee(12, "Manish", "Joshi", "manish.joshi@xyz.com", "9865321470", "Engineering", "QA Engineer", 560000m, new DateTime(2023, 2, 16, 0, 0, 0, DateTimeKind.Utc), "Active", createdAt.AddDays(12)),
            CreateEmployee(13, "Nidhi", "Kapoor", "nidhi.kapoor@xyz.com", "9844556677", "Marketing", "Brand Manager", 720000m, new DateTime(2020, 8, 30, 0, 0, 0, DateTimeKind.Utc), "Inactive", createdAt.AddDays(13)),
            CreateEmployee(14, "Amit", "Bose", "amit.bose@xyz.com", "9833445566", "Finance", "Financial Analyst", 640000m, new DateTime(2022, 11, 11, 0, 0, 0, DateTimeKind.Utc), "Active", createdAt.AddDays(14)),
            CreateEmployee(15, "Shruti", "Banerjee", "shruti.banerjee@xyz.com", "9822003344", "Operations", "Logistics Coordinator", 500000m, new DateTime(2021, 6, 3, 0, 0, 0, DateTimeKind.Utc), "Inactive", createdAt.AddDays(15))
        ];
    }

    private static Employee CreateEmployee(
        int id,
        string firstName,
        string lastName,
        string email,
        string phone,
        string department,
        string designation,
        decimal salary,
        DateTime joinDate,
        string status,
        DateTime createdAt)
    {
        return new Employee
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Department = department,
            Designation = designation,
            Salary = salary,
            JoinDate = joinDate,
            Status = status,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };
    }

    private static AppUser[] GetSeedUsers()
    {
        var createdAt = new DateTime(2026, 03, 01, 0, 0, 0, DateTimeKind.Utc);

        return
        [
            new AppUser
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "$2a$11$vRojhxZWmUInP1sWdOccr.UP7H7MsJTCEsubFcn9oBeQBH9vpABjC",
                Role = "Admin",
                CreatedAt = createdAt
            },
            new AppUser
            {
                Id = 2,
                Username = "viewer",
                PasswordHash = "$2a$11$6KV0ibwC2Fywk7FRNlRCDuWMWbj/wmLSQdUl87KkK0cnSUamIAjpq",
                Role = "Viewer",
                CreatedAt = createdAt
            }
        ];
    }
}
