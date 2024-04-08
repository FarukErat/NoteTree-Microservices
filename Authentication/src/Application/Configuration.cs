using Domain.Enums;

namespace Application;

public static class Configurations
{
    public static readonly PasswordHashAlgorithm PasswordHashAlgorithm = PasswordHashAlgorithm.Argon2id;
}
