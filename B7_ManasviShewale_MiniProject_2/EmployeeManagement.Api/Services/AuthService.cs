using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs.Auth;
using EmployeeManagement.Api.DTOs.Common;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagement.Api.Services;

public class AuthService(AppDbContext dbContext, IConfiguration configuration)
{
    public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var normalizedUsername = request.Username.Trim().ToLower();
        var existingUser = await dbContext.AppUsers
            .AnyAsync(user => user.Username.ToLower() == normalizedUsername);

        if (existingUser)
        {
            return ServiceResult<AuthResponse>.Conflict("Username already exists.");
        }

        var role = string.Equals(request.Role, "Admin", StringComparison.OrdinalIgnoreCase)
            ? "Admin"
            : "Viewer";

        var user = new AppUser
        {
            Username = request.Username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, 12),
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.AppUsers.Add(user);
        await dbContext.SaveChangesAsync();

        return ServiceResult<AuthResponse>.Ok(new AuthResponse
        {
            Success = true,
            Username = user.Username,
            Role = user.Role,
            Message = "User registered successfully."
        }, "User registered successfully.");
    }

    public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var username = request.Username.Trim().ToLower();
        var user = await dbContext.AppUsers
            .FirstOrDefaultAsync(appUser => appUser.Username.ToLower() == username);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return ServiceResult<AuthResponse>.Unauthorized("Invalid username or password.");
        }

        return ServiceResult<AuthResponse>.Ok(new AuthResponse
        {
            Success = true,
            Username = user.Username,
            Role = user.Role,
            Token = GenerateToken(user),
            Message = "Login successful."
        }, "Login successful.");
    }

    private string GenerateToken(AppUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSetting("Jwt:Key", "Token:SecretKey")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: GetSetting("Jwt:Issuer", "Token:Issuer"),
            audience: GetSetting("Jwt:Audience", "Token:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(GetSetting("Jwt:ExpiryHours", "Token:ExpiryHours", "8"))),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GetSetting(params string[] keys)
    {
        foreach (var key in keys)
        {
            var value = configuration[key];
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        throw new InvalidOperationException($"Missing required configuration value. Checked: {string.Join(", ", keys)}");
    }
}
