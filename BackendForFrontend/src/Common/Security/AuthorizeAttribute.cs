using Domain.Enums;

namespace Common.Security;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeAttribute : Attribute
{
    public Role[]? Roles;
    public Permission[]? Permissions;
    public Policy[]? Policies;
}
