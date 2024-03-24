using Domain.Enums;

namespace Application.Interfaces.Infrastructure;

public interface IPasswordHashService
{
    (string, PasswordHashAlgorithm) HashPassword(string password);
    (bool, PasswordHashAlgorithm) VerifyPassword(string password, string passwordHash);
}
