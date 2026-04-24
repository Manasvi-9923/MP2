namespace EmployeeManagement.Api.Configuration;

public class TokenOptions
{
    public const string SectionName = "Token";

    public string SecretKey { get; set; } = "super-secret-key-change-this-for-production";
    public string Issuer { get; set; } = "EmployeeManagement.Api";
    public string Audience { get; set; } = "EmployeeManagement.Frontend";

    public int ExpiryMinutes { get; set; } = 240;
}
