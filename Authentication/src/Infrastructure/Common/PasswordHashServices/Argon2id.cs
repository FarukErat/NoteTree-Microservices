using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

using Application.Interfaces.Infrastructure;
using System.Collections.Specialized;

namespace Infrastructure.Common;

public sealed class Argon2idHasher : IPasswordHasher
{
    private readonly int DEGREE_OF_PARALLELISM = 4;
    private readonly int MEMORY_SIZE_KB = 65536; // 2^16 kb = 2^6 mb
    private readonly int ITERATIONS = 4;
    private readonly int SALT_SIZE = 16;
    private readonly int HASH_SIZE = 32;

    private string Parameters => $"?d={DEGREE_OF_PARALLELISM}&m={MEMORY_SIZE_KB}&t={ITERATIONS}&l={HASH_SIZE}&s={SALT_SIZE}";

    public sealed class HashingParameters
    {
        public string? Hash { get; set; }
        public int DegreeOfParallelism { get; set; }
        public int MemorySizeKB { get; set; }
        public int Iterations { get; set; }
        public int SaltSize { get; set; }
        public int HashSize { get; set; }
    }

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

        return Convert.ToBase64String(saltPlusHash) + Parameters;
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        HashingParameters? parameters = ExtractParameters(passwordHash);
        if (parameters is null || parameters.Hash is null) return false;
        try
        {
            byte[] saltPlusHash = Convert.FromBase64String(parameters.Hash);
            byte[] salt = new byte[parameters.SaltSize];
            byte[] hashBytes = new byte[saltPlusHash.Length - parameters.SaltSize];

            Buffer.BlockCopy(saltPlusHash, 0, salt, 0, salt.Length);
            Buffer.BlockCopy(saltPlusHash, salt.Length, hashBytes, 0, hashBytes.Length);

            using Argon2id hasher = new(Encoding.UTF8.GetBytes(password));
            hasher.Salt = salt;
            hasher.DegreeOfParallelism = parameters.DegreeOfParallelism;
            hasher.MemorySize = parameters.MemorySizeKB;
            hasher.Iterations = parameters.Iterations;

            byte[] computedHashBytes = hasher.GetBytes(parameters.HashSize);

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

    private static HashingParameters? ExtractParameters(string paramString)
    {
        string[] parts = paramString.Split('?');
        if (parts.Length != 2) return null;

        string[] paramsStr = parts[1].Split('&');
        if (paramsStr.Length != 5) return null;

        NameValueCollection parameters = [];
        foreach (string param in paramsStr)
        {
            string[] keyValue = param.Split('=');
            parameters.Add(keyValue[0], keyValue[1]);
        }

        string? d = parameters["d"]; if (d == null) return null;
        string? m = parameters["m"]; if (m == null) return null;
        string? t = parameters["t"]; if (t == null) return null;
        string? l = parameters["l"]; if (l == null) return null;
        string? s = parameters["s"]; if (s == null) return null;

        return new HashingParameters
        {
            Hash = parts[0],
            DegreeOfParallelism = int.Parse(d),
            MemorySizeKB = int.Parse(m),
            Iterations = int.Parse(t),
            HashSize = int.Parse(l),
            SaltSize = int.Parse(s)
        };
    }
}
