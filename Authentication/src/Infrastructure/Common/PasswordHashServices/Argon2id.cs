using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

using Application.Interfaces.Infrastructure;
using Domain.Enums;

namespace Infrastructure.Common;

public sealed class Argon2idHasher : IPasswordHasher
{
    private readonly int DEGREE_OF_PARALLELISM = 4;
    private readonly int MEMORY_SIZE_KB = 65536; // 2^16 kb = 2^6 mb
    private readonly int ITERATIONS = 4;
    private readonly int SALT_SIZE = 16;
    private readonly int HASH_SIZE = 32;

    public string HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt = new byte[SALT_SIZE];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Hash the password with Argon2id
        using Argon2id hasher = new(Encoding.UTF8.GetBytes(password));
        hasher.Salt = salt;
        hasher.DegreeOfParallelism = DEGREE_OF_PARALLELISM;
        hasher.MemorySize = MEMORY_SIZE_KB;
        hasher.Iterations = ITERATIONS;

        byte[] hashBytes = hasher.GetBytes(HASH_SIZE);
        byte[] saltPlusHash = new byte[salt.Length + hashBytes.Length];
        Buffer.BlockCopy(salt, 0, saltPlusHash, 0, salt.Length);
        Buffer.BlockCopy(hashBytes, 0, saltPlusHash, salt.Length, hashBytes.Length);

        return Convert.ToBase64String(saltPlusHash);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            byte[] saltPlusHash = Convert.FromBase64String(passwordHash);
            byte[] salt = new byte[SALT_SIZE];
            byte[] hashBytes = new byte[saltPlusHash.Length - SALT_SIZE];

            Buffer.BlockCopy(saltPlusHash, 0, salt, 0, salt.Length);
            Buffer.BlockCopy(saltPlusHash, salt.Length, hashBytes, 0, hashBytes.Length);

            using Argon2id hasher = new(Encoding.UTF8.GetBytes(password));
            hasher.Salt = salt;
            hasher.DegreeOfParallelism = DEGREE_OF_PARALLELISM;
            hasher.MemorySize = MEMORY_SIZE_KB;
            hasher.Iterations = ITERATIONS;

            byte[] computedHashBytes = hasher.GetBytes(HASH_SIZE);

            return SlowEquals(hashBytes, computedHashBytes);
        }
        catch
        {
            return false;
        }
    }

    private static bool SlowEquals(byte[] a, byte[] b)
    {
        uint diff = (uint)a.Length ^ (uint)b.Length;
        for (int i = 0; i < a.Length && i < b.Length; i++)
        {
            diff |= (uint)(a[i] ^ b[i]);
        }
        return diff == 0;
    }
}
