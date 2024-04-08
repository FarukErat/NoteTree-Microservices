using Domain.Enums;

namespace Application.Interfaces.Infrastructure;

public interface IPasswordHasherFactory
{
    IPasswordHasher? GetPasswordHasher(PasswordHashAlgorithm passwordHashAlgorithm);
}
