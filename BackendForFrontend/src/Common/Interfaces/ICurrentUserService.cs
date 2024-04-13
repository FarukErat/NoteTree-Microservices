using Domain.Enums;

namespace Common.Interfaces;

public interface ICurrentUserService
{
    bool AuthorizeUser(
        Role[] roles,
        Permission[] permissions,
        Policy[] policies);
}
