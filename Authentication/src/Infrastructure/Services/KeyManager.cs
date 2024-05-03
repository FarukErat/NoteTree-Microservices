using System.Security.Cryptography;

namespace Infrastructure.Services;

public static class KeyManager
{
    public static class Directories
    {
        public static string CurrentDir => Directory.GetCurrentDirectory();
        public static string ParentDir => Directory.GetParent(CurrentDir)?.FullName ?? CurrentDir;
        public static string KeyFolder => Path.Combine(ParentDir, "Keys");
    }

    public static (string keyId, byte[] privateKey, byte[] publicKey) LoadOrCreateRsaKeyPair()
    {
        if (!Directory.Exists(Directories.KeyFolder))
        {
            Directory.CreateDirectory(Directories.KeyFolder);
        }

        string? currentKeyId = GetCurrentKeyId();
        if (string.IsNullOrEmpty(currentKeyId))
        {
            using RSA rsa = RSA.Create();
            byte[] privateKey = rsa.ExportRSAPrivateKey();
            byte[] publicKey = rsa.ExportRSAPublicKey();

            string newKeyId = Guid.NewGuid().ToString();
            string newKeyPath = Path.Combine(Directories.KeyFolder, newKeyId);
            Directory.CreateDirectory(newKeyPath);

            string privateKeyFilePath = Path.Combine(newKeyPath, "_private_key.pem");
            string publicKeyFilePath = Path.Combine(newKeyPath, "_public_key.pem");

            File.WriteAllBytes(privateKeyFilePath, privateKey);
            File.WriteAllBytes(publicKeyFilePath, publicKey);

            UpdateCurrentKeyId(newKeyId);

            return (newKeyId, privateKey, publicKey);
        }
        else
        {
            byte[] privateKey = File.ReadAllBytes(Path.Combine(Directories.KeyFolder, currentKeyId, "_private_key.pem"));
            byte[] publicKey = File.ReadAllBytes(Path.Combine(Directories.KeyFolder, currentKeyId, "_public_key.pem"));

            return (currentKeyId, privateKey, publicKey);
        }
    }

    public static string? GetCurrentKeyId()
    {
        string latestKeyFilePath = Path.Combine(Directories.KeyFolder, "current_key_id.txt");
        if (File.Exists(latestKeyFilePath))
        {
            return File.ReadAllText(latestKeyFilePath).Trim();
        }
        return null;
    }

    public static void UpdateCurrentKeyId(string keyId)
    {
        string latestKeyFilePath = Path.Combine(Directories.KeyFolder, "current_key_id.txt");
        File.WriteAllText(latestKeyFilePath, keyId);
    }

    public static Dictionary<string, byte[]> GetPublicKeys()
    {
        Dictionary<string, byte[]> keys = [];
        foreach (string keyDir in Directory.GetDirectories(Directories.KeyFolder))
        {
            string keyId = Path.GetFileName(keyDir);
            byte[] publicKey = File.ReadAllBytes(Path.Combine(keyDir, "_public_key.pem"));
            keys.Add(keyId, publicKey);
        }
        return keys;
    }
}
