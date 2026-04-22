namespace EmployeeManagement.Api.Security;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public sealed class RequireRoleAttribute(string role) : Attribute
{
    public string Role { get; } = role;
}
