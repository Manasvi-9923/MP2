namespace EmployeeManagement.Api.Security;

public static class HttpContextUserExtensions
{
    private const string CurrentUserKey = "CurrentUser";

    public static void SetCurrentUser(this HttpContext context, CurrentUserContext user)
    {
        context.Items[CurrentUserKey] = user;
    }

    public static CurrentUserContext? GetCurrentUser(this HttpContext context)
    {
        return context.Items.TryGetValue(CurrentUserKey, out var value)
            ? value as CurrentUserContext
            : null;
    }
}
