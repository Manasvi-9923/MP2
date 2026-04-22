namespace EmployeeManagement.Api.Services;

public interface ITokenService
{
    string GenerateToken(string username, string role);
    bool TryValidateToken(string token, out string username, out string role);
}
