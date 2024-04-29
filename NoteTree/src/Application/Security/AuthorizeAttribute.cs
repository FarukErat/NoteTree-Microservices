using Domain.Enums;

namespace Application.Security;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class AuthorizeAttribute : Attribute
{
    public Role[]? Roles;
}
