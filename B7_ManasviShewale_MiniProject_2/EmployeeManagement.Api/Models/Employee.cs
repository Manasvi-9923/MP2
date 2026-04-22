using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Models;

public class Employee
{
    public int Id { get; set; }

    [MaxLength(100)]
    public required string FirstName { get; set; }

    [MaxLength(100)]
    public required string LastName { get; set; }

    [MaxLength(200)]
    public required string Email { get; set; }

    [MaxLength(15)]
    public required string Phone { get; set; }

    [MaxLength(50)]
    public required string Department { get; set; }

    [MaxLength(100)]
    public required string Designation { get; set; }

    public decimal Salary { get; set; }

    public DateTime JoinDate { get; set; }

    [MaxLength(10)]
    public required string Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
