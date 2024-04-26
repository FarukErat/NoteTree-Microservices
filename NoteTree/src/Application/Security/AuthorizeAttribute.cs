using Domain.Enums;

namespace Application.Security;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeAttribute : Attribute
{
    public Role[]? Roles;
}
