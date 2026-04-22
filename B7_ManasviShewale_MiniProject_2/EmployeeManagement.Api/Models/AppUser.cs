using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Models;

public class AppUser
{
    public int Id { get; set; }

    [MaxLength(100)]
    public required string Username { get; set; }

    public required string PasswordHash { get; set; }

    [MaxLength(20)]
    public required string Role { get; set; }

    public DateTime CreatedAt { get; set; }
}
