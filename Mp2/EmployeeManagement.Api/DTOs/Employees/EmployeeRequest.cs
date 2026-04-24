using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs.Employees;

public class EmployeeRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{10}$")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Department { get; set; } = string.Empty;

    [Required]
    public string Designation { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999")]
    public decimal Salary { get; set; }

    [Required]
    public DateTime JoinDate { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;
}
