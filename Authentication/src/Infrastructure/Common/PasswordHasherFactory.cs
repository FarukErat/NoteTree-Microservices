using Application.Interfaces.Infrastructure;
using Domain.Enums;

namespace Infrastructure.Common;

public sealed class PasswordHasherFactory : IPasswordHasherFactory
{
    public IPasswordHasher? GetPasswordHasher(PasswordHashAlgorithm passwordHashAlgorithm)
    {
        return passwordHashAlgorithm switch
        {
            PasswordHashAlgorithm.Argon2id => new Argon2idHasher(),
            PasswordHashAlgorithm.Bcrypt => new BcryptHasher(),
            _ => null
        };
    }
}
